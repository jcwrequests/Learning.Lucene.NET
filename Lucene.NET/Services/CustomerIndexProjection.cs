using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using LuceneSearch.Model;
using Version = Lucene.Net.Util.Version;
using Lucene.NET.Contracts;
using System.IO;

namespace Lucene.NET.Services
{
    public class CustomerIndexProjection
    {
        public static string _luceneDir =  @"..\..\Index";
        private static FSDirectory _directoryTemp;
        private static FSDirectory _directory
        {
            get
            {
                if (_directoryTemp == null) _directoryTemp = FSDirectory.Open(new DirectoryInfo(_luceneDir));
                if (IndexWriter.IsLocked(_directoryTemp)) IndexWriter.Unlock(_directoryTemp);
                var lockFilePath = Path.Combine(_luceneDir, "write.lock");
                if (File.Exists(lockFilePath)) File.Delete(lockFilePath);
                return _directoryTemp;
            }
        }

        public void When(CustomerCreated e)
        {
            var analyzer = new StandardAnalyzer(Version.LUCENE_29);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                _addToLuceneIndex(e, writer);

                // close handles
                analyzer.Close();
                writer.Close();
                writer.Dispose();
            }
        }
        private static void _addToLuceneIndex(CustomerCreated e, IndexWriter writer)
        {
            // remove older index entry
            var searchQuery = new TermQuery(new Term("CustomerName", e.CustomerName));
            writer.DeleteDocuments(searchQuery);

            // add new index entry
            var doc = new Document();

            // add lucene fields mapped to db fields
            doc.Add(new Field("Id",e.Id.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("CustomerName", e.CustomerName, Field.Store.YES, Field.Index.ANALYZED));
            
            // add entry to index
            writer.AddDocument(doc);
        }

    }
}
