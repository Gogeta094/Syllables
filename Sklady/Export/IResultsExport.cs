using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sklady.Export
{
    public interface IResultsExport
    {
        string GetSyllables(List<AnalyzeResults> result);
        string GetFirstSyllables(List<AnalyzeResults> result);
        string GetSyllablesCVV(List<AnalyzeResults> result);
        string GetSyllablesFirstCVV(List<AnalyzeResults> result);
    }
}
