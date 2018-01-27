using DataModel.Attributes;
using DataModel.Base;
using Newtonsoft.Json.Linq;

namespace DataModel.Genres
{
    public class MovieGenre
    {
        public int ID { get; set; }
        public string SourceSite { get; set; }
        public string Name { get; set; }
        public long SourceId { get; set; }
    }
}