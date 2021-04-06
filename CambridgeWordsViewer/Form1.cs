using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using CambridgeDictTool;

namespace CambridgeWordsViewer
{
    public partial class Form1 : Form
    {
        private int _current = 0;
        private List<CambridgeWord> _list = new List<CambridgeWord>();
        private List<CambridgeWord> _fullList = new List<CambridgeWord>();
        private Settings _settings = new Settings();

        public Form1()
        {
            InitializeComponent();
            var json = File.ReadAllText("settings.json");
            _settings = JsonSerializer.Deserialize<Settings>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadWords();
        }

        private static Random rng = new Random();

        public static void Shuffle(List<CambridgeWord> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        private void LoadWords()
        {
            btnSave.Enabled = false;
            _fullList = CambridgeAPI.ReadJson(_settings.InputFile);
            _list = (from word in _fullList
                     where (_settings.Level0 && word.Level == 0) ||
                     (_settings.Level1 && word.Level == 1) ||
                     (_settings.Level2 && word.Level == 2) ||
                     (_settings.Level3 && word.Level == 3) ||
                     (_settings.Level4 && word.Level == 4) ||
                     (_settings.Level5 && word.Level == 5)
                     select word
                     ).ToList();

            if (_settings.Random)
                Shuffle(_list);

            if (_settings.MaxItems > 0 && _settings.MaxItems < _list.Count)
                _list = _list.Take(_settings.MaxItems).ToList();

            if (_list.Count == 0)
            {
                richTextBox1.Text = "";
                _current = 0;
            }

            btnPrevious.Enabled = _list.Count > 1;
            btnNext.Enabled = _list.Count > 1;
            if (_list.Count > 0)
            {
                _current = 1;
                ShowWord();
            }
        }

        private void ShowWord()
        {
            var word = _list[_current - 1];
            lWord.Text = word.Text;
            var sb = new StringBuilder();
            sb.Append(@"{\rtf1\ansi");
            foreach (var m in word.Meanings)
            {
                sb.Append(@$"\b {m.Usage} {m.From} - {m.To} \b0 \line ");
                foreach (var e in m.Examples)
                    sb.Append(@$"{e.From} - {e.To} \line ");
                if (m.Phrases.Count > 0)
                {
                    sb.Append(@" \line ");
                    sb.Append(@"\b Phrases: \b0 \line ");
                    foreach (var p in m.Phrases)
                        sb.Append(@$"{p.From} - {p.To} \line ");
                }

                if (m.Synonyms.Count > 0)
                {
                    sb.Append(@"\line ");
                    sb.Append(@"\b Synonyms: \b0 \line ");
                    foreach (var s in m.Synonyms)
                        sb.Append(@$"{s}\line ");
                }

                if (m.Antonyms.Count > 0)
                {
                    sb.Append(@"\line ");
                    sb.Append(@"\b Antonyms: \b0 \line ");
                    foreach (var a in m.Antonyms)
                        sb.Append(@$"{a}\line ");
                }
                sb.Append(@"-----------------------\line ");
            }
            sb.Append(@"}");

            richTextBox1.Rtf = sb.ToString();
            lCounter.Text = $"{_current} / {_list.Count}";
            numLevel.Value = word.Level;
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            _current--;
            if (_current == 0)
                _current = _list.Count;
            ShowWord();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            _current++;
            if (_current > _list.Count)
                _current = 1;
            ShowWord();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            var settings = new SettingsForm(_settings);
            if (settings.ShowDialog() == DialogResult.OK)
                LoadWords();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadWords();
        }

        private void numLevel_ValueChanged(object sender, EventArgs e)
        {
            var word = _list[_current - 1];
            var originalWord = _fullList.FirstOrDefault(w => w.Text.Equals(word.Text));
            word.Level = (int)numLevel.Value;
            if (word.Level != originalWord.Level)
                btnSave.Enabled = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            btnSave.Enabled = false;
            foreach (var word in _list)
            {
                var originalWord = _fullList.FirstOrDefault(w => w.Text.Equals(word.Text));
                if (word.Level != originalWord.Level)
                    originalWord.Level = word.Level;
            }

            CambridgeAPI.WriteToJson(_fullList, _settings.InputFile);
        }
    }
}