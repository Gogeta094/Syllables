using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sklady.TextProcessors
{
    public class RussianPhoneticProcessor : PhoneticProcessorBase
    {
        public override string Process(string input)
        {
            var res = ProcessTwoSoundingLetters(input);
            res = HandleRussianU(res);

            return res;
        }

        private string ProcessTwoSoundingLetters(string input)
        {
            input = ReplacePhoneticCharacter('ю', "jу", input);
            input = ReplacePhoneticCharacter('я', "jа", input);        
            input = ReplacePhoneticCharacter('щ', "шч", input);     
            input = ReplacePhoneticCharacter('ё', "jе", input);
            input = HandleRussianU(input);            

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
    }
}
