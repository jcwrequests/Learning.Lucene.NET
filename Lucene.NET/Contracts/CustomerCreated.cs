using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Lucene.NET.Contracts
{
    [DataContract(Namespace = "DDDSample")]
    public sealed class CustomerCreated : IEvent<CustomerId>
    {
        public CustomerCreated(CustomerId customerId, string customerName)
        {
            this.Id = customerId;
            this.CustomerName = customerName;
        }

        [DataMember(Order = 1)]
        public CustomerId Id { get; private set; }
        [DataMember(Order = 2)]
        public string CustomerName { get; private set; }

    }
}
