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
            var words = File.ReadAllLines(args[0]);
            var list = new List<CambridgeWord>();
            foreach (var word in words)
            {
                if (!string.IsNullOrWhiteSpace(word))
                    list.Add(await CambridgeAPI.GetCambridgeWordAsync(word.Trim()));
            }
            CambridgeAPI.WriteToJson(list, args[1]);
        }
    }
}