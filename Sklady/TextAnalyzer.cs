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

        public event Action<int, int> OnWordAnalyzed;
        public event Action<Exception, string> OnErrorOccured;

        public TextAnalyzer(string text)
        {
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
                    var syllables = _wordAnalyzer.GetSyllables(_words[i]).ToArray();
                    ResultCVV.Add(new AnalyzeResults()
                    {
                        Word = _words[i],
                        Syllables = RemoveApos(syllables)
                    }); 

                    syllables = syllables.Select(c => RemoveTechnicalCharacters(c)).ToArray();
                    syllables = ProcessApos(syllables);
                    
                    result.Add(new AnalyzeResults()
                    {
                        Word = _words[i],
                        Syllables = syllables
                    });

                    if (OnWordAnalyzed != null)
                    {
                        OnWordAnalyzed(i, _words.Length - 1);
                    }
                }
                catch (Exception ex)
                {
                    if (OnErrorOccured != null)
                    {
                        OnErrorOccured(ex, _words[i]);
                    }
                }
            }

            return result;
        }

        private string[] ProcessApos(string[] syllabeles)
        {
            for (var i = 0; i < syllabeles.Length - 1; i++)
            {
                if (syllabeles[i].EndsWith("'") || syllabeles[i].EndsWith("ъ"))
                {
                    if (syllabeles[i + 1].StartsWith("йа"))
                    {
                        syllabeles[i + 1] = syllabeles[i + 1].Replace("йа", "я");
                    }
                    if (syllabeles[i + 1].StartsWith("йу"))
                    {
                        syllabeles[i + 1] = syllabeles[i + 1].Replace("йа", "ю");
                    }
                    if (syllabeles[i + 1].StartsWith("йі"))
                    {
                        syllabeles[i + 1] = syllabeles[i + 1].Replace("йа", "ї");
                    }
                    if (syllabeles[i + 1].StartsWith("йе"))
                    {
                        syllabeles[i + 1] = syllabeles[i + 1].Replace("йа", "є");
                    }
                }
            }

            syllabeles = RemoveApos(syllabeles);            

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
