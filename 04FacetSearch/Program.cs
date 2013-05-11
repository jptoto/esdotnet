using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticSearchWithDotNet;
using Nest;

namespace _04FacetSearch
{
    class Program
    {
        private static ConnectionSettings elasticSettings;
        public static ElasticClient client;

        static void Main(string[] args)
        {
            elasticSettings = new ConnectionSettings(new Uri("http://home-win8:9200"))
            .SetDefaultIndex("people");

            client = new ElasticClient(elasticSettings);

            client.DeleteIndex("people");

            // Create an index
            client.CreateIndex("people", c => c
                                                  .NumberOfReplicas(0)
                                                  .AddMapping<Person>(m => m.MapFromAttributes())
                                                  .NumberOfShards(1));
                                                  

            //client.MapFluent<Person>(m => m.IndexAnalyzer("standard")
            //                                            .Properties(props => props
            //                                                .String(s => s
            //                                                    .Name(p => p.Message)
            //                                                    .Index(FieldIndexOption.analyzed))
            //                                                .Number(n => n
            //                                                    .Name(p => p.Age))
            //                                                .String(s => s
            //                                                    .Name(p => p.FirstName))
            //                                                .String(s => s
            //                                                    .Name(p => p.Sex))
            //                                                .String(s => s
            //                                                    .Name(p => p.LastName))));

            // Add some people
            var jp = new Person { FirstName = "JP", LastName = "Toto", Age = 37, Message = "OMG yay ES!", Sex = "Male" };
            var matt = new Person { FirstName = "Matt", LastName = "Toto", Age = 37, Message = "I'm JPs brother", Sex = "Male" };
            var christine = new Person { FirstName = "Christine", LastName = "Toto", Age = 0, Message = "I'm JPs wife", Sex = "Female" };
            var kevin = new Person { FirstName = "Kevin", LastName = "Smith", Age = 26, Message = "I'm JPs other brother", Sex = "Male" };

            client.Index(jp);
            client.Index(matt);
            client.Index(christine);
            client.Index(kevin);

            client.Flush(true);

            var results = client.Search<Person>(s => s
                            .MatchAll()
                            .FacetStatistical(fs => fs
                                .OnField(f => f.Age)
                            ));

            var facet = results.Facet<StatisticalFacet>(f => f.Age);

            Console.WriteLine("Statistical Facets");
            Console.WriteLine("");
            Console.WriteLine("Max: {0}", facet.Max);
            Console.WriteLine("Min: {0}", facet.Min);
            Console.WriteLine("Std Dev: {0}", facet.StandardDeviation);
            Console.WriteLine("Total: {0}", facet.Total);
            Console.WriteLine("Variance: {0}", facet.Variance);

            Console.ReadKey();

            Console.Clear();

            Console.WriteLine("Histogram Facets");
            Console.WriteLine("");
            var facetResults = client.Search<Person>(s => s
                            .MatchAll()
                            .FacetHistogram(fs => fs
                                .OnField(f => f.Age)
                                .Interval(1)
                            ));

            var facet2 = facetResults.Facet<HistogramFacet>(f => f.Age);

            foreach (var item in facet2.Items)
            {
                Console.WriteLine("Key: {0}  Count: {1}", item.Key, item.Count);    
            }

            Console.ReadKey();


            Console.Clear();

            Console.WriteLine("Term Facets");
            Console.WriteLine("");
            var facetResults2 = client.Search<Person>(s => s
                .From(0)
                .Size(10)
                .MatchAll()
                .FacetTerm(t => t.OnField(f => f.LastName).Size(20))
            );

            var facet3 = facetResults2.Facet<TermFacet>(f => f.LastName);
            foreach (var item in facet3.Items)
            {
                Console.WriteLine("Key: {0}  Count: {1}", item.Term, item.Count);
            }

            Console.ReadKey();
        }
    }
}
