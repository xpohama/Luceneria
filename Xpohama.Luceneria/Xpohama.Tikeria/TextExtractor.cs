using java.io;
using javax.xml.transform;
using javax.xml.transform.sax;
using javax.xml.transform.stream;
using org.apache.tika.io;
using org.apache.tika.metadata;
using org.apache.tika.parser;
using org.apache.tika.sax;

namespace Xpohama.Tikeria {
    public class TextExtractor {
        protected TransformerHandler CreateTransformerHandler (StringWriter output) {
            var factory = TransformerFactory.newInstance() as SAXTransformerFactory;
            var handler = factory.newTransformerHandler();
            handler.getTransformer().setOutputProperty(OutputKeys.METHOD, "text");
            handler.getTransformer().setOutputProperty(OutputKeys.INDENT, "yes");

            handler.setResult(new StreamResult(output));
            return handler;
        }

        public string ExtractText (byte[] data) {
            var parser = new AutoDetectParser();
            var handler = new BodyContentHandler();
            var context = new ParseContext();
            context.set(parser.getClass(), parser);
            var metadata = new Metadata();
            using (var output = new StringWriter()) {
                var transformerHandler = CreateTransformerHandler(output);
                using (var inputStream = TikaInputStream.get(data, metadata)) {
                    parser.parse(inputStream, transformerHandler, metadata, context);
                    inputStream.close();
                }
                return output.toString();
            }
        }
    }


    public static class BodyTextExtractorExtensions {
        public static string Extract(this TextExtractor ex, string path) {
            return ex.ExtractText(System.IO.File.ReadAllBytes(path));
        }
    }
}