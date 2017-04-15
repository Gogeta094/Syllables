using Sklady.TextProcessors;
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
        private PhoneticProcessor _phoneticProcessor = new PhoneticProcessor();

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
            _text = Regex.Replace(_text, @"и́| ̀и|ù|ѝ̀̀| ̀̀и|ѝ|́и", "й");

            _words = _text.Split(new[] { " ", " " }, StringSplitOptions.RemoveEmptyEntries).ToArray(); // Split text by words
        }

        public List<AnalyzeResults> GetResults()
        {
            var result = new List<AnalyzeResults>();

            for (var i = 0; i < _words.Length; i++)
            {
                if (Settings.PhoneticsMode)
                    _words[i] = _phoneticProcessor.Process(_words[i]); // In case of phonetics mode make corresponding replacements

                _words[i] = _phoneticProcessor.ProcessNonStableCharacters(_words[i]); // Replace some chars according to their power
            }

            for (var i = 0; i < _words.Length; i++)
            {
                try
                {
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
            syllables = UnprocessPhonetics(syllables);          

            result.Add(new AnalyzeResults()
            {
                Word = word,
                Syllables = syllables
            });
        }

        private string[] UnprocessPhonetics(string[] syllabeles)
        {
            for (var i = 0; i < syllabeles.Length; i++)
            {
                syllabeles[i] = _phoneticProcessor.Unprocess(syllabeles[i]);
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
    }
}
