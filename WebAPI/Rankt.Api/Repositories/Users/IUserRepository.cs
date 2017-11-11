using System.Threading.Tasks;
using Common.Model.Users;
using Trakker.Api.Repositories;

namespace Rankt.Api.Repositories.Users
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User> GetUserByNameAndPassword(string username, string password);
    }
}