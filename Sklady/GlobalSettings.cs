using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sklady
{  

    class GlobalSettings
    {
        public static bool SeparateAfterFirst = true;
        public static string SyllableSeparator = "-";
       
        public static bool PhoneticsMode = true;
        public static Languages Language = Languages.Ukraine;

        public static string LastOpenFolderPath = "";
        public static string LastSaveFolderPath = "";

        public static bool AbsoluteMeasures = false;

        public static CharactersTable CharactersTable = new CharactersTable(Table.Table1);

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
                "҂",
                "–",
                "„",
                "№"
            };

    }
}
