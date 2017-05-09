using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sklady.Export;
using Sklady.Models;

namespace Sklady
{
    public partial class TestView : UserControl
    {
        public TestView()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox2.Clear();

            var settings = new Settings()
            {
                AbsoluteMeasures = GlobalSettings.AbsoluteMeasures,
                CharactersTable = GlobalSettings.CharactersTable,
                CharactersToRemove = GlobalSettings.CharactersToRemove,
                Language = GlobalSettings.Language,
                LastOpenFolderPath = GlobalSettings.LastOpenFolderPath,
                LastSaveFolderPath = GlobalSettings.LastSaveFolderPath,
                PhoneticsMode = GlobalSettings.PhoneticsMode,
                SeparateAfterFirst = GlobalSettings.SeparateAfterFirst,
                SyllableSeparator = GlobalSettings.SyllableSeparator
            };

            var export = new ResultsExporter(GlobalSettings.CharactersTable, settings);
            var text = richTextBox1.Text;
            var analyzer = new TextAnalyzer(richTextBox1.Text, "", settings, GlobalSettings.CharactersTable, export);

            var result = analyzer.GetResults();
            var resText = export.GetSyllables(result.ReadableResults);
            richTextBox2.Text = resText;
            
            var resCVV = export.GetSyllablesCVV(result.CvvResults);
            richTextBox3.Text = resCVV;            
        }
    }
}
