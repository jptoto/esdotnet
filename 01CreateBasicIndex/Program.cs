using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticSearchWithDotNet;
using Nest;

namespace _01CreateBasicIndex
{
    public class Program
    {
        private static ConnectionSettings elasticSettings;
        public static ElasticClient client;

        static void Main(string[] args)
        {
            elasticSettings = new ConnectionSettings(new Uri("http://127.0.0.1:9200"))
                .SetDefaultIndex("people");

            client = new ElasticClient(elasticSettings);

            
            // Create an index
            client.CreateIndex("people", c => c
                                                    .NumberOfReplicas(0)
                                                    .NumberOfShards(1)
                                                    .AddMapping<Person>(m => m.MapFromAttributes()));


            // Add some people
            var jp = new Person {FirstName = "JP", LastName = "Toto", Age = 37, Message = "OMG yay ES!", Sex = "Male"};
            var matt = new Person { FirstName = "Matt", LastName = "Toto", Age = 32, Message = "I'm JPs brother", Sex = "Male" };
            var christine = new Person { FirstName = "Christine", LastName = "Toto", Age = 0, Message = "I'm JPs wife", Sex = "Female" };
            var kevin = new Person { FirstName = "Kevin", LastName = "Toto", Age = 26, Message = "I'm JPs other brother", Sex = "Male" };

            client.Index(jp);
            client.Index(matt);
            client.Index(christine);
            client.Index(kevin);
        }
    }
}
