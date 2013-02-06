
using System.Collections.Generic;
using Lucene;
using System.IO;
using Lucene.Net.Store;
using Lucene.Net.Index;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;
using System;
using System.Linq;
using Lucene.Net.Documents;

namespace Xpohama.Luceneria.Tests {
    public interface ILuceneFacade {
        void AddToConfiguration(Configuration cfg);

        IEnumerable<Guid> Query(string query);

        void AddIndexForFile(Guid id, Guid parentId, string contentType, byte[] binaryContent);

        void RemoveIndexForFile(Guid id, Guid parentId);
    }

    public class LuceneFacade:ILuceneFacade {

        private const string IndexBaseDirectoryName = "LuceneIndex";
        private const string IdSeperator = " - ";

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
                .Select(sd => indexer.Searcher.Doc(sd.Doc).GetField("Id").StringValue)
                .Select(id => Guid.Parse(id.Contains(IdSeperator) ?
                        id.Substring(0, id.IndexOf(IdSeperator) + 1) : id));


            return ids;
        }

        protected virtual string IdFromGuid(Guid id, Guid parentId) {
            return parentId.ToString() + IdSeperator + id.ToString();
        }

        public void AddIndexForFile(Guid id, Guid parentId, string contentType, byte[] binaryContent) {
            var doc = Indexer.CreateDocument(binaryContent);
            doc.Add(new Field("Id", IdFromGuid(id,parentId), Field.Store.YES, Field.Index.NOT_ANALYZED));
            Indexer.Writer.AddDocument(doc);
            Indexer.Refresh();
        }

        public void RemoveIndexForFile(Guid id, Guid parentId) {
            Indexer.Writer.DeleteDocuments(new Term("Id", IdFromGuid(id, parentId)));
            Indexer.Refresh();
        }

        public string[] allSearchableFieldsIncludingFiles { get; set; }
    }
}