﻿using System;
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

        public override string ProcessNonStableCharacters(string word, bool isPhoneticsMode = true)
        {
            var res =  base.ProcessNonStableCharacters(word);
            res = Regex.Replace(res, "в", "v");

            if (isPhoneticsMode)
            {
                res = Regex.Replace(res, "ь|ъ", "");
                res = Regex.Replace(res, "э", "е");
                res = Regex.Replace(res, "ё", "о");
            }

            return res;
        }

        private string ProcessTwoSoundingLetters(string input)
        {
            input = ReplacePhoneticCharacter('ю', "jу", input);
            input = ReplacePhoneticCharacter('я', "jа", input);
            input = ReplacePhoneticCharacter('е', "jэ", input);
            input = ReplacePhoneticCharacter('ё', "jо", input);
            input = Regex.Replace(input, "щ", "шч");
            input = HandleRussianU(input);
            input = ReplaceNextNonStableChar("ъ", input); // Replace vowel after solid sign

            return input;
        }

        private string ReductionReplacements(string res)
        {
            res = Regex.Replace(res, "стьд", "зд");
            res = Regex.Replace(res, "вств", "ств");
            res = Regex.Replace(res, "дств", "цтв");
            res = Regex.Replace(res, "тств", "цтв");
            res = Regex.Replace(res, "дск", "цк");
            res = Regex.Replace(res, "здч", "шч");
            res = Regex.Replace(res, "здц", "зц");
            res = Regex.Replace(res, "лнц", "нц");
            res = Regex.Replace(res, "ндс", "нс");
            res = Regex.Replace(res, "нтс", "нц");
            res = Regex.Replace(res, "нтц", "нц");
            res = Regex.Replace(res, "рдч", "рц");
            res = Regex.Replace(res, "рдц", "рц");            
            res = Regex.Replace(res, "зсс", "сс");
            res = Regex.Replace(res, "стл", "сл");
            res = Regex.Replace(res, "стн", "сн");
            res = Regex.Replace(res, "стс", "сс");
            res = Regex.Replace(res, "стч", "шш");
            res = Regex.Replace(res, "стц", "сц");
            res = Regex.Replace(res, "тск", "цк");            

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
