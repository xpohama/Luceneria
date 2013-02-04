/*
 * 
 * 
 * 
 * derived from: https://github.com/KevM/tikaondotnet/blob/master/TikaOnDotnet/TextExtractor.cs
 * license: 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using java.io;
using java.lang;
using javax.xml.transform;
using javax.xml.transform.sax;
using javax.xml.transform.stream;
using org.apache.tika.io;
using org.apache.tika.metadata;
using org.apache.tika.parser;
using Exception = System.Exception;
using String = System.String;
using StringBuilder = System.Text.StringBuilder;

namespace Xpohama.Tikeria.Tests {
   public class TextExtractor {
        private StringWriter _outputWriter;

        public TextExtractionResult Extract(string filePath) {
            if (!System.IO.File.Exists(filePath))
                throw new System.ArgumentException("File does not exist");

            var parser = new AutoDetectParser();
            var metadata = new Metadata();
            var parseContext = new ParseContext();
            Class parserClass = parser.GetType();
            parseContext.set(parserClass, parser);

            try {
                var file = new File(filePath);
                var url = file.toURI().toURL();
                using (var inputStream = TikaInputStream.get(url, metadata)) {
                    parser.parse(inputStream, getTransformerHandler(), metadata, parseContext);
                    inputStream.close();
                }

                return assembleExtractionResult(_outputWriter.toString(), metadata);
            } catch (Exception ex) {
                throw new ApplicationException(string.Format("Extraction of text from the file '{0}' failed.", filePath), ex);
            }
        }

        private TextExtractionResult assembleExtractionResult(string text, Metadata metadata) {
            Dictionary<string, string> metaDataResult = metadata.names().ToDictionary(name => name,
                                                                                      name =>
                                                                                      String.Join(", ", metadata.getValues(name)));

            string contentType = metaDataResult["Content-Type"];

            return new TextExtractionResult {
                Text = text,
                ContentType = contentType,
                Metadata = metaDataResult
            };
        }

        private TransformerHandler getTransformerHandler() {
            var factory = TransformerFactory.newInstance() as SAXTransformerFactory;
            var handler = factory.newTransformerHandler();
            handler.getTransformer().setOutputProperty(OutputKeys.METHOD, "text");
            handler.getTransformer().setOutputProperty(OutputKeys.INDENT, "yes");

            _outputWriter = new StringWriter();
            handler.setResult(new StreamResult(_outputWriter));
            return handler;
        }
    }

   public class TextExtractionResult {
       public string Text { get; set; }
       public string ContentType { get; set; }
       public IDictionary<string, string> Metadata { get; set; }

       public override string ToString() {
           var builder = new StringBuilder("Text:\n" + Text + "MetaData:\n");

           foreach (var keypair in Metadata) {
               builder.AppendFormat("{0} - {1}\n", keypair.Key, keypair.Value);
           }

           return builder.ToString();
       }
   }
}