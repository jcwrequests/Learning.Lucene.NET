using Lucene.Net.Documents;
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

            var r = GoLucene.GetAllIndexRecords();
            System.Diagnostics.Debugger.Break();
                    
        }
        
    }
}
