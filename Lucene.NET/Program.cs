using Lucene.Net.Documents;
using Lucene.NET.Contracts;
using Lucene.NET.Services;
using LuceneSearch.Data;
using LuceneSearch.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lucene.NET
{
    class Program
    {
        static void Main(string[] args)
        {
            //GoLucene.AddUpdateLuceneIndex(SampleDataRepository.GetAll());

            //var r = GoLucene.GetAllIndexRecords();
            Contracts.CustomerCreated e = new Contracts.CustomerCreated(new Contracts.CustomerId(1), "Rinat Abdullin");
            CustomerIndexProjection projection = new CustomerIndexProjection();
            projection.When(e);

            System.Diagnostics.Debugger.Break();

            CustomerIndexService indexService = new CustomerIndexService();
            CustomerId id = indexService.GetCustomerId("Rinat Abdullin");

            System.Diagnostics.Debugger.Break();
        }
        
    }
}
