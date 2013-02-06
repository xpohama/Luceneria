using Lucene.Net.Documents;
using Xpohama.Tikeria.Tests;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Util;
using Lucene.Net.Analysis;
using Lucene.Net.Search;


namespace Xpohama.Luceneria.Tests {

    public class Indexer:System.IDisposable {
        public Version Version { get { return Version.LUCENE_29; } }

        public Indexer (Directory dir) {
            this.Directory = dir;
        }

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

        Analyzer _analyser = null;
        public Analyzer Analyser {
            get { return _analyser ?? (_analyser = new StandardAnalyzer(this.Version)); }
            set { _analyser = value;}
        }

        bool refreshReader = false;
        IndexReader _reader = null;
        public IndexReader Reader {
            get {
                if (_reader == null) {
                    if (_writer == null) {
                        _reader = IndexReader.Open(this.Directory, true);
                    } else {
                        // get a near real time reader
                        _reader = _writer.GetReader();
                    }
                } else if (refreshReader) {
                    var newReader = _writer == null? _reader.Reopen():_writer.GetReader();
                    if (_reader != newReader) {
                        _reader.Close();
                        _reader = newReader;
                    }
                    _searcher = null;
                    refreshReader = false;
                }
                return _reader;
            }
        }

        IndexSearcher _searcher = null;
        public IndexSearcher Searcher {
            get {
                return _searcher ?? (_searcher = new IndexSearcher(this.Reader));
            }
        }

        public void Close () {
           
            if (_writer != null) {
                _writer.Close();
                _writer = null;
            }
            if (_reader != null) {
                _reader.Close();
                _reader = null;
            }
            _searcher = null;
        }

        public void Refresh() {
            this.refreshReader = true;
            _searcher = null;
        }
        public string DocumentContentField { get { return "contents"; } }
        public Document CreateDocument (byte[] data) {
            var ex = new BodyTextExtractor();
            var doc = new Document();
            doc.Add(new Field(DocumentContentField, ex.ExtractText(data), Field.Store.NO, Field.Index.ANALYZED));

            return doc;
        }

        public void Dispose () {
            Close();
        }
    }
}