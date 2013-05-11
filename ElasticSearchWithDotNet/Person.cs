using Nest;

namespace ElasticSearchWithDotNet
{
    [ElasticType(Name = "Person")]
    public class Person
    {
        [ElasticProperty(Name = "FirstName")]
        public string FirstName { get; set; }

        [ElasticProperty(Name = "LastName")]
        public string LastName { get; set; }

        [ElasticProperty(Name = "Age")]
        public int Age { get; set; }

        [ElasticProperty(Name = "Sex")]
        public string Sex { get; set; }

        [ElasticProperty(Name = "Message")]
        public string Message { get; set; }
    }
}
