using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sklady.TextProcessors
{
    public abstract class PhoneticProcessorBase
    {
        protected CharactersTable CharactersTable = CharactersTable.Instance;

        public abstract string Process(string input);

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

                if (!CharactersTable.isConsonant(word[indexOfV - 1]) && CharactersTable.isConsonant(word[indexOfV + 1]))
                {
                    word = word.Remove(indexOfV, 1).Insert(indexOfV, "u");
                }
                else if (CharactersTable.isConsonant(word[indexOfV - 1]) && !CharactersTable.isConsonant(word[indexOfV + 1]))
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


                if (!CharactersTable.isConsonant(word[indexOfJ - 1]) && CharactersTable.isConsonant(word[indexOfJ + 1]))
                {
                    word = word.Remove(indexOfJ, 1).Insert(indexOfJ, "Y");
                }

                indexOfJ = word.IndexOf('й', indexOfJ + 1);
            }

            word = word.Replace("дж", "d");

            word = ReplaceNextNonStableChar("'", word); // Replace vowel after apos      

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

        protected string ReplacePhoneticCharacter(char charToReplace, string replacementText, string input)
        {
            var indexofChar = input.IndexOf(charToReplace);

            while (indexofChar != -1)
            {
                if (indexofChar == 0 || !CharactersTable.isConsonant(input[indexofChar - 1]))
                {
                    input = input.Remove(indexofChar, 1).Insert(indexofChar, replacementText);
                }
                indexofChar = input.IndexOf(charToReplace, indexofChar + 1);
            }

            return input;
        }
    }    
}
