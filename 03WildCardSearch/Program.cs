using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticSearchWithDotNet;
using Nest;

namespace _03WildCardSearch
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

            client.DeleteIndex("people");

            // Create an index
            client.CreateIndex("people", c => c
                                                  .NumberOfReplicas(0)
                                                  .NumberOfShards(1));

            client.MapFluent<Person>(m => m.IndexAnalyzer("standard")
                                                        .Properties(props => props
                                                            .String(s => s
                                                                .Name(p => p.Message)
                                                                .Index(FieldIndexOption.analyzed))
                                                            .Number(n => n
                                                                .Name(p => p.Age))
                                                            .String(s => s
                                                                .Name(p => p.FirstName))
                                                            .String(s => s
                                                                .Name(p => p.Sex))
                                                            .String(s => s
                                                                .Name(p => p.LastName))));

            //Add some people
            var jp = new Person { FirstName = "JP", LastName = "Smith", Age = 37, Message = "OMG yay ES!", Sex = "Male" };
            var matt = new Person { FirstName = "Matt", LastName = "Toto", Age = 32, Message = "I'm JPs brother", Sex = "Male" };
            var christine = new Person { FirstName = "Christine", LastName = "Toto", Age = 0, Message = "I'm JPs wife", Sex = "Female" };
            var kevin = new Person { FirstName = "Kevin", LastName = "Toto", Age = 26, Message = "I'm JPs other brother", Sex = "Male" };

            client.Index(jp);
            client.Index(matt);
            client.Index(christine);
            client.Index(kevin);

            client.Refresh();


            ///// ** Wildcard search
            var searchResults = client.Search<Person>(s => s
                .From(0)
                .Size(10)
                .Query(q => q
                    .Wildcard(f => f.LastName, "to*", Boost: 1.0)
                )
            );

            foreach (var result in searchResults.Documents)
            {
                Console.WriteLine("Found: {0}", result.FirstName + " " + result.LastName);
            }

            Console.ReadKey();
        }
    }
}
