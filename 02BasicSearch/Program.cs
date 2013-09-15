using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticSearchWithDotNet;
using Nest;

namespace _02BasicSearch
{
    class Program
    {
        private static ConnectionSettings elasticSettings;
        public static ElasticClient client;


        static void Main(string[] args)
        {
            elasticSettings = new ConnectionSettings(new Uri("http://127.0.0.1:9200"))
                .SetDefaultIndex("people");

            client = new ElasticClient(elasticSettings);

            ///// ** Basic search with QueryString
            var searchResults = client.Search<Person>(s => s.Query(q => q.
                                                    QueryString(x => x.Query("toto").
                                                                        OnField(of => of.LastName))));

            foreach (var result in searchResults.Documents)
            {
                Console.WriteLine("Found: {0}", result.FirstName + " " + result.LastName);
            }

            
            
            
            ///// ** Basic search on Message
            //var searchResults = client.Search<Person>(s => s.Query(q => q.
            //    QueryString(x => x.Query("brother").
            //        OnField(of => of.Message).
            //        Operator((Operator.and)))));

            //foreach (var result in searchResults.Documents)
            //{
            //    Console.WriteLine("Found: {0}", result.FirstName + " " + result.LastName);
            //}




            
            
            
            
            
            
            
            
            
            
            
            Console.ReadKey();

        }
    }
}
