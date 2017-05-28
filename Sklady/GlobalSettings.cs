using Core.Models;
using System.Collections.Generic;

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

        public static List<char> CharsToSkip = new List<char>()
        {

        };
    }
}
