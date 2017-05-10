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
        public void TestJReplacements()
        {
            var settings = new Settings();
            var export = new ResultsExporter(settings);

            var analyzer = new TextAnalyzer("туристський", "", settings, export);
            var res = analyzer.GetResults();

            Assert.AreEqual("ту-ри-скиj", export.GetSyllables(res.ReadableResults));

            analyzer = new TextAnalyzer("студентський", "", settings, export);
            res = analyzer.GetResults();
            Assert.AreEqual("сту-ден-скиj", export.GetSyllables(res.ReadableResults));

            analyzer = new TextAnalyzer("йод", "", settings, export);
            res = analyzer.GetResults();
            Assert.AreEqual("jод", export.GetSyllables(res.ReadableResults));

            analyzer = new TextAnalyzer("юра", "", settings, export);
            res = analyzer.GetResults();
            Assert.AreEqual("jу-ра", export.GetSyllables(res.ReadableResults));

            analyzer = new TextAnalyzer("майор", "", settings, export);
            res = analyzer.GetResults();
            Assert.AreEqual("ма-jор", export.GetSyllables(res.ReadableResults));

            analyzer = new TextAnalyzer("йду", "", settings, export);
            res = analyzer.GetResults();
            Assert.AreEqual("Yду", export.GetSyllables(res.ReadableResults));

            analyzer = new TextAnalyzer("йти", "", settings, export);
            res = analyzer.GetResults();
            Assert.AreEqual("Yти", export.GetSyllables(res.ReadableResults));

            analyzer = new TextAnalyzer("майдан", "", settings, export);
            res = analyzer.GetResults();
            Assert.AreEqual("маY-дан", export.GetSyllables(res.ReadableResults));
        }

        [TestMethod]
        public void TestVReplacements()
        {
            var settings = new Settings();
            var export = new ResultsExporter(settings);

            var analyzer = new TextAnalyzer("авдіївка", "", settings, export);
            var res = analyzer.GetResults();
            Assert.AreEqual("аu-ді-jіu-ка", export.GetSyllables(res.ReadableResults));

            analyzer = new TextAnalyzer("свалява", "", settings, export);
            res = analyzer.GetResults();
            Assert.AreEqual("сwа-ля-ва", export.GetSyllables(res.ReadableResults));
        }

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
