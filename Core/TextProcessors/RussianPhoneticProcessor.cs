using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sklady.TextProcessors
{
    public class RussianPhoneticProcessor : PhoneticProcessorBase
    {
        public RussianPhoneticProcessor(CharactersTable charactersTable) : base(charactersTable)
        {
        }

        public override string Process(string input)
        {
            var res = ProcessTwoSoundingLetters(input);            
            res = ReductionReplacements(res);

            return res;
        }

        private string ProcessTwoSoundingLetters(string input)
        {
            input = ReplacePhoneticCharacter('ю', "jу", input);
            input = ReplacePhoneticCharacter('я', "jа", input);        
            input = ReplacePhoneticCharacter('щ', "шч", input);     
            input = ReplacePhoneticCharacter('ё', "jе", input);
            input = HandleRussianU(input);
            input = ReplaceNextNonStableChar("ъ", input); // Replace vowel after solid sign

            return input;
        }
        private string ReductionReplacements(string res)
        {
            res = Regex.Replace(res, "стьд", "зд");
            res = Regex.Replace(res, "нде", "не");
            res = Regex.Replace(res, "зсс", "сс");
            res = Regex.Replace(res, "стл", "сл");
            res = Regex.Replace(res, "стн", "сн");

            return res;
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

        public override string RemoveTechnicalCharacters(string word)
        {
            return word;
        }
    }
}
