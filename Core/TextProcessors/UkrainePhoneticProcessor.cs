using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sklady.TextProcessors
{
    public class UkrainePhoneticProcessor : PhoneticProcessorBase
    {
        private string[] dzPrefixes = new string[] { "під", "над", "від" };

        public UkrainePhoneticProcessor(CharactersTable charactersTable) : base(charactersTable)
        {
        }

        public override string Process(string input)
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
            input = Regex.Replace(input, @"([а-яА-Я])\1+", "$1");          

            return input;
        }

        private string ProcessTwoSoundingLetters(string input)
        {
            input = ReplacePhoneticCharacter('ю', "jу", input);
            input = ReplacePhoneticCharacter('я', "jа", input);
            input = ReplacePhoneticCharacter('є', "jе", input);
            input = ReplacePhoneticCharacter('ї', "jі", input);
            input = ReplacePhoneticCharacter('щ', "шч", input);

            return input;
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
    }
}
