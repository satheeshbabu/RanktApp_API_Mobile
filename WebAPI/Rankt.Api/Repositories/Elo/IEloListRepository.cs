using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Model.Elo;

namespace Rankt.Api.Repositories.Elo
{
    public interface IEloListRepository : IBaseRepository<EloList>
    {
        Task<List<EloList>> GetAllListsForUser(long userId);
    }
}