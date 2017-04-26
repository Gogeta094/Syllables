using Sklady.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sklady.Export
{
    public class StatisticsTableGenerator
    {
        private List<string> _cvvHeaders;
        private StatisticsCalculator _statisticsCalculator;

        public StatisticsTableGenerator()
        {            
        }

        public string GetTableString(List<FileProcessingResult> results)
        {
            var sb = new StringBuilder();
            ProcessCVVHeaders(results);

            var headerItems = GenerateTableHeader();
            sb.AppendLine(String.Join(",", headerItems));


            var filesStatistics = new List<List<int>>();

            foreach (var resItem in results)
            {
                var fileStatistics = GenerateStatistics(resItem);
                filesStatistics.Add(fileStatistics);

                sb.AppendLine(String.Format("{0},{1}", resItem.FileName, String.Join(",", fileStatistics)));
            }

            var groupedStatistics = GroupByMeasure(filesStatistics);
            _statisticsCalculator = new StatisticsCalculator(groupedStatistics[0]); // at 0 position we have list of file lengths

            var avg = groupedStatistics.Select(c => _statisticsCalculator.GetWeightedAvarage(c));
            sb.AppendLine(String.Format("{0},{1}", "Average", String.Join(",", avg)));

            var weightedDelta = groupedStatistics.Select(c => _statisticsCalculator.GetWeightedDelta(c));
            sb.AppendLine(String.Format("{0},{1}", "Avg Square Delta", String.Join(",", weightedDelta)));

            return sb.ToString();
        }

        private List<List<int>> GroupByMeasure(List<List<int>> fileStatistics)
        {
            var count = fileStatistics.First().Count; // all list should have the same count
            var res = new List<List<int>>();

            for (var i = 0; i < count; i++)
            {
                res.Add(new List<int>());
            }

            for (var i = 0; i < count; i++)
            {                
                for (var j = 0; j < fileStatistics.Count; j++)
                {
                    res[i].Add(fileStatistics[j][i]);
                }
            }

            return res;
        }

        private List<int> GenerateStatistics(FileProcessingResult fileResult)
        {
            var res = new List<int>();

            res.Add(fileResult.TextLength);
            res.Add(fileResult.SyllablesCount);

            foreach (var header in _cvvHeaders)
            {
                if (fileResult.CvvStatistics.ContainsKey(header))
                {
                    res.Add(fileResult.CvvStatistics[header]);
                }
                else
                {
                    res.Add(0);
                }
            }

            return res;
        }

        private List<int> GenerateStatisticsSummary(List<int> input)
        {
            var result = new List<int>();


            return result;
        }

        private List<string> GenerateTableHeader()
        {
            var res = new List<string>();
            res.AddRange(new string[] { "Text", "Length", "SyllablesCount" });
            res.AddRange(_cvvHeaders);

            return res;
        }

        private void ProcessCVVHeaders(List<FileProcessingResult> results)
        {
            var cvvSet = new SortedSet<string>();

            foreach (var item in results)
            {
                cvvSet.UnionWith(item.CvvStatistics.Select(c => c.Key));
            }

            _cvvHeaders = cvvSet.ToList();
        }


        //public List<int> GetWeightedAvg(List<List<int>> inputData)
        //{
        //    var res = new List<int>();

        //    var temporary = new int[inputData[0].Count, inputData.Count];

        //    for (var i = 0; i < inputData.Count; i++)
        //    {
        //        for (var j = 0; j < inputData[i].Count; j++)
        //        {
        //            temporary[j, i] = inputData[i][j];
        //        }
        //    }            

        //    return res;
        //}
    }
}
