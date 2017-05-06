﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sklady.TextProcessors
{
    public class PhoneticProcessor
    {
        private CharactersTable _table = CharactersTable.Instance;
        private string[] dzPrefixes = new string[] { "під", "над", "від" };

        public string Process(string input)
        {
            var res = ProcessTwoSoundingLetters(input);
            res = ProcessDoubleConsonants(res);
            res = ProcessDz(res);
            res = ReductionReplacements(res);
            res = AsymilativeReplacements(res);


            return res;
        }

        private string ReductionReplacements(string res)
        {
            res = Regex.Replace(res, "нтськ", "нск");
            res = Regex.Replace(res, "стськ", "ск");
            res = Regex.Replace(res, "нтст", "нст");
            res = Regex.Replace(res, "стц", "сц");
            res = Regex.Replace(res, "стч", "шч");
            res = Regex.Replace(res, "стд", "зд");
            res = Regex.Replace(res, "стс", "с");
            res = Regex.Replace(res, "стн", "сн");
            res = Regex.Replace(res, "нтс", "нс");
            res = Regex.Replace(res, "нтс", "нс");
            res = Regex.Replace(res, "тст", "ц");
            res = Regex.Replace(res, "тьс", "ц");

            return res;
        }

        private string AsymilativeReplacements(string res)
        {
            res = Regex.Replace(res, "^(с|з)(ш|ж)", "$2");
            res = Regex.Replace(res, "(с)(ш)", "$2");
            res = Regex.Replace(res, "(ч)(ц)", "$2");
            res = Regex.Replace(res, "(т)(с)", "ц");
            res = Regex.Replace(res, "(т)(ц)", "$2");
            res = Regex.Replace(res, "(т)(ч)", "$2");

            return res;
        }

        private string ProcessDoubleConsonants(string input)
        {
            for (var i = 0; i < input.Length - 1; i++)
            {
                if (input[i] == input[i + 1]
                    && _table.isConsonant(input[i]))
                {
                    input = input.Remove(i + 1, 1);
                }
            }

            return input;
        }

        public string Unprocess(string input)
        {
            var res = UnprocessTwoSoundingLetters(input);

            return res;
        }

        private string UnprocessTwoSoundingLetters(string input)
        {
            var res = input.Replace("jу", "ю")
                        .Replace("jа", "я")
                        .Replace("jі", "ї");

            if (Settings.Language == Languages.Ukraine)
            {
                res = res.Replace("jе", "є")
                         .Replace("шч", "щ");
            }
            else if (Settings.Language == Languages.Russian)
            {
                res = res.Replace("jе", "ё");
                res = res.Replace("jи", "и");
            }

            return res;
        }

        private string ProcessTwoSoundingLetters(string input)
        {
            input = ReplacePhoneticCharacter('ю', "jу", input);
            input = ReplacePhoneticCharacter('я', "jа", input);
            input = ReplacePhoneticCharacter('є', "jе", input);
            input = ReplacePhoneticCharacter('ї', "jі", input);
            input = ReplacePhoneticCharacter('щ', "шч", input);

            if (Settings.Language == Languages.Russian)
            {
                input = ReplacePhoneticCharacter('ё', "jе", input);
                input = HandleRussianU(input);                
            }

            return input;
        }

        private string HandleRussianU(string input)
        {
            var indexOfU = input.IndexOf("и");

            while (indexOfU != -1)
            {
                if (indexOfU > 0 && input[indexOfU - 1] == 'ь')
                {
                    input = input.Remove(indexOfU, 1).Insert(indexOfU, "jи");
                }
                indexOfU = input.IndexOf("и", indexOfU + 1);
            }

            return input;
        }

        private string ReplacePhoneticCharacter(char charToReplace, string replacementText, string input)
        {
            var indexofChar = input.IndexOf(charToReplace);

            while (indexofChar != -1)
            {
                if (indexofChar == 0 || !_table.isConsonant(input[indexofChar - 1]))
                {
                    input = input.Remove(indexofChar, 1).Insert(indexofChar, replacementText);
                }
                indexofChar = input.IndexOf(charToReplace, indexofChar + 1);
            }

            return input;
        }

        public string ProcessNonStableCharacters(string word)
        {
            var indexOfV = word.IndexOf('в');

            while (indexOfV != -1)
            {
                if (indexOfV == 0)
                {
                    indexOfV = word.IndexOf('в', indexOfV + 1);
                    continue;
                }
                if (indexOfV == word.Length - 1 || indexOfV == word.Length)
                {
                    break;
                }

                if (!_table.isConsonant(word[indexOfV - 1]) && _table.isConsonant(word[indexOfV + 1]))
                {
                    word = word.Remove(indexOfV, 1).Insert(indexOfV, "u");
                }
                else if (_table.isConsonant(word[indexOfV - 1]) && !_table.isConsonant(word[indexOfV + 1]))
                {
                    word = word.Remove(indexOfV, 1).Insert(indexOfV, "w");
                }

                indexOfV = word.IndexOf('в', indexOfV + 1);
            }

            var indexOfJ = word.IndexOf('й');

            while (indexOfJ != -1)
            {
                if (indexOfJ == 0)
                {
                    word = word.Remove(indexOfJ, 1).Insert(indexOfJ, "j");
                    indexOfJ = word.IndexOf('й', indexOfJ + 1);

                    continue;
                }

                if (indexOfJ == word.Length - 1 || indexOfJ == word.Length)
                {
                    word = word.Remove(indexOfJ, 1).Insert(indexOfJ, "j");
                    break;
                }


                if (!_table.isConsonant(word[indexOfJ - 1]) && _table.isConsonant(word[indexOfJ + 1]))
                {
                    word = word.Remove(indexOfJ, 1).Insert(indexOfJ, "Y");
                }

                indexOfJ = word.IndexOf('й', indexOfJ + 1);
            }

            word = word.Replace("дж", "d");


            word = ReplaceNextNonStableChar("'", word); // Replace vowel after apos           

            if (Settings.Language == Languages.Ancient)
            {
                word = ReplaceAncientSymbols(word);
            }
            else
            {
                word = ReplaceNextNonStableChar("ъ", word); // Replace vowel after solid sign
            }

            return word;
        }

        private string ProcessDz(string word)
        {
            var indexOfDz = word.IndexOf("дз");

            while (indexOfDz != -1)
            {
                if (HasPredefinedPreffix(word, indexOfDz))
                {
                    indexOfDz = word.IndexOf("дз", indexOfDz + 1);
                }
                else
                {
                    word = word.Remove(indexOfDz, 2).Insert(indexOfDz, "z");
                    indexOfDz = word.IndexOf("дз", indexOfDz + 1);
                }
            }

            return word;
        }

        private bool HasPredefinedPreffix(string word, int indexOfSound)
        {
            if (indexOfSound > 1 && this.dzPrefixes.Any(p => p == word.Substring(indexOfSound - 2, indexOfSound + 1)))
                return true;

            return false;
        }

        private string ReplaceAncientSymbols(string word)
        {
            return new StringBuilder(word)
                .Replace("ъ", "s")
                .Replace("ь", "m")
                .ToString();
        }

        private string ReplaceNextNonStableChar(string symbol, string word)
        {
            var indexOfAp = word.IndexOf(symbol);

            while (indexOfAp != -1)
            {
                var nextCharIndex = indexOfAp + 1;
                if (nextCharIndex > word.Length - 1)
                {
                    break;
                }

                var nextChar = word[nextCharIndex];

                if (nextChar == 'я')
                {
                    word = word.Remove(nextCharIndex, 1);
                    word = word.Insert(nextCharIndex, "jа");
                }
                if (nextChar == 'ю')
                {
                    word = word.Remove(nextCharIndex, 1);
                    word = word.Insert(nextCharIndex, "jу");
                }
                if (nextChar == 'є')
                {
                    word = word.Remove(nextCharIndex, 1);
                    word = word.Insert(nextCharIndex, "jе");
                }
                if (nextChar == 'ї')
                {
                    word = word.Remove(nextCharIndex, 1);
                    word = word.Insert(nextCharIndex, "jі");
                }

                indexOfAp = word.IndexOf(symbol, indexOfAp + 1);
            }

            return word;
        }

        public string RemoveTechnicalCharacters(string word)
        {
            return new StringBuilder(word)
                .Replace('w', 'в')
                .Replace('u', 'в')
                .Replace('j', 'й')
                .Replace('Y', 'й')
                .Replace("d", "дж")
                .Replace("z", "дз")
                .Replace("s", "ъ")
                .Replace("m", "ь")
                .ToString();
        }
    }
}
