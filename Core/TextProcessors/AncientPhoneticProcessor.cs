﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sklady.TextProcessors
{
    public class AncientPhoneticProcessor : PhoneticProcessorBase
    {
        public AncientPhoneticProcessor(CharactersTable charactersTable) : base(charactersTable)
        {
        }

        public override string Process(string input)
        {
            var res = HandleJ(input);
            res = HandleSolidAndSoftSigns(res);
            res = ReplaceAncientSymbols(res);            
            res = ProcessV(res);

            return res;
        }

        private string ReplaceAncientSymbols(string word)
        {
            return new StringBuilder(word)
                .Replace("ъ", "s")
                .Replace("ь", "m")
                .ToString();
        }

        private string HandleJ(string input)
        {
            var res = Regex.Replace(input, "іо", "jо");
            res = Regex.Replace(res, "є", "jе");
            res = Regex.Replace(res, "ю", "jу");
            res = Regex.Replace(res, "я", "jа");
            res = Regex.Replace(res, "іа", "jа");
            res = Regex.Replace(res, "иа", "jа");
            res = Regex.Replace(res, "іі", "jі");
            res = Regex.Replace(res, "иі", "jі");            
            res = Regex.Replace(res, "іе", "jе");
            res = Regex.Replace(res, "ие", "jе");
            res = Regex.Replace(res, "ио", "jо");
            res = Regex.Replace(res, "іу", "jу");
            res = Regex.Replace(res, "иу", "jу");
            res = Regex.Replace(res, "іы", "jы");
            res = Regex.Replace(res, "иы", "jы");            
            
            //res = Regex.Replace(res, "оу", "Ü");
            res = Regex.Replace(res, "йе", "jе");
            res = Regex.Replace(res, "йа", "jа");
            res = Regex.Replace(res, "йу", "jу");
            res = Regex.Replace(res, "йі", "jі");
            res = Regex.Replace(res, "йо", "jо");

            return res;
        }

        private string HandleSolidAndSoftSigns(string input)
        {
            var res = Regex.Replace(input, "ьі", "ы");
            res = Regex.Replace(res, "ъі", "ы");
            res = Regex.Replace(res, "ьи", "ы");
            res = Regex.Replace(res, "ъи", "ы");

            return res;
        }

        public string ProcessV(string word)
        {
            var indexOfV = word.IndexOf('в');

            while (indexOfV != -1)
            {
                if (IsFirstOrHasVowelBefore(word, indexOfV) && IsLastOrHasConsonantAfter(word, indexOfV))
                {
                    word = word.Remove(indexOfV, 1).Insert(indexOfV, "u");
                }
                else if (HasConsonantBeforeAndVowelAfter(word, indexOfV))
                {
                    word = word.Remove(indexOfV, 1).Insert(indexOfV, "w");
                }
                else
                {
                    //word = word.Remove(indexOfV, 1).Insert(indexOfV, "u");
                }

                indexOfV = word.IndexOf('в', indexOfV + 1);
            }

            return word;
        }

        private bool IsFirstOrHasVowelBefore(string word, int indexOfV)
        {
            return indexOfV == 0 || !CharactersTable.isConsonant(word[indexOfV - 1]);
        }

        private bool IsLastOrHasConsonantAfter(string word, int indexOfV)
        {
            return indexOfV == word.Length - 1 || CharactersTable.isConsonant(word[indexOfV + 1]);
        }

        private bool HasConsonantBeforeAndVowelAfter(string word, int indexOfV)
        {
            return (indexOfV != 0 && CharactersTable.isConsonant(word[indexOfV - 1]))
                   && (indexOfV != word.Length - 1 && !CharactersTable.isConsonant(word[indexOfV + 1]));
        }

        public override string RemoveTechnicalCharacters(string word)
        {
            return word.Replace("Ü", "оу")
                       .Replace("s", "ъ")
                       .Replace("m", "ь")
                       .Replace("b", "ьі");         
        }
    }
}
