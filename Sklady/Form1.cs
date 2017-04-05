using Sklady.Export;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sklady
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();    
        }

        private IResultsExport _export = ExportResults.Instance;

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
           
            dialog.Filter = "Text Files (*.txt;*.doc;)" + "All files (*.*)|*.*";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Settings.Text = File.ReadAllText(dialog.FileName, Encoding.UTF8);                
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {        
            var folderDialog = new FolderBrowserDialog();

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                SaveSyllables(Settings.AnalyzeResults, folderDialog.SelectedPath);
                SaveFirst(Settings.AnalyzeResults, folderDialog.SelectedPath);
                SaveCVV(Settings.AnalyzedCvvResults, folderDialog.SelectedPath);
                SaveFirstCVV(Settings.AnalyzedCvvResults, folderDialog.SelectedPath);
            }
        }

        private void SaveSyllables(List<AnalyzeResults> analyzeResults, string selectedPath)
        {
            var res = _export.GetSyllables(analyzeResults);

            File.WriteAllText(Path.Combine(selectedPath, "Syllables.txt"), res, Encoding.UTF8);
        }

        private void SaveFirst(List<AnalyzeResults> analyzeResults, string selectedPath)
        {
            var res = _export.GetFirstSyllables(analyzeResults);

            File.WriteAllText(Path.Combine(selectedPath, "FirstSyllables.txt"), res, Encoding.UTF8);
        }

        private void SaveCVV(List<AnalyzeResults> analyzeResults, string selectedPath)
        {
            var res = _export.GetSyllablesCVV(analyzeResults);

            File.WriteAllText(Path.Combine(selectedPath, "SyllablesCVV.txt"), res, Encoding.UTF8);
        }

        private void SaveFirstCVV(List<AnalyzeResults> analyzeResults, string selectedPath)
        {
            var res = _export.GetSyllablesFirstCVV(analyzeResults);

            File.WriteAllText(Path.Combine(selectedPath, "SyllablesFirstCVV.txt"), res, Encoding.UTF8);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            saveToolStripMenuItem.Enabled = false;

            Settings.OnResultChanged += (newValue) => saveToolStripMenuItem.Enabled = newValue != null && newValue.Count > 0;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new SettingsForm();
            form.ShowDialog();
        }

        private void charactersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new CharactersBase();
            form.ShowDialog();
        }

        private void mainView1_Load(object sender, EventArgs e)
        {
            
        }
    }
}
