using System.Threading.Tasks;
using Common.Model.AuthTokens;
using Trakker.Api.Repositories;

namespace Rankt.Api.Repositories.AuthTokens
{
    public interface IAuthTokenRepository : IBaseRepository<AuthToken>
    {
        Task<AuthToken> GetByUserId(long userId);
    }
}