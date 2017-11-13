using System;
using System.Collections.Generic;
using DataModel.Attributes;
using DataModel.Base;
using Newtonsoft.Json.Linq;

namespace Common.Model.Elo
{
    public class EloList : BaseEntity
    {
        [Category("ELO_LIST_ENTITY", "Elo List Entity")]
        public static long ENTITY_CATEGORY_ID { get; set; }

        public string Name { get; set; }
        public long UserId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }

        public List<EloListItem> Items = new List<EloListItem>();
        public List<EloBattle> Battles = new List<EloBattle>();

        private EloList(long id, string name, long userId, 
            DateTime dateCreated, DateTime dateUpdated) : base(id)
        {
            Name = name;
            UserId = userId;
            DateCreated = dateCreated;
            DateUpdated = dateUpdated;
        }

        public static EloList CreateFromReader(long id, string name, long userId,
            DateTime dateCreated, DateTime dateUpdated)
        {
            return new EloList(id, name, userId, dateCreated, dateUpdated);
        }

        public static EloList Instanciate(string name, long userId,
            DateTime dateCreated, DateTime dateUpdated)
        {
            return new EloList(0, name, userId, dateCreated, dateUpdated);
        }

        public void AddListItem(EloListItem item)
        {
            Items.Add(item);
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