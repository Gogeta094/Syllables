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
            lettersView1.OnFilesProcessed += LettersView1_OnFilesProcessed;
        }

        private void LettersView1_OnFilesProcessed(ExportResults result)
        {
            _exportResults = result;
            UpdateSaveButton();
        }

        private ExportResults _exportResults;

        private void MainView1_OnFilesProcessed1(ExportResults result)
        {
            _exportResults = result;
            UpdateSaveButton();
        }       

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = GlobalSettings.LastOpenFolderPath;
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
                lettersView1.InputData = texts;
            }

            GlobalSettings.LastOpenFolderPath = dialog.SelectedPath;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_exportResults.FullResults)
            {
                SaveFullResults();
            }
            else
            {
                SaveLettersResults();
            }
        }

        private void SaveLettersResults()
        {
            var folderDialog = new FolderBrowserDialog();
            folderDialog.Description = "Save results to folder.";
            folderDialog.SelectedPath = GlobalSettings.LastSaveFolderPath;

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                SaveCvv(_exportResults.StatisticsTableCsv, folderDialog.SelectedPath);
            }

            GlobalSettings.LastSaveFolderPath = folderDialog.SelectedPath;
        }

        private void SaveFullResults()
        {
            const string SyllablesFolderName = "Syllables";
            const string FirstSyllablesFolderName = "FirstSyllables";
            const string SyllablesCVVFolderName = "SyllablesCVV";
            const string FirstSyllablesCVVFolderName = "FirstSyllablesCVV";

            var folderDialog = new FolderBrowserDialog();
            folderDialog.Description = "Save results to folder.";
            folderDialog.SelectedPath = GlobalSettings.LastSaveFolderPath;

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                var syllablesDirectory = Directory.CreateDirectory(Path.Combine(folderDialog.SelectedPath, SyllablesFolderName)).FullName;
                var syllablesFirstDirectory = Directory.CreateDirectory(Path.Combine(folderDialog.SelectedPath, FirstSyllablesFolderName)).FullName;
                var syllablesCVVDirectory = Directory.CreateDirectory(Path.Combine(folderDialog.SelectedPath, SyllablesCVVFolderName)).FullName;
                var syllablesFirstCVVDirectory = Directory.CreateDirectory(Path.Combine(folderDialog.SelectedPath, FirstSyllablesCVVFolderName)).FullName;

                foreach (var fileResult in _exportResults.FileExportResults)
                {
                    SaveFile(fileResult.Syllables, syllablesDirectory, fileResult.FileName);
                    SaveFile(fileResult.FirstSyllables, syllablesFirstDirectory, fileResult.FileName);
                    SaveFile(fileResult.SyllablesCVV, syllablesCVVDirectory, fileResult.FileName);
                    SaveFile(fileResult.SyllablesFirstCVV, syllablesFirstCVVDirectory, fileResult.FileName);
                }

                SaveCvv(_exportResults.StatisticsTableCsv, folderDialog.SelectedPath);
            }

            GlobalSettings.LastSaveFolderPath = folderDialog.SelectedPath;
        }

        private void SaveCvv(string statisticsTableCsv, string path)
        {
            var fullPath = Path.Combine(path, "Statistics.csv");
            File.WriteAllText(fullPath, statisticsTableCsv, Encoding.UTF8);
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

        private void UpdateSaveButton()
        {
            if (menuStrip1.InvokeRequired)
            {
                menuStrip1.Invoke((MethodInvoker)delegate ()
               {
                   saveToolStripMenuItem.Enabled = !String.IsNullOrEmpty(_exportResults.StatisticsTableCsv);
               });
            }
            else
            {
                saveToolStripMenuItem.Enabled = !String.IsNullOrEmpty(_exportResults.StatisticsTableCsv);
            }
        }

        private void mainView1_Load(object sender, EventArgs e)
        {

        }
    }
}
