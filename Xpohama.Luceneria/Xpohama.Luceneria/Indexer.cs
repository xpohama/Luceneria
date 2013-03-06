using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Util;
using Lucene.Net.Analysis;
using Lucene.Net.Search;
using Xpohama.Tikeria;


namespace Xpohama.Luceneria {

    public class Indexer:System.IDisposable {
        public Version Version { get { return Version.LUCENE_29; } }

        public Indexer (Directory dir) {
            this.Directory = dir;
        }

        public Directory Directory { get; set; }

        IndexWriter _writer = null;
        /// <summary>
        /// IndexWriter of Directory, using Analyser
        /// </summary>
        public IndexWriter Writer {
            get {
                if (_writer == null) {
                    _writer = new IndexWriter(
                                    this.Directory,
                                    this.Analyser,
                                    false,
                                    IndexWriter.MaxFieldLength.UNLIMITED);
                }
                return _writer;
            }
        }

        Analyzer _analyser = null;
        /// <summary>
        /// Analyser used in IndexWriter
        /// to be used in Query too
        /// </summary>
        public Analyzer Analyser {
            get { return _analyser ?? (_analyser = new StandardAnalyzer(this.Version)); }
            set { _analyser = value;}
        }

        bool refreshReader = false;
        IndexReader _reader = null;
        /// <summary>
        /// IndexReader of Directory
        /// uses Writer if there is one
        /// opening a new IndexReader is time-consuming
        /// use Refresh() to get an IndexReader with the latest index contents 
        /// </summary>
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
        /// <summary>
        /// IndexSearcher on IndexReader
        /// use Refresh() to get the latest index contents
        /// </summary>
        public IndexSearcher Searcher {
            get {
                return _searcher ?? (_searcher = new IndexSearcher(this.Reader));
            }
        }

        /// <summary>
        /// closes Writer, Reader and Searcher and set them to null
        /// </summary>
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

        /// <summary>
        /// Refreshes the Reader and the Searcher to get the latest index contents
        /// </summary>
        public void Refresh() {
            this.refreshReader = true;
            _searcher = null;
        }

        string _documentContentField = null;
        /// <summary>
        /// name of field to index document streams
        /// </summary>
        public string DocumentContentField {
            get { return _documentContentField ?? "contents"; }
            set { _documentContentField = value; }
        }

        /// <summary>
        /// creates a Document out of data
        /// and adds all words of data into a field 
        /// uses Tika to extract the text out of data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Document CreateDocument (byte[] data) {
            var ex = new TextExtractor();
            var doc = new Document();
            var content = ex.ExtractText(data);
            doc.Add(new Field(DocumentContentField, content, Field.Store.NO, Field.Index.ANALYZED));

            return doc;
        }

        public void Dispose () {
            Close();
        }
    }
}