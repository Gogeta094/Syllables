using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sklady.TextProcessors
{
    public class PhoneticProcessor
    {
        private CharactersTable _table = CharactersTable.Instance;

        public string Process(string input)
        {
            var res = ProcessTwoSoundingLetters(input);            

            return res;
        }        

        public string Unprocess(string input)
        {
            var res = RemoveTechnicalCharacters(input);
            res = UnprocessTwoSoundingLetters(res);

            return res;
        }

        private string UnprocessTwoSoundingLetters(string input)
        {
            var res = input.Replace("йу", "ю")
                        .Replace("йа", "я")
                        .Replace("йі", "ї");

            if (Settings.Language == Languages.Ukraine)
            {
                res = res.Replace("йе", "є")
                         .Replace("шч", "щ");
            }
            else if (Settings.Language == Languages.Russian)
            {
                res = res.Replace("йе", "ё");
            }

            return res;
        }

        private string ProcessTwoSoundingLetters(string input)
        {
            input = ReplacePhoneticCharacter('ю', "йу", input);
            input = ReplacePhoneticCharacter('я', "йа", input);
            input = ReplacePhoneticCharacter('є', "йе", input);
            input = ReplacePhoneticCharacter('ї', "йі", input);
            input = ReplacePhoneticCharacter('щ', "шч", input);
            input = ReplacePhoneticCharacter('ё', "йе", input);

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
                    indexOfJ = word.IndexOf('й', indexOfJ + 1);
                    continue;
                }

                if (indexOfJ == word.Length - 1 || indexOfJ == word.Length)
                {
                    break;
                }


                if (!_table.isConsonant(word[indexOfJ - 1]) && _table.isConsonant(word[indexOfJ + 1]))
                {
                    word = word.Remove(indexOfJ, 1).Insert(indexOfJ, "j");
                }

                indexOfJ = word.IndexOf('й', indexOfJ + 1);
            }

            word = word.Replace("дж", "d").Replace("дз", "z");

            word = ReplaceNextNonStableChar("'", word); // Replace vowel after apos
            word = ReplaceNextNonStableChar("ъ", word); // Replace vowel after solid sign

            return word;
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
                    word = word.Insert(nextCharIndex, "йа");
                }
                if (nextChar == 'ю')
                {
                    word = word.Remove(nextCharIndex, 1);
                    word = word.Insert(nextCharIndex, "йу");
                }
                if (nextChar == 'є')
                {
                    word = word.Remove(nextCharIndex, 1);
                    word = word.Insert(nextCharIndex, "йе");
                }
                if (nextChar == 'ї')
                {
                    word = word.Remove(nextCharIndex, 1);
                    word = word.Insert(nextCharIndex, "йі");
                }

                indexOfAp = word.IndexOf(symbol, indexOfAp + 1);
            }

            return word;
        }

        private string RemoveTechnicalCharacters(string word)
        {
            return new StringBuilder(word)
                .Replace('w', 'в')
                .Replace('u', 'в')
                .Replace('j', 'й')
                .Replace("d", "дж")
                .Replace("z", "дз")
                .ToString();
        }
    }
}
