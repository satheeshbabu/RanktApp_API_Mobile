using DataModel.Attributes;
using DataModel.Base;
using Newtonsoft.Json.Linq;

namespace Common.Model.Elo
{
    public class EloBattle : BaseEntity
    {
        [Category("ELO_BATTLE_ENTITY", "Elo Battle Entity")]
        public static long ENTITY_CATEGORY_ID { get; set; }

        public long EloListId { get; set; }
        public long LeftEloItemId { get; set; }
        public long RightEloItemId { get; set; }
        public string WinnerItem { get; set; }

        private EloBattle(long id, long eloListId, long leftEloItemId, long rightEloItemId, string winnerItem) : base(id)
        {
            EloListId = eloListId;
            LeftEloItemId = leftEloItemId;
            RightEloItemId = rightEloItemId;
            WinnerItem = winnerItem;
        }

        public static EloBattle CreateFromReader(long id, long eloListId, long leftEloItemId, long rightEloItemId, string winnerItem)
        {
            return new EloBattle(id, eloListId, leftEloItemId, rightEloItemId, winnerItem);
        }

        public static EloBattle InstanciateEloBattle( long eloListId, long leftEloItemId, long rightEloItemId,
            string winnerItem)
        {
            return new EloBattle(0, eloListId, leftEloItemId, rightEloItemId, winnerItem);
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