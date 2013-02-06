using NUnit.Framework;
using System.IO;
using System;
using Lucene.Net.Store;
using Lucene.Net.Search;
using Lucene.Net.Index;
using System.Linq;
using System.Collections.Generic;
using Lucene.Net.Documents;

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
            if (!File.Exists(path))
                throw new ArgumentException(path + " does not exist");
            return Tuple.Create(Guid.NewGuid(), File.ReadAllBytes(path));
        }


        public Guid IndexTest (Indexer indexer, string path, string contains) {
            contains = contains.ToLower();

            var file = ReadFile(path);

            var doc = indexer.CreateDocument(file.Item2);
            doc.Add(new Field("Id", file.Item1.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            indexer.Writer.AddDocument(doc);
            indexer.Writer.Commit();
            indexer.Refresh();

            var query = new TermQuery(new Term(indexer.DocumentContentField, contains));
            var topDocs = indexer.Searcher.Search(query, 1000);
            //topDocs = indexer.Searcher.Search(query, topDocs.totalHits);
            var docs = topDocs.ScoreDocs
                .Select(sd => indexer.Searcher.Doc(sd.Doc))
                .ToArray();

            Assert.IsTrue(docs.Any(d => d.GetField("Id").StringValue == file.Item1.ToString()));
            return file.Item1;
        }

        [Test]
        public void IndexerTest () {
            using (var indexer = new Indexer(this.Directory)) {

                var guids = new List<Guid>();
                guids.Add(IndexTest(indexer, TestDir + "Tika.rtf", "almonds"));
                guids.Add(IndexTest(indexer, TestDir + "Tika.pdf", "almonds"));
                guids.Add(IndexTest(indexer, TestDir + "Tika.docx", "almonds"));
                guids.Add(IndexTest(indexer, TestDir + "Tika.odt", "almonds"));
                guids.Add(IndexTest(indexer, TestDir + "Tika.pptx", "almonds"));
                guids.Add(IndexTest(indexer, TestDir + "Tika.xlsx", "almonds"));

                foreach (var guid in guids) {
                    indexer.Writer.DeleteDocuments(new Term("Id", guid.ToString()));
                    indexer.Refresh();
                    var docs = indexer.Searcher
                        .Search(new TermQuery(new Term(indexer.DocumentContentField, "almonds")), 1000)
                        .ScoreDocs
                        .Select(sd => indexer.Searcher.Doc(sd.Doc))
                        .ToArray();
                    Assert.IsFalse(docs.Any(d => d.GetField("Id").StringValue == guid.ToString()));

                }
                
            }
        }


        [Test]
        public void IndexerGerTest () {
            using (var indexer = new Indexer(this.Directory)) {
                // TODO: use the right analyser here; document contains "donaudampfschiff...."
                IndexTest(indexer, TestDir + "TikaGer.odt", "donau");

            }

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