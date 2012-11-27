using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lucene.NET.Storage
{
    public interface ISerializationStrategy
    {
        void Serialize<TEntity>(TEntity entity, Stream stream);
        TEntity Deserialize<TEntity>(Stream stream);
        string GetEntityLocationPath<TEntity>(object key);
        string GetEntityBucket<TEntity>();
    }
}
