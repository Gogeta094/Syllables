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

        public List<InputFileModel> InputData { get; set; }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!InputData.Any())
            {
                MessageBox.Show("No text file selected.");
                return;
            }

            var analyzers = new List<TextAnalyzer>();
            for (var i = 0; i < InputData.Count; i++)
            {
                var textAnalyzer = new TextAnalyzer(InputData[i].Text, InputData[i].FileName);
                //textAnalyzer.OnWordAnalyzed += Analyzer_OnWordAnalyzed;
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

        private void Analyzer_OnErrorOccured(Exception arg1, string arg2)
        {
            richTextBox1.Text += String.Format("Error occured processing next word - {0}\n", arg2);
        }

        private void Analyzer_OnWordAnalyzed(int current, int total)
        {
            progressBar1.Maximum = total;
            progressBar1.Value = current;
        }    

        private void MainView_Load(object sender, EventArgs e)
        {
            
        }       
    }
}
