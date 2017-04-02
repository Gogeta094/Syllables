using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sklady
{
    public enum Table
    {
        Table1,
        Table2
    }

    class Settings
    {
        public static bool SeparateAfterFirst = true;
        public static string SyllableSeparator = "-";
        public static string Text = String.Empty;
        public static Table CharactersTable = Table.Table1;

        public static bool SaveOnlyFirstSyllable = false;
        public static bool SaveAsCVV = false;

        public static Action<List<AnalyzeResults>> OnResultChanged;

        private static List<AnalyzeResults> _analyzeResult;
        public static List<AnalyzeResults> AnalyzeResults
        {
            get
            {
                return _analyzeResult;
            }
            set
            {
                OnResultChanged(value);
                _analyzeResult = value;
            }
        }

        private static List<AnalyzeResults> _analyzedCvvResults;
        public static List<AnalyzeResults> AnalyzedCvvResults
        {
            get
            {
                return _analyzedCvvResults;
            }
            set
            {
                OnResultChanged(value);
                _analyzedCvvResults = value;
            }
        }


        public static List<string> CharactersToRemove = new List<string>
            {
                "!",
                ".",
                ",",
                "?",
                "/",
                "\"",
                "\\",
                "”",
                "“",
                "’",
                //"'",
                "{",
                "}",
                "[",
                "]",
                "(",
                ")",
                "<",
                ">",
                ";",
                ":",
                "~",
                "`",
                "|",
                "*",
                "@",
                "#",
                "$",
                "%",
                "^",
                "&",
                "+",
                "=",
                "—",
                "_",
                "…",
                "«",
                "»",
                "̑",
                "҂҃",
                "҃",
                "҂"               

            };

    }
}
