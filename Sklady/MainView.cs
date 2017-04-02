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

namespace Sklady
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
        }
        
        private CharactersTable charsTable = CharactersTable.Instance;

        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(Settings.Text))
            {
                MessageBox.Show("No text file selected.");
                return;
            }

            var analyzer = new TextAnalyzer(Settings.Text);
            analyzer.OnWordAnalyzed += Analyzer_OnWordAnalyzed;
            analyzer.OnErrorOccured += Analyzer_OnErrorOccured;
            var result = analyzer.GetResults();
            Settings.AnalyzeResults = result;                 
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
