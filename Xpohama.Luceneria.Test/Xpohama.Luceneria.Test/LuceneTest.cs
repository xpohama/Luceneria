using NUnit.Framework;
using System.IO;
using System;

namespace Xpohama.Luceneria.Tests {
    [TestFixture]
    public class LuceneTest {

        string TestDir = @"..\..\..\TestData\";
        public Lucene.Net.Store.FSDirectory Directory { get {
            return Lucene.Net.Store.FSDirectory.Open(new DirectoryInfo(TestDir+"Index"));
        }}

        [SetUp]
        public virtual void SetUp() {
            
        }

        Tuple<Guid,byte[]> ReadFile(string path) {
            return Tuple.Create(Guid.NewGuid, File.ReadAllBytes(path));
        }

       
        public void IndexTest (string path) {
            var indexer = new Indexer(this.Directory);
            var file = ReadFile(path);

            var doc = indexer.CreateDocument(file.Item1,file.Item2);
            indexer.Writer.AddDocument(doc);
            indexer.Writer.Commit();

            //TODO: test if doc is indexed
        }

        [Test]
        public void TestPattern() {
            
            // arrange
            // ...

            
            // act
            // ...

            // assert
            // ...

        }

       
    }
}