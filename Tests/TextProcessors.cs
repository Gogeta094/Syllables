using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sklady;
using Sklady.Export;

namespace Tests
{
    [TestClass]
    public class TextProcessors
    {
        [TestMethod]
        public void TestPhonetics()
        {
            var export = ResultsExporter.Instance;
            var analyzer = new TextAnalyzer("вирісши", "");

            var res = analyzer.GetResults();

            Assert.AreEqual("ви-рі-ши", export.GetSyllables(res.ReadableResults));
        }

        [TestMethod]
        public void TestPhonetics2()
        {
            var export = ResultsExporter.Instance;
            var analyzer = new TextAnalyzer("вирісши", "");

            var res = analyzer.GetResults();

            Assert.AreEqual("ви-рі-ши", export.GetSyllables(res.ReadableResults));
        }
    }
}
