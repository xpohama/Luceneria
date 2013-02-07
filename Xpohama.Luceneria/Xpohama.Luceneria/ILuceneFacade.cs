
using System.Collections.Generic;
using Lucene;
using System.IO;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Search;
using System;

namespace Xpohama.Luceneria.Tests {
    public interface ILuceneFacade {
        void AddToConfiguration(Configuration cfg);

        IEnumerable<Guid> Query(string query);

        void AddIndexForFile(Guid id, Guid parentId, string contentType, byte[] binaryContent);

        void RemoveIndexForFile(Guid id, Guid parentId);
    }
}