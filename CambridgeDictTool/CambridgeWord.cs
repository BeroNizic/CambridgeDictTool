using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeDictTool
{
    public class CambridgeWord
    {
        public string Text { get; set; }
        public int Level { get; set; }
        public List<Meaning> Meanings { get; set; } = new List<Meaning>();
    }
}