using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lokad.Cqrs.AtomicStorage;

namespace Lucene.NET.Storage
{
    public class LucenStrategy : IDocumentStrategy
    {
        public TEntity Deserialize<TEntity>(System.IO.Stream stream)
        {
            throw new NotImplementedException();
        }

        public string GetEntityBucket<TEntity>()
        {
            throw new NotImplementedException();
        }

        public string GetEntityLocation<TEntity>(object key)
        {
            throw new NotImplementedException();
        }

        public void Serialize<TEntity>(TEntity entity, System.IO.Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}
