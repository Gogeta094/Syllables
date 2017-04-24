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

        public event Action<int, int> OnFileCvvItemCalculated;

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

            var cvvHeader = GenerateTableHeader(sb, fileProcessingResults);

            var statisticsInfo = new List<List<int>>();

            foreach (var fileResult in fileProcessingResults)
            {
                var cvvStatistics = GetCvvStatistics(fileResult, cvvHeader, fileProcessingResults.Count);
                cvvStatistics.Insert(0, fileResult.SyllablesCount);
                cvvStatistics.Insert(0, fileResult.TextLength);

                var statisticsString = String.Join(",", cvvStatistics);
                sb.AppendLine(String.Format("{0},{1}", fileResult.FileName, cvvStatistics));

                statisticsInfo.Add(cvvStatistics);
            }           

            return sb.ToString();
        }

       
        private List<int> GetCvvStatistics(FileProcessingResult fileResult, SortedSet<string> cvvHeader, int filesCount)
        {  
            var cvvCounts = new List<int>();

            foreach (var header in cvvHeader)
            {
                if (fileResult.CvvStatistics.ContainsKey(header))
                {
                    cvvCounts.Add(fileResult.CvvStatistics[header]);
                }
                else
                {
                    cvvCounts.Add(0);
                }

                if (OnFileCvvItemCalculated != null)
                    OnFileCvvItemCalculated(cvvHeader.Count, filesCount);
            }

            return cvvCounts;            
        }

        private SortedSet<string> GenerateTableHeader(StringBuilder sb, List<FileProcessingResult> fileProcessingResults)
        {
            var cvvSet = new SortedSet<string>();

            foreach (var item in fileProcessingResults)
            {
                cvvSet.UnionWith(item.CvvStatistics.Select(c => c.Key));
            }

            var cvvHeader = String.Join(",", cvvSet);

            sb.AppendLine(String.Format("{0},{1},{2},{3}", "Text", "Length", "SyllablesCount", cvvHeader));

            return cvvSet;
        }
    }
}
