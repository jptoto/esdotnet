using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElasticSearchWithDotNet;
using Nest;

namespace _06RankingSearch
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


            // Add some people
            var jp = new Person { FirstName = "JP", LastName = "Smith", Age = 37, Message = "OMG yay ES!", Sex = "Male" };
            var matt = new Person { FirstName = "Matt", LastName = "boosting", Age = 32, Message = "test", Sex = "Male" };
            var christine = new Person { FirstName = "Christine", LastName = "Toto", Age = 0, Message = "I'm JPs wife", Sex = "Female" };
            var kevin = new Person { FirstName = "Kevin", LastName = "boosting", Age = 26, Message = "I'm JPs other boosting", Sex = "Male" };

            client.Index(jp);
            client.Index(matt);
            client.Index(christine);
            client.Index(kevin);

            client.Refresh();

            ///// ** Basic search with QueryString
            var searchResults = client.Search<Person>(s => s.Query(q => q.
                            QueryString(x => x.Query("boosting"))));

            foreach (var result in searchResults.Documents)
            {
                Console.WriteLine("Found: {0}", result.FirstName + " " + result.LastName);
            }



            Console.ReadKey();



            // Boosting

            client.DeleteIndex("people");

            // Create an index

            client.MapFluent<Person>(m => m.IndexAnalyzer("standard")
                                                        .Properties(props => props
                                                            .String(s => s
                                                                .Name(p => p.Message)
                                                                .Index(FieldIndexOption.analyzed).Boost(2.0))
                                                            .Number(n => n
                                                                .Name(p => p.Age))
                                                            .String(s => s
                                                                .Name(p => p.FirstName))
                                                            .String(s => s
                                                                .Name(p => p.Sex))
                                                            .String(s => s
                                                                .Name(p => p.LastName))));


            // Add some people
            var jp1 = new Person { FirstName = "JP", LastName = "Smith", Age = 37, Message = "OMG yay ES!", Sex = "Male" };
            var matt1 = new Person { FirstName = "Matt", LastName = "boosting", Age = 32, Message = "boosting", Sex = "Male" };
            var christine1 = new Person { FirstName = "Christine", LastName = "Toto", Age = 0, Message = "I'm JPs wife", Sex = "Female" };
            var kevin1 = new Person { FirstName = "Kevin", LastName = "boosting", Age = 26, Message = "I'm JPs other test", Sex = "Male" };

            client.Index(jp1);
            client.Index(matt1);
            client.Index(christine1);
            client.Index(kevin1);

            client.Refresh();

            ///// ** Basic search with QueryString
            var searchResults2 = client.Search<Person>(s => s.Query(q => q.
                            QueryString(x => x.Query("boosting"))));

            foreach (var result in searchResults2.Documents)
            {
                Console.WriteLine("Found: {0}", result.FirstName + " " + result.LastName);
            }

            Console.ReadKey();
        }
    }
}
