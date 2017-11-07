using System.Collections.Generic;
using System.Threading.Tasks;
using DataModel.Overall;
using Trakker.Api.Repositories;

namespace TrakkerApp.Api.Repositories.Lists
{
    public interface IMediaListRepository : IBaseRepository<MediaList>
    {
        Task<MediaList> GetBySourceAndCollectionId(long source, string id, bool includeMediaElements);
        Task<List<MediaList>> GetAllLists(bool includeMediaElements);
    }
}