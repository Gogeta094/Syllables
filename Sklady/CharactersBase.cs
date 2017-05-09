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
    public partial class CharactersBase : Form
    {
        public CharactersBase()
        {
            InitializeComponent();
        }

        private CharactersTable charsTable = GlobalSettings.CharactersTable;
        private BindingList<Character> _consonants;
        private BindingList<Character> _vowels;
        private BindingList<string> _charsToRemove;

        private void CharactersBase_Load(object sender, EventArgs e)
        {
            _consonants = new BindingList<Character>(charsTable.GetConsonants());
            _vowels = new BindingList<Character>(charsTable.GetVowels());
            _charsToRemove = new BindingList<string>(GlobalSettings.CharactersToRemove);


            listBox1.DataSource = _consonants;
            listBox2.DataSource = _vowels;
            listBox3.DataSource = _charsToRemove;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            char character = ' ';
            int power = 0;

            try
            {
                character = Char.Parse(tbConsValue.Text);
                power = Int32.Parse(tbConsPower.Text);
            }catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }

            if (charsTable.Get(character) != null)
            {
                MessageBox.Show("Character already registered.");
                return;
            }

            _consonants.Add(new Character()
            {
                CharacterValue = character,
                Power = power
            });
        }

        private void btnRemoveConsonant_Click(object sender, EventArgs e)
        {
            var selected = listBox1.SelectedIndex;
            _consonants.RemoveAt(selected);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAddVowel_Click(object sender, EventArgs e)
        {
            char character = ' ';

            try
            {
                character = Char.Parse(tbVowel.Text);                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }

            if (charsTable.Get(character) != null)
            {
                MessageBox.Show("Character already registered.");
                return;
            }

            _vowels.Add(new Character()
            {
                CharacterValue = character,
                Power = -1
            });
        }

        private void btnRemoveVowel_Click(object sender, EventArgs e)
        {
            var selected = listBox2.SelectedIndex;
            _vowels.RemoveAt(selected);
        }

        private void btnAddChrToRemove_Click(object sender, EventArgs e)
        {
            string value = "";

            try
            {
                value = tbChToRemove.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }

            if (_charsToRemove.Any(c => c == value))
            {
                MessageBox.Show("Character already registered.");
                return;
            }

            _charsToRemove.Add(value);
        }

        private void btnRemoveChr_Click(object sender, EventArgs e)
        {
            var selected = listBox3.SelectedIndex;
            _charsToRemove.RemoveAt(selected);
        }
    }
}
