using System.Collections.Generic;
using Sample.Elasticsearch.Domain.Indices;

namespace Sample.Elasticsearch.Domain.Concrete
{
    public interface IActorsApplication
    {
        void PostActorsSample();
        ICollection<IndexActors> GetAll();
        ICollection<IndexActors> GetByName(string name);
    }
}
