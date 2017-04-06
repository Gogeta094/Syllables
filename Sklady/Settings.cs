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
        public static Table CharactersTable = Table.Table1;

        public static string LastOpenFolderPath = "";
        public static string LastSaveFolderPath = "";

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
