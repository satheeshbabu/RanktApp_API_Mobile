using System.Threading.Tasks;
using Common.Model.Users;

namespace Trakker.Api.Repositories.Users
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User> GetUserByNameAndPassword(string username, string password);
    }
}