using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sklady;
using Sklady.Export;
using Core.Models;
using Sklady.Models;

namespace Tests
{
    [TestClass]
    public class TextProcessors
    {            

        [TestMethod]
        public void TestPhonetics()
        {
            var settings = new Settings();
            var export = new ResultsExporter(settings);
            var analyzer = new TextAnalyzer("вирісши", "", settings, export);

            var res = analyzer.GetResults();

            Assert.AreEqual("ви-рі-ши", export.GetSyllables(res.ReadableResults));
        }
       
    }
}
