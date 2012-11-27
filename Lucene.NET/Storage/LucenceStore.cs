using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lokad.Cqrs.AtomicStorage;
using System.IO;
using Hyper.ComponentModel;
using System.ComponentModel;
using Lucene.Net.Search;
using Lucene.Net.Index;
using Lucene.Net.Analysis.Standard;
using Version = Lucene.Net.Util.Version;
using Lucene.Net.Documents;

namespace Lucene.NET.Storage
{
    public class LucenceStore<TKey, TEntity> : IDocumentReader<TKey, TEntity>,
                                               IDocumentWriter<TKey, TEntity>
     {
        readonly string _folder;
        readonly ISerializationStrategy _strategy;
        readonly string _indexPath;

        public LucenceStore(string directoryPath, 
                            ISerializationStrategy strategy,
                            string indexPath)
        {
            _folder = directoryPath;
            _strategy = strategy;
            _indexPath = indexPath;
            HyperTypeDescriptionProvider.Add(typeof(TKey));
        }

        public void InitIfNeeded()
        {
            Directory.CreateDirectory(_folder);
        }

        public bool TryGet(TKey key, out TEntity view)
        {
            view = default(TEntity);
            try
            {
                var name = GetFileName(key);
                //need implement lucene handling hear


                if (!File.Exists(name))
                    return false;

                using (var stream = File.Open(name, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    if (stream.Length == 0)
                        return false;
                    view = _strategy.Deserialize<TEntity>(stream);
                    return true;
                }
            }
            catch (FileNotFoundException)
            {
                // if file happened to be deleted between the moment of check and actual read.
                return false;
            }
            catch (DirectoryNotFoundException)
            {
                return false;
            }
        }

        string GetName(TKey key)
        {
            //key will be guid
            return Path.Combine(_folder, _strategy.GetEntityLocationPath<TEntity>(key));
        }

        

        string GetFileName(TKey key)
        {
            using (var searcher = new IndexSearcher(_indexPath, false))
            {
                var hits_limit = 1000;
                var analyzer = new StandardAnalyzer(Version.LUCENE_29);

                {
                    var query = new BooleanQuery();
                    PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(key);

                    foreach (PropertyDescriptor property in properties)
                    {

                        var value = property.GetValue(key).ToString();
                        var name = property.Name;
                        query.Add(new TermQuery(new Term(property.Name, property.GetValue(key).ToString())), BooleanClause.Occur.MUST);
                    }
                   

                    var hits = searcher.Search(query, hits_limit).ScoreDocs;
                    var results = _mapLuceneToDataList(hits, searcher);

                    analyzer.Close();
                    searcher.Close();
                    searcher.Dispose();

                    //if null the return default file namimg convention
                    return results.FirstOrDefault();
                }

            }

        }

        private static IEnumerable<string> _mapLuceneToDataList(IEnumerable<ScoreDoc> hits, IndexSearcher searcher)
        {
            return hits.Select(hit => _mapLuceneDocumentToData(searcher.Doc(hit.doc)));
        }

        private static string _mapLuceneDocumentToData(Document doc)
        {
            return doc.Get("documentPath");

        }


        public TEntity AddOrUpdate(TKey key, Func<TEntity> addFactory, Func<TEntity, TEntity> update,
            AddOrUpdateHint hint)
        {
            var name = GetFileName(key);
            //need implement lucene handling hear

            try
            {
                // This is fast and allows to have git-style subfolders in atomic strategy
                // to avoid NTFS performance degradation (when there are more than 
                // 10000 files per folder). Kudos to Gabriel Schenker for pointing this out
                var subfolder = Path.GetDirectoryName(name);
                if (subfolder != null && !Directory.Exists(subfolder))
                    Directory.CreateDirectory(subfolder);
 

                // we are locking this file.
                using (var file = File.Open(name, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                {
                    byte[] initial = new byte[0];
                    TEntity result;
                    if (file.Length == 0)
                    {
                        result = addFactory();
                    }
                    else
                    {
                        using (var mem = new MemoryStream())
                        {
                            file.CopyTo(mem);
                            mem.Seek(0, SeekOrigin.Begin);
                            var entity = _strategy.Deserialize<TEntity>(mem);
                            initial = mem.ToArray();
                            result = update(entity);
                        }
                    }

                    // some serializers have nasty habbit of closing the
                    // underling stream
                    using (var mem = new MemoryStream())
                    {
                        _strategy.Serialize(result, mem);
                        var data = mem.ToArray();

                        if (!data.SequenceEqual(initial))
                        {
                            // upload only if we changed
                            file.Seek(0, SeekOrigin.Begin);
                            file.Write(data, 0, data.Length);
                            // truncate this file
                            file.SetLength(data.Length);
                        }
                    }

                    return result;
                }
            }
            catch (DirectoryNotFoundException)
            {
                var s = string.Format(
                    "Container '{0}' does not exist.",
                    _folder);
                throw new InvalidOperationException(s);
            }
        }

        public bool TryDelete(TKey key)
        {
            var name = GetName(key);
            //need implement lucene handling hear


            if (File.Exists(name))
            {
                File.Delete(name);
                return true;
            }
            return false;
        }
       
    }
}
