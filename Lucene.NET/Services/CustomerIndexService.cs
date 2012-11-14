﻿using System;
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
                if (IndexWriter.IsLocked(_directoryTemp)) IndexWriter.Unlock(_directoryTemp);
                var lockFilePath = Path.Combine(_luceneDir, "write.lock");
                //if (File.Exists(lockFilePath)) File.Delete(lockFilePath);
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

                    //var parser = new QueryParser(Version.LUCENE_29, "CustomerName", analyzer);
                    //var query = parseQuery(searchQuery, parser);

                    //var query = new BooleanQuery();
                    //var names = searchQuery.Split(' ');
                    //var firstName = new TermQuery(new Term("CustomerName", names[0]));
                    //var lastName = new TermQuery(new Term("CusomterName", names[1]));
                    //query.Add(firstName, BooleanClause.Occur.MUST);
                    //query.Add(lastName, BooleanClause.Occur.MUST);

                    //var pquery = new PhraseQuery();
                    //pquery.Add(new Term("CustomerName", names[0]));
                    //pquery.Add(new Term("CustomerName", names[1]));
                    //var query = new BooleanQuery();
                    //query.Add(pquery, BooleanClause.Occur.MUST);

                    //query.Add(new Term("CustomerName",searchQuery.ToLowerInvariant()));

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
        private static Query parseQuery(string searchQuery, QueryParser parser)
        {
            Query query;
            try
            {
                query = parser.Parse(searchQuery.Trim());
            }
            catch (ParseException)
            {
                query = parser.Parse(QueryParser.Escape(searchQuery.Trim()));
            }
            return query;
        }
        private static IEnumerable<CustomerId> _mapLuceneToDataList(IEnumerable<Document> hits)
        {
            return hits.Select(_mapLuceneDocumentToData).ToList();
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
