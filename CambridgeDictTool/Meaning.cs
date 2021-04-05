using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeDictTool
{
    public class Meaning
    {
        public string From { get; set; } = "";
        public string To { get; set; } = "";
        public string Usage { get; set; } = "";
        public List<string> Synonyms { get; set; } = new List<string>();
        public List<string> Antonyms { get; set; } = new List<string>();
        public List<Pair> Phrases { get; set; } = new List<Pair>();

        public List<Pair> Examples { get; set; } = new List<Pair>();
    }
}