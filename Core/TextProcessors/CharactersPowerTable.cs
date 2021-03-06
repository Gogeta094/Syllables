﻿using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sklady
{
    public class CharactersTable
    {
        private List<Character> _vowel;
        private static CharactersTable _instance;

        private List<Character> _table1;
        private List<Character> _table2;

        private List<Character> _currentTable;

        private Table selectedTable;

        public Table SelectedTable
        {
            get
            {
                return selectedTable;
            }
            set
            {
                ChangeTable(value);
                selectedTable = value;
            }
        }

        
        public CharactersTable(Table selectedTable)
        {
            _vowel = GetVowelCharacters();
            _table1 = GetFirstTable();
            _table2 = GetSecondTable();
            SelectedTable = selectedTable;
            _currentTable = SelectedTable == Table.Table1 ? _table1 : _table2;
        }

        public bool isConsonant(char character)
        {
            return _currentTable.Any(c => c.CharacterValue == character);
        }

        public Character Get(char character)
        {
            return _currentTable.Union(_vowel).SingleOrDefault(c => c.CharacterValue == character);
        }

        public int GetPower(char character)
        {
            return _currentTable.Union(_vowel).Single(c => c.CharacterValue == character).Power;
        }

        private void ChangeTable(Table table)
        {
            _currentTable = table == Table.Table1 ? _table1 : _table2;
        }

        public List<Character> GetConsonants()
        {
            return _currentTable;
        }

        public List<Character> GetVowels()
        {
            return _vowel;
        }

        public void Add(Character character)
        {
            _currentTable.Add(character);
        }

        public void Remove(char character)
        {
           _currentTable.RemoveAll(c => c.CharacterValue == character);
        }        

        private List<Character> GetFirstTable()
        {
            return new List<Character>()
            {
                new Character() {CharacterValue = 'ь', Power = 0},
                new Character() {CharacterValue = '-', Power = 0},
                new Character() {CharacterValue = '\'', Power = 0},
                new Character() {CharacterValue = 'ъ', Power = 0},
                new Character() {CharacterValue = 'ф', Power = 1},
                new Character() {CharacterValue = 'с', Power = 1},
                new Character() {CharacterValue = 'х', Power = 1},
                new Character() {CharacterValue = 'ц', Power = 1},
                new Character() {CharacterValue = 'ч', Power = 1},
                new Character() {CharacterValue = 'ш', Power = 1},
                new Character() {CharacterValue = 'щ', Power = 1},
                new Character() {CharacterValue = 'd', Power = 2},
                new Character() {CharacterValue = 'z', Power = 2},
                new Character() {CharacterValue = 'j', Power = 2},
                new Character() {CharacterValue = 'г', Power = 2},
                new Character() {CharacterValue = 'з', Power = 2},
                new Character() {CharacterValue = 'ж', Power = 2},
                new Character() {CharacterValue = 'v', Power = 2},
                new Character() {CharacterValue = 'в', Power = 2},
                new Character() {CharacterValue = 'к', Power = 3},
                new Character() {CharacterValue = 'п', Power = 3},
                new Character() {CharacterValue = 'т', Power = 3},
                new Character() {CharacterValue = 'б', Power = 4},
                new Character() {CharacterValue = 'д', Power = 4},
                new Character() {CharacterValue = 'ґ', Power = 4},
                new Character() {CharacterValue = 'м', Power = 5},
                new Character() {CharacterValue = 'н', Power = 5},
                new Character() {CharacterValue = 'р', Power = 6},
                new Character() {CharacterValue = 'л', Power = 6},
                new Character() {CharacterValue = 'w', Power = 6},
                new Character() {CharacterValue = 'u', Power = 7},
                new Character() {CharacterValue = 'Y', Power = 7},
            };
        }

        private List<Character> GetSecondTable()
        {
            return new List<Character>()
            {
                new Character() {CharacterValue = 'ь', Power = 0},
                new Character() {CharacterValue = '-', Power = 0},
                new Character() {CharacterValue = '\'', Power = 0},
                new Character() {CharacterValue = 'ъ', Power = 0},
                new Character() {CharacterValue = 'п', Power = 1},
                new Character() {CharacterValue = 'т', Power = 1},
                new Character() {CharacterValue = 'к', Power = 1},
                new Character() {CharacterValue = 'ф', Power = 2},
                new Character() {CharacterValue = 'с', Power = 2},
                new Character() {CharacterValue = 'х', Power = 2},
                new Character() {CharacterValue = 'ц', Power = 2},
                new Character() {CharacterValue = 'ч', Power = 2},
                new Character() {CharacterValue = 'ш', Power = 2},
                new Character() {CharacterValue = 'щ', Power = 2},
                new Character() {CharacterValue = 'б', Power = 3},
                new Character() {CharacterValue = 'д', Power = 3},
                new Character() {CharacterValue = 'ґ', Power = 3},
                new Character() {CharacterValue = 'v', Power = 4},
                new Character() {CharacterValue = 'j', Power = 4},
                new Character() {CharacterValue = 'г', Power = 4},
                new Character() {CharacterValue = 'з', Power = 4},
                new Character() {CharacterValue = 'ж', Power = 4},
                new Character() {CharacterValue = 'd', Power = 4},
                new Character() {CharacterValue = 'z', Power = 4},
                new Character() {CharacterValue = 'м', Power = 5},
                new Character() {CharacterValue = 'н', Power = 5},
                new Character() {CharacterValue = 'л', Power = 6},
                new Character() {CharacterValue = 'w', Power = 6},
                new Character() {CharacterValue = 'р', Power = 7},
                new Character() {CharacterValue = 'u', Power = 8},
                new Character() {CharacterValue = 'Y', Power = 8},
            };
        }

        private List<Character> GetVowelCharacters()
        {
            return new List<Character>()
            {
                new Character() { CharacterValue = 'а', Power = -1 },
                new Character() { CharacterValue = 'о', Power = -1 },
                new Character() { CharacterValue = 'у', Power = -1 },
                new Character() { CharacterValue = 'и', Power = -1 },
                new Character() { CharacterValue = 'е', Power = -1 },
                new Character() { CharacterValue = 'і', Power = -1 },
                new Character() { CharacterValue = 'ї', Power = -1 },
                new Character() { CharacterValue = 'є', Power = -1 },
                new Character() { CharacterValue = 'я', Power = -1 },
                new Character() { CharacterValue = 'ю', Power = -1 },
                new Character() { CharacterValue = 's', Power = -1 },
                new Character() { CharacterValue = 'm', Power = -1 },
                new Character() { CharacterValue = 'ы', Power = -1 },
                new Character() { CharacterValue = 'э', Power = -1 }, 
                new Character() { CharacterValue = 'ѣ', Power = -1 },
                new Character() { CharacterValue = 'ђ', Power = -1 },
                new Character() { CharacterValue = 'Ђ', Power = -1 },
                new Character() { CharacterValue = 'ё', Power = -1 },
                new Character() { CharacterValue = 'ӕ', Power = -1 },//ӕ
                new Character() { CharacterValue = 'Ü', Power = -1 },
                new Character() { CharacterValue = 'b', Power = -1 }
            };
        }

    }
}
