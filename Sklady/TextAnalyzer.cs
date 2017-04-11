using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sklady
{
    public class TextAnalyzer
    {
        private string _text;
        private string[] _words;       
        private WordAnalyzer _wordAnalyzer;
        private CharactersTable table = CharactersTable.Instance;

        public List<AnalyzeResults> ResultCVV { get; private set; }

        public string FileName { get; private set; }

        public event Action<int, int, string> OnWordAnalyzed;
        public event Action<Exception, string, string> OnErrorOccured;

        public TextAnalyzer(string text, string fileName)
        {
            FileName = fileName;
            _wordAnalyzer = new WordAnalyzer();
            ResultCVV = new List<AnalyzeResults>();
            PrepareText(text);            
        }

        private void PrepareText(string inputText)
        {
            //_text = Regex.Replace(inputText.ToLower(), @"\.|\!|\?|\,|\(|\)|\[|\]|\*|\'|\«|\»|[a-zA-Z]|[0-9]|\:|\;|\—|\""|\<|\>|\=|\+|\/|\\|\{|\}|\#|\@|\||_", "", RegexOptions.Multiline | RegexOptions.IgnoreCase); // remove special characters                 
            var sb = new StringBuilder(inputText.ToLower());
            for (var i = 0; i < Settings.CharactersToRemove.Count; i++) // Remove all unnecesarry characters
            {
                sb.Replace(Settings.CharactersToRemove[i], "");
            }
            _text = sb.ToString();
            _text = Regex.Replace(_text, "([0-9][а-яА-Я])", "");//Remove chapter number (for vk)
            _text = Regex.Replace(_text, "[a-zA-Z]|[0-9]", "");
            _text = Regex.Replace(_text, @"/\t|\n|\r", " "); // remove new line, tabulation and other literals
            //_text = _text.Replace("ъ", "");

            _text = Regex.Replace(_text, @"(\- )", ""); // Handle word hyphenations
            _text = Regex.Replace(_text, @"\-", "");
            _text = Regex.Replace(_text, @"и́", "й");

            _words = _text.Split(new[] { " ", " " }, StringSplitOptions.RemoveEmptyEntries).ToArray(); // Split text by words
        }

        public List<AnalyzeResults> GetResults()
        {
            var result = new List<AnalyzeResults>();

            for (var i = 0; i < _words.Length; i++)
            {
                _words[i] = AnalyzeNonStableCharacters(_words[i]); // Replace some chars according to their power
            }

            for (var i = 0; i < _words.Length; i++)
            {
                try
                {
                    _words[i] = PhoneticReplace(_words[i]);
                    var syllables = _wordAnalyzer.GetSyllables(_words[i]).ToArray();                    

                    UpdateCVVResultSet(syllables, _words[i]);
                    UpdateReadableViewSet(syllables, _words[i], result);

                    OnWordAnalyzed?.Invoke(i, _words.Length - 1, FileName);
                }
                catch (Exception ex)
                {
                    OnErrorOccured?.Invoke(ex, _words[i], FileName);
                }
            }

            return result;
        }

        private void UpdateCVVResultSet(string[] syllables, string word)
        {     
            ResultCVV.Add(new AnalyzeResults()
            {
                Word = word,
                Syllables = RemoveApos(syllables)
            });
        }

        private void UpdateReadableViewSet(string[] syllables, string word, List<AnalyzeResults> result)
        {
            syllables = syllables.Select(c => RemoveTechnicalCharacters(c)).ToArray();
            syllables = ReplacePhonetics(syllables);          

            result.Add(new AnalyzeResults()
            {
                Word = word,
                Syllables = syllables
            });
        }

        private string[] ReplacePhonetics(string[] syllabeles)
        {
            for (var i = 0; i < syllabeles.Length - 1; i++)
            {       
                syllabeles[i] = syllabeles[i].Replace("йа", "я");
                syllabeles[i] = syllabeles[i].Replace("йу", "ю");
                syllabeles[i] = syllabeles[i].Replace("йі", "ї");
                syllabeles[i] = syllabeles[i].Replace("йе", "є");
            }           

            return syllabeles;
        }

        private string[] RemoveApos(string[] input)
        {
            var result = new string[input.Length];
            for (var i = 0; i < input.Length; i++)
            {
                result[i] = input[i].Replace("'", "");
            }

            return result;
        }

        private string PhoneticReplace(string word)
        {
            word = ReplacePhoneticCharacter('ю', "йу", word);
            word = ReplacePhoneticCharacter('я', "йа", word);
            word = ReplacePhoneticCharacter('є', "йе", word);
            word = ReplacePhoneticCharacter('ї', "йі", word);

            return word;
        }

        private string ReplacePhoneticCharacter(char charToReplace, string replacementText, string word)
        {
            var indexofChar = word.IndexOf(charToReplace);            

            while (indexofChar != -1)
            {
                if (indexofChar == 0 || !table.isConsonant(word[indexofChar - 1]))
                {
                    word = word.Remove(indexofChar, 1).Insert(indexofChar, replacementText);
                }
                indexofChar = word.IndexOf(charToReplace, indexofChar + 1);
            }

            return word;
        }

        private string AnalyzeNonStableCharacters(string word)
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

                if (!table.isConsonant(word[indexOfV - 1]) && table.isConsonant(word[indexOfV + 1]))
                {
                    word = word.Remove(indexOfV, 1).Insert(indexOfV, "u");
                }
                else if (table.isConsonant(word[indexOfV - 1]) && !table.isConsonant(word[indexOfV + 1]))
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


                if (!table.isConsonant(word[indexOfJ - 1]) && table.isConsonant(word[indexOfJ + 1]))
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
                //.Replace("\'йа", "я")
                //.Replace("\'йу", "ю")
                //.Replace("\'йе", "є")
                //.Replace("\'йі", "ї")
                .ToString();
        }
    }
}
