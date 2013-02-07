/*
 * 
 * 
 * 
 * derived from: https://github.com/KevM/tikaondotnet/blob/master/TikaOnDotnet/tikadriver_examples.cs
 * license: 
 * 
 */

using System;
using NUnit.Framework;
using SpecUnit;

namespace Xpohama.Tikeria.Tests.TikaOnDotNet {
    [TestFixture]
    public class TikaDriverTest {
        private TextExtractor _cut;

        string TestDir = @"..\..\..\TestData\";
        [SetUp]
        public virtual void SetUp() {
            _cut = new TextExtractor();
        }


        [Test]
        public void ShouldExtractContainedFilenamesFromZips() {
            var textExtractionResult = _cut.Extract(TestDir + "tika.zip");

            textExtractionResult.Text.ShouldContain("Tika.docx");
            textExtractionResult.Text.ShouldContain("Tika.pptx");
            textExtractionResult.Text.ShouldContain("tika.xlsx");
        }

        [Test]
        public void ShouldExtractFromJpg() {
            var textExtractionResult = _cut.Extract(TestDir + "apache.jpg");

            textExtractionResult.Text.Trim().ShouldBeEmpty();

            textExtractionResult.Metadata["Software"].ShouldContain("Paint.NET");

            //Console.WriteLine(textExtractionResult);
        }

        [Test]
        public void ShouldExtractFromRtf() {
            var textExtractionResult = _cut.Extract(TestDir + "Tika.rtf");

            textExtractionResult.Text.ShouldContain("pack of pickled almonds");
        }

        [Test]
        public void ShouldExtractFromPdf() {
            var textExtractionResult = _cut.Extract(TestDir + "Tika.pdf");

            textExtractionResult.Text.ShouldContain("pack of pickled almonds");
        }

        [Test]
        public void ShouldExtractFromDocx() {
            var textExtractionResult = _cut.Extract(TestDir + "Tika.docx");

            textExtractionResult.Text.ShouldContain("formatted in interesting ways");
        }

        [Test]
        public void ShouldExtractFromOdt() {
            var textExtractionResult = _cut.Extract(TestDir + "Tika.odt");

            textExtractionResult.Text.ShouldContain("formatted in interesting ways");
        }

        [Test]
        public void ShouldExtractFromPptx() {
            var textExtractionResult = _cut.Extract(TestDir + "Tika.pptx");

            textExtractionResult.Text.ShouldContain("Tika Test Presentation");
        }

        [Test]
        public void ShouldExtractFromXlsx() {
            var textExtractionResult = _cut.Extract(TestDir + "Tika.xlsx");

            textExtractionResult.Text.ShouldContain("Use the force duke");
        }

        [Test]
        public void ShouldExtractFromDoc() {
            var textExtractionResult = _cut.Extract(TestDir + "Tika.doc");

            textExtractionResult.Text.ShouldContain("formatted in interesting ways");
        }

        [Test]
        public void ShouldExtractFromPpt() {
            var textExtractionResult = _cut.Extract(TestDir + "Tika.ppt");

            textExtractionResult.Text.ShouldContain("This document is used for testing");
        }

        [Test]
        public void ShouldExtractFromXls() {
            var textExtractionResult = _cut.Extract(TestDir + "Tika.xls");

            textExtractionResult.Text.ShouldContain("Use the force duke");
        }
    }
}