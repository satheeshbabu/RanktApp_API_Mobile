using System.Threading.Tasks;
using DataModel.Base;

namespace TrakkerApp.Api.Repositories.Categories
{
    public interface ICategoryRepository
    {
        Task<Category> GetBySimpleName(string name);
    }
}