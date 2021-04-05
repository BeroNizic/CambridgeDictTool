using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeWordsViewer
{
    public class Settings
    {
        public string InputFile { get; set; }
        public bool Level0 { get; set; }
        public bool Level1 { get; set; }
        public bool Level2 { get; set; }
        public bool Level3 { get; set; }
        public bool Level4 { get; set; }
        public bool Level5 { get; set; }
        public bool Random { get; set; }
        public int MaxItems { get; set; }
    }
}