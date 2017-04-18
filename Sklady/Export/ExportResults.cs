﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sklady.Export
{
    public class ExportResults : IResultsExport
    {
        private static ExportResults _instance;

        private CharactersTable _charsTable = CharactersTable.Instance;
        private ExportResults() {}

        public static ExportResults Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ExportResults();

                return _instance;
            }
        }
        public string GetFirstSyllables(List<AnalyzeResults> result)
        {          
            var sb = new StringBuilder();
            var res = result.Select(r => r).ToList();

            for (var i = 0; i < res.Count; i++)
            {
                sb.Append(String.Join(Settings.SyllableSeparator, res[i].Syllables) + " ");
            }

            return sb.ToString();
        }

        public string GetSyllables(List<AnalyzeResults> result)
        {
            var sb = new StringBuilder();
            var res = result.Select(r => r).ToList();

            res = TakeOnlyFirstSyllable(result);

            for (var i = 0; i < result.Count; i++)
            {
                sb.Append(String.Join(Settings.SyllableSeparator, result[i].Syllables) + " ");
            }

            return sb.ToString();
        }

        public string GetSyllablesCVV(List<AnalyzeResults> result)
        {
            var sb = new StringBuilder();
            var res = result.Select(r => new AnalyzeResults() { Syllables = r.Syllables.Select(s => (string)s.Clone()).ToArray() }).ToList();

            res = ConvertToCvv(res);

            for (var i = 0; i < res.Count; i++)
            {
                sb.Append(String.Join(Settings.SyllableSeparator, res[i].Syllables) + " ");
            }

            return sb.ToString();
        }

        public string GetSyllablesFirstCVV(List<AnalyzeResults> result)
        {
            var sb = new StringBuilder();
            var res = result.Select(r => new AnalyzeResults() { Syllables = r.Syllables.Select(s => (string)s.Clone()).ToArray() }).ToList();

            res = TakeOnlyFirstSyllable(res);
            res = ConvertToCvv(res);

            for (var i = 0; i < res.Count; i++)
            {
                sb.Append(String.Join(Settings.SyllableSeparator, res[i].Syllables) + " ");
            }

            return sb.ToString();
        }

        private List<AnalyzeResults> TakeOnlyFirstSyllable(List<AnalyzeResults> anResults)
        {
            return anResults.Select(c => new AnalyzeResults()
            {
                Word = c.Word,
                Syllables = new string[] { c.Syllables.First() }
            }).ToList();
        }

        public List<AnalyzeResults> ConvertToCvv(List<AnalyzeResults> anResults)
        {
            foreach (var resultitem in anResults)
            {
                for (var i = 0; i < resultitem.Syllables.Length; i++)
                {
                    var list = resultitem.Syllables[i].ToList();
                    list.RemoveAll(c => _charsTable.GetPower(c) == 0);
                    resultitem.Syllables[i] = new string(list.ToArray());
                    resultitem.Syllables[i] = new string(resultitem.Syllables[i].Select(s => _charsTable.isConsonant(s) ? 'c' : 'v').ToArray());
                }
            }

            return anResults;
        }
    }
}
