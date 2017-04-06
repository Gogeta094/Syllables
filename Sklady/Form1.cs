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
            var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = Settings.LastOpenFolderPath;
            dialog.Description = "Open folder with text files.";            

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

            Settings.LastOpenFolderPath = dialog.SelectedPath;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var folderDialog = new FolderBrowserDialog();
            folderDialog.Description = "Save results to folder.";
            folderDialog.SelectedPath = Settings.LastSaveFolderPath;

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (var fileResult in _exportResults)
                {
                    var directoryPath = Directory.CreateDirectory(Path.Combine(folderDialog.SelectedPath, fileResult.FileName)).FullName;

                    SaveFile(fileResult.Syllables, directoryPath, "Syllables.txt");
                    SaveFile(fileResult.FirstSyllables, directoryPath, "FirstSyllables.txt");
                    SaveFile(fileResult.SyllablesCVV, directoryPath, "SyllablesCVV.txt");
                    SaveFile(fileResult.SyllablesFirstCVV, directoryPath, "SyllablesFirstCVV.txt");
                }
            }

            Settings.LastSaveFolderPath = folderDialog.SelectedPath;
        }

        private void SaveFile(string result, string path, string fileName)
        {
            var fullPath = Path.Combine(path, fileName);
            File.WriteAllText(fullPath, result, Encoding.UTF8);
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
