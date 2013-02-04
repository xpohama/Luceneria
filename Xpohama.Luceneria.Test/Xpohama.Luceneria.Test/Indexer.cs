using Lucene.Net.Documents;
using Xpohama.Tikeria.Tests;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Util;
using Lucene.Net.Analysis;

namespace Xpohama.Luceneria.Tests {

    public class Indexer {
        Version Version { get { return Version.LUCENE_29; } }
       
        public Directory Directory { get; set; }

        IndexWriter _writer = null;
        public IndexWriter Writer {
            get {
                if (_writer == null) {
                    _writer = new IndexWriter(
                                    this.Directory,
                                    this.Analyser,
                                    true,
                                    IndexWriter.MaxFieldLength.UNLIMITED);
                }
                return _writer;
            }
        }

        public Indexer (Directory dir) {
            this.Directory = dir;
        }

        public void Close () {
            if (_writer != null) {
                _writer.Close();
                _writer = null;
            }
        }

        public Document CreateDocument (System.Guid id, byte[] data) {
            var ex = new BodyTextExtractor();
            var doc = new Document();
            doc.Add(new Field("contents", ex.ExtractText(data), Field.Store.NO, Field.Index.ANALYZED));
            doc.Add(new Field("ID", id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            return doc;
        }

        Analyzer _analyser = null;
        public Analyzer Analyser {
            get { return _analyser ?? (_analyser = new StandardAnalyzer(this.Version)); }
            set { _analyser = value;}
        }
    }
}