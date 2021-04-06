using AngleSharp;
using System;
using System.Net.Http;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using System.Text;

namespace CambridgeDictTool
{
    internal class Program
    {
        private static async System.Threading.Tasks.Task Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            var inputWordsFile = args[0];
            if (!File.Exists(inputWordsFile))
            {
                Console.WriteLine($"{inputWordsFile} doesn't exist");
                return;
            }

            var words = File.ReadAllLines(inputWordsFile);
            var outputFile = args[1];
            var list = new List<CambridgeWord>();
            if (File.Exists(outputFile))
            {
                var json = File.ReadAllText(outputFile);
                list = JsonSerializer.Deserialize<List<CambridgeWord>>(json);
            }

            foreach (var word in words)
            {
                if (!string.IsNullOrWhiteSpace(word))
                {
                    if (!list.Any(r => r.Text.Equals(word)))
                        list.Add(await CambridgeAPI.GetCambridgeWordAsync(word.Trim()));
                }
            }

            CambridgeAPI.WriteToJson(list, outputFile);
        }
    }
}