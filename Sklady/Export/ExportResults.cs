using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sklady.Export
{
    public class ExportResults
    {
        public List<FileExportResults> FileExportResults { get; set; }
        public string StatisticsTableCsv { get; set; }
    }
}
