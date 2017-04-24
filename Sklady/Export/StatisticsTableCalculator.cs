using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sklady.Export
{
    public class StatisticsTableCalculator
    {        
        public List<int> GetWeightedAvg(List<List<int>> inputData)
        {
            var res = new List<int>();

            var temporary = new int[inputData[0].Count, inputData.Count];

            for (var i = 0; i < inputData.Count; i++)
            {
                for (var j = 0; j < inputData[i].Count; j++)
                {
                    temporary[j, i] = inputData[i][j];
                }
            }            

            return res;
        }
    }
}
