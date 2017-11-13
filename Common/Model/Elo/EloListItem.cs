using DataModel.Attributes;
using DataModel.Base;
using Newtonsoft.Json.Linq;

namespace Common.Model.Elo
{
    public class EloListItem : BaseEntity
    {
        [Category("ELO_LIST_ITEM_ENTITY", "Elo List Item Entity")]
        public static long ENTITY_CATEGORY_ID { get; set; }

        public long CategoryId { get; set; }
        public long EntityId { get; set; }
        public int EloRanking { get; set; }

        private EloListItem(long id, long categoryId, long entityId, int eloRanking) : base(id)
        {
            CategoryId = categoryId;
            EntityId = entityId;
            EloRanking = eloRanking;
        }

        public static EloListItem CreateFromReader(long id, long categoryId, 
            long entityId, int eloRanking)
        {
            return new EloListItem(id, categoryId, entityId,eloRanking);
        }

        public static EloListItem InstanciateFromListable(IListable listableItem)
        {
            return new EloListItem(0, listableItem.GetEntityCategoryId(), listableItem.GetId(), 0);
        }

        public override long GetEntityCategoryId()
        {
            return ENTITY_CATEGORY_ID;
        }

        public override JObject ToJsonToken()
        {
            throw new System.NotImplementedException();
        }
    }
}