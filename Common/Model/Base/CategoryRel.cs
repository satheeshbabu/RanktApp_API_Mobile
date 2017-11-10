using DataModel.Attributes;
using Newtonsoft.Json.Linq;

namespace DataModel.Base
{
    public class CategoryRel : BaseEntity
    {
        [Category("CATEGORY_RELATION_ENTITY", "Category Entity")]
        public static long ENTITY_CATEGORY_ID { get; set; }
        
        public long ParentCatId { get; set; }
        public long ChildCatId { get; set; }
        public string Status { get; set; }

        private CategoryRel(long id, long parentCatId, long childCatId, string status) : base(id)
        {
            Id = id;
            ParentCatId = parentCatId;
            ChildCatId= childCatId;
            Status = status;
        }

        public static CategoryRel Instanciate(long parentId, long childId,
            string status)
        {
            return new CategoryRel(0, parentId, childId, status);
        }

        public override long GetEntityCategoryId()
        {
            throw new System.NotImplementedException();
        }

        public override JObject ToJsonToken()
        {
            throw new System.NotImplementedException();
        }
    }
}