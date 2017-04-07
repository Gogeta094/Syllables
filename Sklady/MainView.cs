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

namespace Sklady
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
        }

        public event Action<List<FileExportResults>> OnFilesProcessed;

        private CharactersTable charsTable = CharactersTable.Instance;
        private ExportResults _export = ExportResults.Instance;

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

            var exportResults = new List<FileExportResults>();

            Parallel.ForEach(analyzers, textAnalyzer =>
            {
                var res = textAnalyzer.GetResults();
                var cvv = textAnalyzer.ResultCVV;

                exportResults.Add(new FileExportResults()
                {
                    Syllables = _export.GetSyllables(res),
                    FirstSyllables = _export.GetFirstSyllables(res),
                    SyllablesCVV = _export.GetSyllablesCVV(cvv),
                    SyllablesFirstCVV = _export.GetSyllablesFirstCVV(cvv),
                    FileName = textAnalyzer.FileName
                });
            });

            if (OnFilesProcessed != null)
                OnFilesProcessed(exportResults);
        }

        private void CreateProgressBars()
        {
            for (var i = 0; i < InputData.Count; i++)
            {
                var item = InputData[i];

                var label = new Label();
                label.Text = item.FileName;
                label.Top = i * 30;
                label.Left = 20;
                label.Height = 15;
                label.Name = item.FileName + "lbl";

                var progressBar = new ProgressBar();
                progressBar.Top = i * 30;
                progressBar.Left = 150;
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
            if (current % 1000 != 0)
            {
                return;
            }

            if (progressBar.InvokeRequired)
            {
                progressBar.BeginInvoke((MethodInvoker)delegate ()
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
