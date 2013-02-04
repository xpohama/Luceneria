using NUnit.Framework;
using System.IO;
using SpecUnit; 

namespace Xpohama.Tikeria.Tests {
    [TestFixture]
    public class TikaExtractTextTest {
        BodyTextExtractor _cut { get; set; }
        string TestDir = @"..\..\..\TestData\";
        [SetUp]
        public virtual void SetUp () {
            _cut = new BodyTextExtractor();
        }

        byte[] FromFile(string path) {

            return File.ReadAllBytes(path);
        }
        [Test]
        public void ShouldExtractFromRtf () {
            var textExtractionResult = _cut.Extract(TestDir + "Tika.rtf");

            textExtractionResult.ShouldContain("pack of pickled almonds");
        }

       
    }
}