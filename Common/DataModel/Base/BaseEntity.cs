using Newtonsoft.Json.Linq;

namespace DataModel.Base
{
    public abstract class BaseEntity : IBaseEntity
    {
        protected long Id ;
        
        //TODO at one point update all to LONG
        protected BaseEntity(long id)
        {
            Id = id;
        }

        protected BaseEntity()
        {
            Id = 0;
        }

        public abstract long GetEntityCategoryId();

        public abstract JObject ToJsonToken();

        public long GetId()
        {
            return Id;
        }
        
        public void SetId(long id)
        {
            Id = id;
        }
    }

    public interface IBaseEntity
    {
        long GetId();
        long GetEntityCategoryId();
        JObject ToJsonToken();
    }
}