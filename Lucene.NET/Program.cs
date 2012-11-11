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
            Contracts.CustomerCreated e2 = new Contracts.CustomerCreated(new Contracts.CustomerId(2), "Pumpkin Jones");
            CustomerIndexProjection projection = new CustomerIndexProjection();
            projection.When(e);
            projection.When(e2);

            string[] firstNames = { "John", "Jason", "Eric", "Eva", "Keegan", "Sophie", "Pumpkin" };
            string[] lastNames = { "Smith", "Jones", "Johnson", "Kirk", "Simpson", "Kahn", "Jobs" };
            Random randomizer = new Random();
            for (int i = 1; i <= 10000; i++)
            {
                int firstName = randomizer.Next(7);
                int lastName = randomizer.Next(7);
                Contracts.CustomerCreated randomEvent = new Contracts.
                                                            CustomerCreated(new Contracts.CustomerId(i + 1),
                                                            string.Format("{0} {1}", firstNames[firstName], lastNames[lastName]));
                projection.When(randomEvent);

            }


            System.Diagnostics.Debugger.Break();

            CustomerIndexService indexService = new CustomerIndexService();
            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            for (int i = 1; i <= 100; i++)
            {

                stopWatch.Start();
                CustomerId id = indexService.GetCustomerId("Rinat Abdullin");
                stopWatch.Stop();
                Console.WriteLine(id);
                Console.WriteLine(stopWatch.Elapsed.TotalMilliseconds.ToString());
                stopWatch.Reset();
                stopWatch.Start();
                id = indexService.GetCustomerId("Pumpkin Jones");
                stopWatch.Stop();
                Console.WriteLine(id);
                Console.WriteLine(stopWatch.Elapsed.TotalMilliseconds.ToString());
                stopWatch.Reset();
            }
            
            Console.ReadLine();
            System.Diagnostics.Debugger.Break();

           

        }
        
    }
}
