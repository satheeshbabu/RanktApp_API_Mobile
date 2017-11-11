using System.Collections.Generic;
using System.Threading.Tasks;
using DataModel.Overall;

namespace Rankt.Api.Repositories.Lists
{
    public interface IMediaListRepository : IBaseRepository<MediaList>
    {
        Task<MediaList> GetBySourceAndCollectionId(long source, string id, bool includeMediaElements);
        Task<List<MediaList>> GetAllLists(bool includeMediaElements);
    }
}