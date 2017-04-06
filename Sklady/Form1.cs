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
            mainView1.OnFilesProcessed += MainView1_OnFilesProcessed1;
        }

        private List<FileExportResults> _exportResults;

        private void MainView1_OnFilesProcessed1(List<FileExportResults> result)
        {
            _exportResults = result;
            saveToolStripMenuItem.Enabled = result.Any();
        }

        private IResultsExport _export = ExportResults.Instance;

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //var dialog = new OpenFileDialog();

            //dialog.Filter = "Text Files (*.txt;*.doc;)" + "All files (*.*)|*.*";

            //if (dialog.ShowDialog() == DialogResult.OK)
            //{
            //    Settings.Text = File.ReadAllText(dialog.FileName, Encoding.UTF8);                
            //}
            var dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var path = dialog.SelectedPath;
                var di = new DirectoryInfo(path);

                var files = di.GetFiles("*.txt");

                var texts = new List<InputFileModel>();
                foreach (var file in files)
                {
                    texts.Add(new InputFileModel()
                    {
                        FileName = file.Name,
                        Text = File.ReadAllText(file.FullName, Encoding.UTF8)
                    });
                }

                mainView1.InputData = texts;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var folderDialog = new FolderBrowserDialog();

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (var fileResult in _exportResults)
                {
                    var directoryPath = Directory.CreateDirectory(Path.Combine(folderDialog.SelectedPath, fileResult.FileName)).FullName;

                    SaveSyllables(fileResult.Syllables, directoryPath);
                    SaveFirst(fileResult.FirstSyllables, directoryPath);
                    SaveCVV(fileResult.SyllablesCVV, directoryPath);
                    SaveFirstCVV(fileResult.SyllablesFirstCVV, directoryPath);
                }
            }
        }

        private void SaveSyllables(string result, string selectedPath)
        {
            File.WriteAllText(Path.Combine(selectedPath, "Syllables.txt"), result, Encoding.UTF8);
        }

        private void SaveFirst(string result, string selectedPath)
        {
            File.WriteAllText(Path.Combine(selectedPath, "FirstSyllables.txt"), result, Encoding.UTF8);
        }

        private void SaveCVV(string result, string selectedPath)
        {
            File.WriteAllText(Path.Combine(selectedPath, "SyllablesCVV.txt"), result, Encoding.UTF8);
        }

        private void SaveFirstCVV(string result, string selectedPath)
        {
            File.WriteAllText(Path.Combine(selectedPath, "SyllablesFirstCVV.txt"), result, Encoding.UTF8);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            saveToolStripMenuItem.Enabled = false;            
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
