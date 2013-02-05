using NUnit.Framework;
using System.IO;
using System;
using Lucene.Net.Store;
using Lucene.Net.Search;
using Lucene.Net.Index;
using System.Linq;

namespace Xpohama.Luceneria.Tests {
    [TestFixture]
    public class LuceneTest {

        string TestDir = @"..\..\..\TestData\";
        public Lucene.Net.Store.Directory Directory {
            get {
                return new RAMDirectory(); 
                    //FSDirectory.Open(new DirectoryInfo(TestDir + "Index"));
            }
        }

        [SetUp]
        public virtual void SetUp() {
            
        }

        Tuple<Guid,byte[]> ReadFile(string path) {
            return Tuple.Create(Guid.NewGuid(), File.ReadAllBytes(path));
        }


        public void IndexTest (Indexer indexer, string path, string contains) {
            contains = contains.ToLower();

            var file = ReadFile(path);

            var doc = indexer.CreateDocument(file.Item1, file.Item2);
            indexer.Writer.AddDocument(doc);
            indexer.Writer.Commit();
            indexer.Refresh();

            var query = new TermQuery(new Term(indexer.DocumentContentField, contains));
            var topDocs = indexer.Searcher.Search(query, 1000);
            var docs = topDocs.scoreDocs
                .Select(sd => indexer.Searcher.Doc(sd.doc))
                .ToArray();

            Assert.IsTrue(docs.Any(d => d.GetField("ID").StringValue() == file.Item1.ToString()));

        }

        [Test]
        public void IndexerTest () {
            var indexer = new Indexer(this.Directory);

            IndexTest(indexer, TestDir + "Tika.rtf", "almonds");
            IndexTest(indexer, TestDir + "Tika.pdf", "almonds");
            IndexTest(indexer, TestDir + "Tika.docx", "interesting");
            IndexTest(indexer, TestDir + "Tika.odt", "interesting");
            IndexTest(indexer, TestDir + "Tika.pptx", "Presentation");
            IndexTest(indexer, TestDir + "Tika.xlsx", "duke");

            indexer.Close();
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