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
        private CharactersTable charsTable = CharactersTable.Instance;

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
            var sb = new StringBuilder();
            var anResults = analyzeResults.Select(r => r).ToList();

            for (var i = 0; i < anResults.Count; i++)
            {
                sb.Append(String.Join(Settings.SyllableSeparator, anResults[i].Syllables) + " ");
            }

            File.WriteAllText(Path.Combine(selectedPath, "Syllables.txt"), sb.ToString(), Encoding.UTF8);
        }

        private void SaveFirst(List<AnalyzeResults> analyzeResults, string selectedPath)
        {
            var sb = new StringBuilder();
            var anResults = analyzeResults.Select(r => r).ToList();
           
            anResults = TakeOnlyFirstSyllable(anResults);

            for (var i = 0; i < anResults.Count; i++)
            {
                sb.Append(String.Join(Settings.SyllableSeparator, anResults[i].Syllables) + " ");
            }

            File.WriteAllText(Path.Combine(selectedPath, "FirstSyllables.txt"), sb.ToString(), Encoding.UTF8);
        }

        private void SaveCVV(List<AnalyzeResults> analyzeResults, string selectedPath)
        {
            var sb = new StringBuilder();
            var anResults = analyzeResults.Select(r => new AnalyzeResults() { Syllables = r.Syllables.Select(s => (string)s.Clone()).ToArray() }).ToList();

            anResults = ConvertToCvv(anResults);

            for (var i = 0; i < anResults.Count; i++)
            {
                sb.Append(String.Join(Settings.SyllableSeparator, anResults[i].Syllables) + " ");
            }

            File.WriteAllText(Path.Combine(selectedPath, "SyllablesCVV.txt"), sb.ToString(), Encoding.UTF8);
        }

        private void SaveFirstCVV(List<AnalyzeResults> analyzeResults, string selectedPath)
        {
            var sb = new StringBuilder();
            var anResults = analyzeResults.Select(r => new AnalyzeResults() { Syllables = r.Syllables.Select(s => (string)s.Clone()).ToArray() }).ToList();

            anResults = TakeOnlyFirstSyllable(anResults);
            anResults = ConvertToCvv(anResults);

            for (var i = 0; i < anResults.Count; i++)
            {
                sb.Append(String.Join(Settings.SyllableSeparator, anResults[i].Syllables) + " ");
            }

            File.WriteAllText(Path.Combine(selectedPath, "SyllablesFirstCVV.txt"), sb.ToString(), Encoding.UTF8);
        }  

        private List<AnalyzeResults> TakeOnlyFirstSyllable(List<AnalyzeResults> anResults)
        {
            return anResults.Select(c => new AnalyzeResults()
            {
                Word = c.Word,
                Syllables = new string[] { c.Syllables.First() }
            }).ToList();
        }

        private List<AnalyzeResults> ConvertToCvv(List<AnalyzeResults> anResults)
        {            
            foreach (var resultitem in anResults)
            {
                for (var i = 0; i < resultitem.Syllables.Length; i++)
                {
                    resultitem.Syllables[i] = new string(resultitem.Syllables[i].Select(s => charsTable.isConsonant(s) ? 'c' : 'v').ToArray());
                }
            }

            return anResults;
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
    }
}
