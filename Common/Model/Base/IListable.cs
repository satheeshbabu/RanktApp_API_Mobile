using Newtonsoft.Json.Linq;

namespace DataModel.Base
{
    public interface IListable : IBaseEntity
    {
        //TODO Maybe add more info, such as position in list
        JObject GetListableJsonToken();
    }
}