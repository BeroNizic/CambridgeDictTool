using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace CambridgeWordsViewer
{
    public partial class SettingsForm : Form
    {
        private Settings _settings = new Settings();

        public SettingsForm(Settings settings)
        {
            InitializeComponent();
            _settings = settings;
            DialogResult = DialogResult.Cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _settings.InputFile = tbInputFile.Text;
            _settings.Level0 = cb0.Checked;
            _settings.Level1 = cb1.Checked;
            _settings.Level2 = cb2.Checked;
            _settings.Level3 = cb3.Checked;
            _settings.Level4 = cb4.Checked;
            _settings.Level5 = cb5.Checked;
            _settings.Random = cbRandom.Checked;
            _settings.MaxItems = (int)numMaxItems.Value;
            var json = JsonSerializer.Serialize(_settings,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            File.WriteAllText("settings.json", json);
            MessageBox.Show("Saved successfully!");
            DialogResult = DialogResult.OK;
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "json files (*.json)|*.json"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
                tbInputFile.Text = ofd.FileName;
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            cb0.Checked = _settings.Level0;
            cb1.Checked = _settings.Level1;
            cb2.Checked = _settings.Level2;
            cb3.Checked = _settings.Level3;
            cb4.Checked = _settings.Level4;
            cb5.Checked = _settings.Level5;
            cbRandom.Checked = _settings.Random;
            numMaxItems.Value = _settings.MaxItems;
            tbInputFile.Text = _settings.InputFile;
        }
    }
}