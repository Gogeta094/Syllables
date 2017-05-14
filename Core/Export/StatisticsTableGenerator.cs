using Core.Models;
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
        private List<string> _repetitionsHeaders;

        private StatisticsCalculator _statisticsCalculator;
        private bool _useAbsoluteValues = false;
        private CharactersTable _charactersTable;
        

        public StatisticsTableGenerator(bool useAbsoluteMeasures = false)
        {
            _useAbsoluteValues = useAbsoluteMeasures;
            _charactersTable = new CharactersTable(Table.Table1);
        }

        public string GetTableString(List<FileProcessingResult> results)
        {
            var sb = new StringBuilder();
            ProcessCVVHeaders(results);
            ProcessRepetitionsHeaders(results);

            var headerItems = GenerateTableHeader();
            sb.AppendLine(String.Join(",", headerItems));


            var filesStatistics = new List<List<double>>();

            foreach (var resItem in results)
            {
                var fileStatistics = GenerateStatistics(resItem);
                filesStatistics.Add(fileStatistics);

                sb.AppendLine(String.Format("{0},{1}", resItem.FileName, String.Join(",", fileStatistics)));
            }

            var groupedStatistics = GroupByMeasure(filesStatistics);
            _statisticsCalculator = new StatisticsCalculator(groupedStatistics[0]); // at 0 position we have list of file lengths

            var avg = groupedStatistics.Select(c => String.Format("{0:0.00}", _statisticsCalculator.GetWeightedAvarage(c)));
            sb.AppendLine(String.Format("{0},{1}", "Average", String.Join(",", avg)));

            var weightedDelta = groupedStatistics.Select(c => String.Format("{0:0.00}", _statisticsCalculator.GetWeightedDelta(c)));
            sb.AppendLine(String.Format("{0},{1}", "Avg Square Delta", String.Join(",", weightedDelta)));

            return sb.ToString();
        }       

        private List<List<double>> GroupByMeasure(List<List<double>> fileStatistics)
        {
            var count = fileStatistics.First().Count; // all list should have the same count
            var res = new List<List<double>>();

            for (var i = 0; i < count; i++)
            {
                res.Add(new List<double>());
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

        private List<double> GenerateStatistics(FileProcessingResult fileResult)
        {
            var res = new List<double>();

            var CVVSyllablesStatistics = new List<double>();
            var RepetitionsStatistics = new List<double>();
            var CandVSums = GetCVCounts(fileResult);           

            foreach (var header in _cvvHeaders)
            {
                if (fileResult.CvvStatistics.ContainsKey(header))
                {
                    CVVSyllablesStatistics.Add(fileResult.CvvStatistics[header]);
                }
                else
                {
                    CVVSyllablesStatistics.Add(0);
                }
            }

            foreach (var header in _repetitionsHeaders)
            {
                if (fileResult.Repetitions.ContainsKey(header))
                {
                    RepetitionsStatistics.Add(fileResult.Repetitions[header]);
                }
                else
                {
                    RepetitionsStatistics.Add(0);
                }
            }

            if (!_useAbsoluteValues)
                CVVSyllablesStatistics = CVVSyllablesStatistics.Select(r => (double) r / fileResult.SyllablesCount).ToList();

            res.AddRange(CandVSums);
            res.AddRange(CVVSyllablesStatistics);
            res.AddRange(RepetitionsStatistics);

            res.Insert(0, fileResult.SyllablesCount);
            res.Insert(0, fileResult.TextLength);            

            return res;
        }       

        private List<double> GetCVCounts(FileProcessingResult fileResult)
        {
            var CCount = 0.0;
            var VCount = 0.0;
            var openSyllables = 0.0;
            var closedSyllables = 0.0;

            foreach(var item in fileResult.ReadableResults)
            {
                for (var i = 0; i < item.Syllables.Length; i++)
                {
                    var HalfCharsCount = item.Syllables[i].Count(c => c == 'Y' || c == 'u');

                    CCount += item.Syllables[i].Count(c => _charactersTable.isConsonant(c)) - 0.5 * HalfCharsCount; // as we're counting Y as 0.5V 
                    VCount += item.Syllables[i].Count(c => !_charactersTable.isConsonant(c)) + 0.5 * HalfCharsCount; // and 0.5C we have to make corresponding calculations

                    if (_charactersTable.isConsonant(item.Syllables[i].Last()))
                    {
                        closedSyllables++;
                    }
                    else
                    {
                        openSyllables++;
                    }
                }
            }

            var CtoV = CCount / VCount;
            openSyllables = openSyllables / fileResult.SyllablesCount;
            closedSyllables = closedSyllables / fileResult.SyllablesCount;

            return new List<double>() { CCount, VCount, CtoV, openSyllables, closedSyllables };
        }       

        private List<string> GenerateTableHeader()
        {
            var res = new List<string>();
            res.AddRange(new string[] { "Text", "Length", "SyllablesCount", "Total C", "Total V", "C/V", "Opened", "Closed" });
            res.AddRange(_cvvHeaders);
            res.AddRange(_repetitionsHeaders);

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

        private void ProcessRepetitionsHeaders(List<FileProcessingResult> results)
        {
            var repetitions = new SortedSet<string>();

            foreach (var item in results)
            {
                repetitions.UnionWith(item.Repetitions.Select(c => c.Key));
            }

            _repetitionsHeaders = repetitions.ToList();
        }
    }
}
