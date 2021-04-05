using AngleSharp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Text.Json;
using System.IO;

namespace CambridgeDictTool
{
    public class CambridgeAPI
    {
        public static string FromDict { get; set; } = "italian";
        public static string ToDict { get; set; } = "english";

        public static async Task<CambridgeWord> GetCambridgeWordAsync(string fromWord)
        {
            var word = new CambridgeWord
            {
                Text = fromWord
            };

            var url = "https://dictionary.cambridge.org/dictionary/" + $"{FromDict}-{ToDict}/{fromWord}";
            var context = BrowsingContext.New(Configuration.Default);
            //Generate HTML DOM for the following source code

            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
                var htmlContent = await response.Content.ReadAsStringAsync();
                var document = await context.OpenAsync(req => req.Content(htmlContent));
                var contentDiv = document.All.FirstOrDefault(row => row.ClassName == "pos-body");

                var translations = contentDiv.GetElementsByClassName("sense-block");
                foreach (var translation in translations)
                {
                    var meaning = new Meaning();
                    var head = translation.GetElementsByClassName("def-head").FirstOrDefault();
                    if (head != null)
                    {
                        var headIndicator = head.GetElementsByClassName("indicator").FirstOrDefault();
                        var usage = head.GetElementsByClassName("usage").FirstOrDefault();
                        meaning.From = headIndicator != null ? headIndicator.TextContent : "";
                        meaning.Usage = usage != null ? usage.TextContent : "";
                    }
                    var defBody = translation.GetElementsByClassName("def-body").FirstOrDefault();
                    if (defBody != null)
                    {
                        var trans = defBody.GetElementsByClassName("trans");
                        var to = new StringBuilder();
                        foreach (var tran in trans)
                        {
                            if (tran.ParentElement.Equals(defBody))
                                to.Append(tran.TextContent);
                        }
                        meaning.To = to.ToString();
                    }

                    var examples = translation.GetElementsByClassName("examp");
                    foreach (var example in examples)
                    {
                        var examplePair = new Pair
                        {
                            From = example.FirstElementChild.TextContent,
                            To = example.FirstElementChild.NextElementSibling.TextContent
                        };
                        meaning.Examples.Add(examplePair);
                    }

                    var synonyms = translation.GetElementsByClassName("synonym").FirstOrDefault();
                    if (synonyms != null)
                    {
                        var items = synonyms.GetElementsByClassName("item");
                        foreach (var item in items)
                        {
                            if (item.FirstElementChild != null)
                                meaning.Synonyms.Add(item.FirstElementChild.TextContent);
                        }
                    }

                    var antonyms = translation.GetElementsByClassName("antonym").FirstOrDefault();
                    if (antonyms != null)
                    {
                        var items = antonyms.GetElementsByClassName("item");
                        foreach (var item in items)
                        {
                            if (item.FirstElementChild != null)
                                meaning.Antonyms.Add(item.FirstElementChild.TextContent);
                        }
                    }

                    var phrases = translation.GetElementsByClassName("phrase-block");
                    foreach (var phrase in phrases)
                    {
                        var trans = phrase.GetElementsByClassName("trans").FirstOrDefault();
                        var phrasePair = new Pair
                        {
                            From = phrase.GetElementsByClassName("phrase-title").FirstOrDefault().TextContent,
                            To = trans != null ? trans.TextContent : ""
                        };
                        meaning.Phrases.Add(phrasePair);
                    }

                    word.Meanings.Add(meaning);
                }
            }

            return word;
        }

        public static void WriteToJson(List<CambridgeWord> items, string outputFile)
        {
            var encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(
                System.Text.Unicode.UnicodeRanges.All);
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = encoder
            };
            var jsonOutput = JsonSerializer.Serialize(items, options);
            jsonOutput = System.Text.RegularExpressions.Regex.Unescape(jsonOutput);
            File.WriteAllText(outputFile, jsonOutput);
        }

        public static List<CambridgeWord> ReadJson(string inputFile)
        {
            var json = File.ReadAllText(inputFile, Encoding.UTF8);
            var list = JsonSerializer.Deserialize<List<CambridgeWord>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return list;
        }
    }
}