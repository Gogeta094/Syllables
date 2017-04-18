using Sklady.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sklady.Export
{
    public class ResultsExporter
    {
        private static ResultsExporter _instance;

        private CharactersTable _charsTable = CharactersTable.Instance;
        private ResultsExporter() {}

        public static ResultsExporter Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ResultsExporter();

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

        public string GetStatisticsTableCsv(List<FileProcessingResult> fileProcessingResults)
        {
            var sb = new StringBuilder();

            foreach (var fileResult in fileProcessingResults)
            {
                var cvvStatistics = GetCvvStatistics(fileResult);
                sb.AppendLine(String.Format("{0},{1},{2},{3}", fileResult.FileName, fileResult.TextLength, fileResult.SyllablesCount, cvvStatistics));
            }

            return sb.ToString();
        }

        private string GetCvvStatistics(FileProcessingResult fileResult)
        {
            var sb = new StringBuilder();

            foreach (var item in fileResult.CvvStatistics)
            {
                //TODO:Think about dictionary sorting logic
            }

            return sb.ToString();
        }
    }
}
