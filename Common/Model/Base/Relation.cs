using System;
using DataModel.Attributes;
using Newtonsoft.Json.Linq;

namespace DataModel.Base
{
    public class Relation : BaseEntity
    {
        [Category("RELATION ENTITY", "Relation Entity")]
        public static long ENTITY_CATEGORY_ID { get; set; }
        [Category("RELATION STATUS ACTIVE", "Relation Status Active")]
        public static long RELATION_STATUS_ACTIVE_ID { get; set; }
        [Category("RELATION STATUS INACTIVE", "Relation Status Inactive")]
        public static long RELATION_STATUS_INACTIVE_ID { get; set; }
        
        public long CategoryFrom { get; set; }
        public long EntityFrom { get; set; }
        public long CategoryTo { get; set; }
        public long EntityTo { get; set; }
        public long RelationStatus { get; set; }
        public DateTime CreatedDate { get; set; }

        private Relation(long id, long categoryFrom, long entityFrom, long categoryTo, long entityTo, long relationStatus, DateTime createdDate) : base(id)
        {
            Id = id;
            CategoryFrom = categoryFrom;
            EntityFrom = entityFrom;
            CategoryTo = categoryTo;
            EntityTo = entityTo;
            RelationStatus = relationStatus;
            CreatedDate = createdDate;
        }

        public static Relation CreateFromReader(long id, long categoryFrom, long entityFrom, long categoryTo, long entityTo,
            long relationStatus, DateTime createdDate)
        {
            return new Relation(id, categoryFrom, entityFrom, categoryTo, entityTo, relationStatus, createdDate);
        }

        public static Relation Instanciate(long categoryFrom, long entityFrom, long categoryTo, long entityTo,
            long relationStatus, DateTime createdDate)
        {
            return new Relation(0, categoryFrom, entityFrom, categoryTo, entityTo, relationStatus, createdDate);
        }

        public override long GetEntityCategoryId()
        {
            throw new NotImplementedException();
        }

        public override JObject ToJsonToken()
        {
            var token = new JObject
            {
                { "id", Id},
                { "cat_from", CategoryFrom},
                { "entity_from", EntityFrom},
                {"cat_to", CategoryTo },
                {"entity_to", EntityTo }
            };
            return token;
        }
    }
    
    
}