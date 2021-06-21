using System;
using System.Collections.Generic;
using System.Text;

namespace Sample.Elasticsearch.Domain.Model
{
    public class ActorsAggregationModel
    {
        public double TotalAge { get; set; }
        public double TotalMovies { get; set; }
        public double AverageAge { get; set; }
    }
}
