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
    public class CustomerIndexService
    {
        public static string _luceneDir = @"..\..\Index";
        private static FSDirectory _directoryTemp;
        private static FSDirectory _directory
        {
            get
            {
                if (_directoryTemp == null) _directoryTemp = FSDirectory.Open(new DirectoryInfo(_luceneDir));
                return _directoryTemp;
            }
        }
        public CustomerId GetCustomerId(string userName)
        {
            return _search(userName).FirstOrDefault();
        }
        private static IEnumerable<CustomerId> _search(string searchQuery)
        {
            // validation
            if (string.IsNullOrEmpty(searchQuery.Replace("*", "").Replace("?", ""))) return new List<CustomerId>();

            // set up lucene searcher
            using (var searcher = new IndexSearcher(_directory, false))
            {
                var hits_limit = 1000;
                var analyzer = new StandardAnalyzer(Version.LUCENE_29);

                {
                    var query = new TermQuery(new Term("CustomerName", searchQuery));
                    var hits = searcher.Search(query, hits_limit).ScoreDocs;
                    var results = _mapLuceneToDataList(hits, searcher);

                    analyzer.Close();
                    searcher.Close();
                    searcher.Dispose();
                    return results;
                }
                
            }
        }
        
        
        private static IEnumerable<CustomerId> _mapLuceneToDataList(IEnumerable<ScoreDoc> hits, IndexSearcher searcher)
        {
            return hits.Select(hit => _mapLuceneDocumentToData(searcher.Doc(hit.doc))).ToList();
        }

        private static CustomerId _mapLuceneDocumentToData(Document doc)
        {
            return new CustomerId(Convert.ToInt32(doc.Get("Id")));
               
        }
    }
}
