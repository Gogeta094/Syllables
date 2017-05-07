using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sklady
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            tbSeparator.Text = Settings.SyllableSeparator;
            cbSeparationMode.SelectedIndex = Settings.SeparateAfterFirst ? 0 : 1;
            cbCharactersTable.SelectedIndex = (int) Settings.CharactersTable;
            cbPhoneticsMode.Checked = Settings.PhoneticsMode;
            cbbLanguage.DataSource = new BindingList<string>(Enum.GetNames(typeof(Languages)));
            cbbLanguage.SelectedIndex = (int)Settings.Language;
            cbAbsoluteMeasures.Checked = Settings.AbsoluteMeasures;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Settings.SyllableSeparator = tbSeparator.Text;
            Settings.SeparateAfterFirst = cbSeparationMode.SelectedText.Equals("c-cc");
            Settings.CharactersTable = (Table) cbCharactersTable.SelectedIndex;
            Settings.PhoneticsMode = cbPhoneticsMode.Checked;
            Settings.Language = (Languages)cbbLanguage.SelectedIndex;
            Settings.AbsoluteMeasures = cbAbsoluteMeasures.Checked;

            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
