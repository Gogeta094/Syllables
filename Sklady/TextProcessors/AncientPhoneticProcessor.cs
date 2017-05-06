using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sklady.TextProcessors
{
    public class AncientPhoneticProcessor : PhoneticProcessorBase
    {
        public override string Process(string input)
        {
            throw new NotImplementedException();
        }

        private string ReplaceAncientSymbols(string word)
        {
            return new StringBuilder(word)
                .Replace("ъ", "s")
                .Replace("ь", "m")
                .ToString();
        }
    }
}
