using System.Collections.Generic;
using System.Threading.Tasks;
using DataModel.Base;

namespace DataModel.Genres
{
    public abstract class Genre : BaseEntity
    {
        public long Source { get; set; }
        public string SourceName { get; set; }
        public long SourceId { get; set; }

        protected Genre(long id, long source, string sourceName, long sourceId) : base(id)
        {
            Id = id;
            Source = source;
            SourceName = sourceName;
            SourceId = sourceId;
        }
    }
}