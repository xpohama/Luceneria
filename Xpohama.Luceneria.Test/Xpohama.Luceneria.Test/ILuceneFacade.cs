
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

        public void OpenIndex() {
            
        }

        public IEnumerable<Guid> Query(string query) {
            var directoryInfo = new DirectoryInfo(IndexDirectoryName);
            var directory = FSDirectory.Open(directoryInfo);

            var indexReader = IndexReader.Open(directory, true);
            var indexSearch = new IndexSearcher(indexReader);

            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
            var parser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_29, allSearchableFieldsIncludingFiles, analyzer);

            var hits = indexSearch.Search(parser.Parse(query));

            var ids = new List<Guid>();
            for (var i = 0; i < hits.Length(); i++) {
                var doc = indexSearch.Doc(i);
                var idString = doc.GetField("Id").StringValue();

                if (idString.Contains(IdSeperator))
                    idString = idString.Split(new[] { IdSeperator }, StringSplitOptions.None).First();

                var id = Guid.Parse(idString);

                if (!ids.Contains(id))
                    ids.Add(Guid.Parse(idString));
            }

            return ids;
        }

        public void AddIndexForFile(Guid id, Guid parentId, string contentType, byte[] binaryContent) {
            throw new NotImplementedException();
        }

        public void RemoveIndexForFile(Guid id, Guid parentId) {
            throw new NotImplementedException();
        }

        public string[] allSearchableFieldsIncludingFiles { get; set; }
    }
}