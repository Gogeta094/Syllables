using Sklady.Export;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sklady.Models
{
    public class FileProcessingResult
    {
        public FileProcessingResult()
        {
            ReadableResults = new List<AnalyzeResults>();
            CvvResults = new List<AnalyzeResults>();            
        }

        public List<AnalyzeResults> ReadableResults { get; set; }

        public List<AnalyzeResults> CvvResults { get; set; }

        public int TextLength
        {
            get
            {
                return ReadableResults.Sum(r => r.Word.Length);
            }
        }

        public int SyllablesCount
        {
            get
            {
                return ReadableResults.Sum(r => r.Syllables.Length);
            }
        }

        public Dictionary<string, int> CvvStatistics
        {
            get
            {
                return GetCvvStatistics();
            }
        }

        private Dictionary<string, int> GetCvvStatistics()
        {
            var res = new Dictionary<string, int>();
            var exporter = ResultsExporter.Instance;

            var exportedCvv = exporter.ConvertToCvv(this.CvvResults);

            foreach(var cvvResult in exportedCvv)
            {
                for (var i = 0; i < cvvResult.Syllables.Length; i++)
                {                    
                    if (!res.ContainsKey(cvvResult.Syllables[i]))
                    {
                        res.Add(cvvResult.Syllables[i], 1);
                    }
                    else
                    {
                        res[cvvResult.Syllables[i]]++;
                    }
                }
            }

            return res;
        }
    }
}
