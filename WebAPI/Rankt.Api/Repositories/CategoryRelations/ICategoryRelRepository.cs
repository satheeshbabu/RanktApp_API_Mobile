using System.Threading.Tasks;
using DataModel.Base;

namespace TrakkerApp.Api.Repositories.CategoryRelations
{
    public interface ICategoryRelRepository
    {
        Task<CategoryRel> CreateRelFromCategories(Category parentCategory, Category childCategory);
    }
}