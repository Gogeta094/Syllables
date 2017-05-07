using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sklady.TextProcessors
{
    public class AncientPhoneticProcessor : PhoneticProcessorBase
    {
        public override string Process(string input)
        {
            var res = HandleI(input);
            res = ReplaceAncientSymbols(res);

            return res;
        }

        private string ReplaceAncientSymbols(string word)
        {
            return new StringBuilder(word)
                .Replace("ъ", "s")
                .Replace("ь", "m")
                .ToString();
        }

        private string HandleI(string input)
        {
            var res = Regex.Replace(input, "іо", "jо");
            res = Regex.Replace(res, "іе", "jе");
            res = Regex.Replace(res, "іа", "jа");
            res = Regex.Replace(res, "іу", "jу");
            res = Regex.Replace(res, "іі", "jі");
            res = Regex.Replace(res, "я", "jа");
            res = Regex.Replace(res, "ьі", "b");
            res = Regex.Replace(res, "оу", "Ü");

            return res;
        }

        public override string RemoveTechnicalCharacters(string word)
        {
            var res = base.RemoveTechnicalCharacters(word);

            res.Replace("Ü", "оу")
                .Replace("s", "ъ")
                .Replace("m", "ь")
                .Replace("b", "ьі");

            return res;
        }
    }
}
