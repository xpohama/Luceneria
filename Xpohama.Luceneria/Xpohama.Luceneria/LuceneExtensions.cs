using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;
using System.Diagnostics;
using System.IO;

namespace Xpohama.Luceneria {
    public static class LuceneExtensions {
        public static void DisplayTokens (this Analyzer analyzer, string fieldName, string text) {
            DisplayTokens(analyzer.TokenStream(fieldName, new StringReader(text)));
        }
        public static void DisplayTokens (TokenStream stream) {
            // error in Lucene.Net? should work, look in source code why not
            // source: Lucene in Action, page ??
            var term = stream.AddAttribute<TermAttribute>();
            while (stream.IncrementToken()) {
#if LuceneV303
                Trace.WriteLine("[" + term.Term + "] ");
#endif
            }
        }
    }
}