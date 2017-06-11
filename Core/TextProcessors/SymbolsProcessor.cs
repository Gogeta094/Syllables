using Sklady.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core.TextProcessors
{
    public class SymbolsProcessor
    {
        private readonly Settings settings;
        private string _text;

        public string FileName { get; private set; }

        public event Action<int, int, string> OnLetterAnalyzed;

        public SymbolsProcessor(Settings settings, string text, string fileName)
        {
            this.settings = settings;
            this.FileName = fileName;
            PrepareText(text);
        }

        private void PrepareText(string inputText)
        {
            var sb = new StringBuilder(inputText.ToLower());

            for (var i = 0; i < settings.CharactersToRemove.Count; i++) // Remove all unnecesarry characters
            {
                sb.Replace(settings.CharactersToRemove[i], "");
            }

            _text = sb.ToString();                        
            _text = Regex.Replace(_text, @"/\t|\n|\r|\s", ""); // remove new line, tabulation and other literals
            
            _text = Regex.Replace(_text, @"(\- )", ""); // Handle word hyphenations                        
        }

        public FileProcessingResult GetResults()
        {
            var res = new FileProcessingResult(null);
            var letters = new Dictionary<char, int>();

            for (var i = 0; i < _text.Length; i++)
            {
                if (letters.ContainsKey(_text[i]))
                {
                    letters[_text[i]]++;
                }
                else
                {
                    letters[_text[i]] = 1;
                }

                if (OnLetterAnalyzed != null)
                {
                    OnLetterAnalyzed(i, _text.Length, this.FileName);
                }
            }

            res.Letters = letters;
            res.FileName = this.FileName;

            return res;
        }
    }
}
