using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DataModel.Attributes;
using DataModel.Base;
using DataModel.Genres;
using DataModel.Movies;
using DataModel.Overall;
using DataModel.TVShows;
using TrakkerApp.Api.Repositories.Categories;
using TrakkerApp.Api.Repositories.CategoryRelations;

namespace TrakkerApp.Api.StartUp
{
    public class CategoryMapping
    {
        private static IEnumerable<Type> ClassesToPopulate()
        {
            return new List<Type>
            {
                typeof(Category),
                typeof(CategoryRel),
                typeof(Movie),
                typeof(MovieGenre),
                typeof(TVShow),
                typeof(TVShowGenre),
                typeof(Relation),
                typeof(MovieCollection),
                typeof(MediaList)
            };
        }

        private readonly CategoryRepository _categoryRepository;
        private readonly CategoryRelRepository _categoryRelRepository;
        public CategoryMapping(CategoryRepository categoryRepository, CategoryRelRepository categoryRelRepository)
        {
            _categoryRepository = categoryRepository;
            _categoryRelRepository = categoryRelRepository;
        }

        public async Task PopulateAllCategories()
        {
            await InjectAnnotedValues();
        }

        private async Task InjectAnnotedValues()
        {
            var counter = 0;
            foreach (var type in ClassesToPopulate())
            {
                foreach (var prop in type.GetProperties())
                {
                    if (!prop.GetCustomAttributes(false).OfType<CategoryAttribute>().Any()) continue;
                    var attribute = prop.GetCustomAttributes(false).OfType<CategoryAttribute>().FirstOrDefault();
                    if (attribute == null) continue;
                    var category = await _categoryRepository.GetBySimpleName(attribute.FieldName);

                    if (category == null)
                    {
                        category = await GenerateMissingCategory(attribute.FieldName, attribute.SimpleName);

                        if (attribute.ParentCategory != null)
                        {
                            var parentCategory =
                                await _categoryRepository.GetBySimpleName(attribute.ParentCategory);
                            await GenerateMissingCategoryRel(parentCategory, category);
                        }
                    }
                    //Console.WriteLine("Attribute name is " + attribute.FieldName);
                    prop.SetValue(type, category.GetId());
                    counter++;
//                    if ("ENTITY_CATEGORY_ID".Equals(prop.Name))
//                    {   
//                       Console.WriteLine("Class " + type.Name + " can be assigned to Base Entiry");
//                    }
                }
            }
            Console.WriteLine("Populated {0} categories", counter);
        }

        private async Task<Category> GenerateMissingCategory(string name, string simpleName)
        {
            var missingCategory = Category.Instanciate(name, simpleName, "ACTIVE");
            var error = await _categoryRepository.Save(missingCategory);

            if (error.ErrorCode == BaseError.Success)
            {
                Console.WriteLine("Created missing category " + missingCategory.Name);
                return missingCategory;
            }
            Console.WriteLine("An Error occured with adding + " + name);
            //TODO fix me
            throw new NotImplementedException();
        }

        private async Task GenerateMissingCategoryRel(Category parentCategory, Category childCategory)
        {
            await _categoryRelRepository.CreateRelFromCategories(parentCategory, childCategory);
        }
    }
}