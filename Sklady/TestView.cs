using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

            var text = richTextBox1.Text;

            var analyzer = new TextAnalyzer(richTextBox1.Text, "");

            var result = analyzer.GetResults();

            foreach (var item in result)
            {
                richTextBox2.Text += " " + String.Join("-", item.Syllables);
            }
        }
    }
}
