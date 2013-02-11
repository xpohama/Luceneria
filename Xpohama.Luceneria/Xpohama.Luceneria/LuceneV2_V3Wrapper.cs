using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Documents;

namespace Xpohama.Luceneria {
    /// <summary>
    /// extensions to make it possible to use Lucene 2.9 or 3.0
    /// </summary>
    public static class LuceneV2_V3Wrapper {
#if LuceneV290
        public static Lucene.Net.Util.Attribute AddAttribute<T>(this TokenStream stream) {
            return stream.AddAttribute(typeof(T));
        }

        public static Term Term (this Lucene.Net.Util.Attribute att) {
            throw new System.NotImplementedException();
        }

        public static string AsString (this Field field) {
            return field.StringValue();
        }
#else
        public static string AsString (this Field field) {
            return field.StringValue;
        }
#endif

    }
}