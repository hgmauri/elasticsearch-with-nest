using System;

namespace Sample.Elasticsearch.Infrastructure.Indices
{
    public class IndexActors : ElasticBaseIndex
    {
        public DateTime RegistrationDate { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime BirthDate { get; set; }
        public int Age { get; set; }
        public int TotalMovies { get; set; }
        public string Movies { get; set; }
    }
}
