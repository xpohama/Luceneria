using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Store;

namespace Xpohama.Luceneria.Tests {
    public class LuceneFacade:ILuceneFacade {

        private const string IndexBaseDirectoryName = "LuceneIndex";
        private const string IdSeperator = " - ";
        public string IdFieldName = "Id";

        public void AddToConfiguration(Configuration cfg) {
            
        }

        public string IndexDirectoryName {
            get { return IndexBaseDirectoryName; }
        }

        Indexer _indexer = null;
        public Indexer Indexer {
            get {
                return _indexer ?? (_indexer = new Indexer(new RAMDirectory()));
            }
        }

        
        public IEnumerable<Guid> Query (string query) {
            var indexer = this.Indexer;
            var parser = new MultiFieldQueryParser(Indexer.Version, allSearchableFieldsIncludingFiles, Indexer.Analyser);

            var ids = indexer.Searcher
                .Search(parser.Parse(query), 1000)
                .ScoreDocs
                .Select(sd => indexer.Searcher.Doc(sd.Doc))
                .Where(doc => doc.GetField(IdFieldName) != null)
                .Select(doc => doc.GetField(IdFieldName).StringValue)
                .Select(id => Guid.Parse(id.Contains(IdSeperator) ?
                              id.Substring(0, id.IndexOf(IdSeperator) + 1) : id));


            return ids;
        }

        protected virtual string IdFromGuid(Guid id, Guid parentId) {
            return parentId.ToString() + IdSeperator + id.ToString();
        }

        public void AddIndexForFile(Guid id, Guid parentId, string contentType, byte[] binaryContent) {
            var doc = Indexer.CreateDocument(binaryContent);
            doc.Add(new Field(IdFieldName, IdFromGuid(id, parentId), Field.Store.YES, Field.Index.NOT_ANALYZED));
            Indexer.Writer.AddDocument(doc);
            Indexer.Refresh();
        }

        public void RemoveIndexForFile(Guid id, Guid parentId) {
            Indexer.Writer.DeleteDocuments(new Term(IdFieldName, IdFromGuid(id, parentId)));
            Indexer.Refresh();
        }

        public string[] allSearchableFieldsIncludingFiles { get; set; }
    }
}