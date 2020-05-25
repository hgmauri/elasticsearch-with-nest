using System;
using System.Collections.Generic;

namespace Sample.Elasticsearch.Domain.Indices
{
    public class IndexActors
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }

        public static List<IndexActors> GetSampleData()
        {
            var list = new List<IndexActors>
            {
                new IndexActors {Id = "b9044bf7-6917-4521-9b1b-f737a05965f9", BirthDate = new DateTime(1944, 9, 25), Name = "Michael Douglas"},
                new IndexActors {Id = "abc6caf0-6c60-4e9e-93d2-2f53fbe941e2", BirthDate = new DateTime(1956, 7, 9), Name = "Tom Hanks"},
                new IndexActors {Id = "7b0636fd-42ca-4435-a9ff-bebef6a40fda", BirthDate = new DateTime(1933, 3, 14), Name = "Michael Caine"},
                new IndexActors {Id = "b7666135-d20b-4a52-98f2-87023d4c2bdb", BirthDate = new DateTime(1974, 1, 30), Name = "Christian Bale"},
                new IndexActors {Id = "6b0b2f52-ee2d-4b90-9dcc-c303d1b9830c", BirthDate = new DateTime(1943, 8, 17), Name = "Robert De Niro"},
                new IndexActors {Id = "2eda34e3-3cdb-47b7-ac56-c5946d7d695a", BirthDate = new DateTime(1940, 4, 25), Name = "Al Pacino"}
            };
            return list;
        }
    }
}
