using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Lucene.NET.Contracts
{
    [DataContract(Namespace = "Sample")]
    public sealed class CustomerId : AbstractIdentity<int>
    {
        public const string TagValue = "customer";

        public CustomerId() { }
        public CustomerId(int id)
        {
            this.Id = id;
        }
        [DataMember(Order = 1)]
        public override int Id { get; protected set; }

        public override string GetTag()
        {
            return TagValue;
        }
    }
}
