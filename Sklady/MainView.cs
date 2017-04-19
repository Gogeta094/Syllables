using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Sklady.Export;
using Sklady.Models;

namespace Sklady
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
        }

        public event Action<ExportResults> OnFilesProcessed;
        private const int UPDATE_UI_EVERY_N_ITEMS = 2000;

        private CharactersTable charsTable = CharactersTable.Instance;
        private ResultsExporter _export = ResultsExporter.Instance;       

        private List<InputFileModel> _inputData;
        public List<InputFileModel> InputData
        {
            get
            {
                return _inputData;
            }
            set
            {
                _inputData = value;
                OnInputDataChanged();
            }
        }

        private void OnInputDataChanged()
        {
            progressBar1.Value = 0;
            CreateProgressBars();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (InputData == null || !InputData.Any())
            {
                MessageBox.Show("No text file selected.");
                return;
            }                  

            var analyzers = new List<TextAnalyzer>();
            for (var i = 0; i < InputData.Count; i++)
            {
                var textAnalyzer = new TextAnalyzer(InputData[i].Text, InputData[i].FileName);
                textAnalyzer.OnWordAnalyzed += Analyzer_OnWordAnalyzed;
                textAnalyzer.OnErrorOccured += Analyzer_OnErrorOccured;

                analyzers.Add(textAnalyzer);
            }

            progressBar1.Maximum = InputData.Count;

            var exportResult = new ExportResults();
            var fileProcessingResults = new List<FileProcessingResult>();

            var task = Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(analyzers, textAnalyzer =>
                {
                    var res = textAnalyzer.GetResults();
                    fileProcessingResults.Add(res);

                    exportResult.FileExportResults.Add(new FileExportResults()
                    {
                        Syllables = _export.GetSyllables(res.ReadableResults),
                        FirstSyllables = _export.GetFirstSyllables(res.ReadableResults),
                        SyllablesCVV = _export.GetSyllablesCVV(res.CvvResults),
                        SyllablesFirstCVV = _export.GetSyllablesFirstCVV(res.CvvResults),
                        FileName = textAnalyzer.FileName
                    });

                    OnFileProcessed();
                });
                
                exportResult.StatisticsTableCsv = _export.GetStatisticsTableCsv(fileProcessingResults);

                if (OnFilesProcessed != null)
                    OnFilesProcessed(exportResult);
            });
        }

        private void OnFileProcessed()
        {
            if (progressBar1.InvokeRequired)
            {
                progressBar1.Invoke((MethodInvoker)delegate ()
                {
                    progressBar1.Value += 1;
                });
            }
            else
            {
                progressBar1.Value += 1;
            }
        }

        private void CreateProgressBars()
        {
            panel1.Controls.Clear();

            for (var i = 0; i < InputData.Count; i++)
            {
                var item = InputData[i];

                var label = new Label();
                label.Text = item.FileName;
                label.Top = 15 + i * 30;
                label.Left = 20;
                label.Height = 15;
                label.Name = item.FileName + "lbl";

                var progressBar = new ProgressBar();
                progressBar.Top = 15 + i * 30;
                progressBar.Left = 170;
                progressBar.Height = 15;
                progressBar.Name = item.FileName + "pb";

                panel1.Controls.Add(label);
                panel1.Controls.Add(progressBar);
            }
        }

        private void Analyzer_OnErrorOccured(Exception arg1, string word, string file)
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.BeginInvoke((MethodInvoker)delegate ()
                {
                    richTextBox1.Text += String.Format("{0} Error occured processing next word - {1}\n", file, word);
                });
            }
            else
            {
                richTextBox1.Text += String.Format("{0} Error occured processing next word - {1}\n", file, word);
            }
        }
        
        private void Analyzer_OnWordAnalyzed(int current, int total, string fileName)
        {
            var progressBar = (ProgressBar)panel1.Controls.Find(fileName + "pb", false).First();

            UpdateProgressBar(current, total, progressBar);
        }

        private void UpdateProgressBar(int current, int total, ProgressBar progressBar)
        {
            if (current % UPDATE_UI_EVERY_N_ITEMS != 0 && total - current > UPDATE_UI_EVERY_N_ITEMS)
            {
                return;
            }

            if (progressBar.InvokeRequired)
            {
                progressBar.Invoke((MethodInvoker)delegate ()
                {
                    progressBar.Maximum = total;
                    progressBar.Value = current;
                });
            }
            else
            {
                progressBar.Maximum = total;
                progressBar.Value = current;
            }
        }

        private void MainView_Load(object sender, EventArgs e)
        {

        }
    }
}
