using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using DataModel.Base;
using Microsoft.Extensions.Caching.Memory;
using Rankt.Api.Repositories;
using Trakker.Api.Repositories;

namespace TrakkerApp.Api.Repositories.CategoryRelations
{
    public class CategoryRelRepository : BaseRepository<CategoryRel>, ICategoryRelRepository
    {
        private const string TABLE_NAME = "dbo.tblCategoryRel";
        private const string ID_FIELD_NAME = "ID";
        private const string FIELD_PARENT_CAT = "Parent_Cat";
        private const string FIELD_CHILD_CAT = "Child_Cat";
        private const string FIELD_STATUS = "Status";

        private const string ALL_FIELDS =
            TABLE_NAME + "." + ID_FIELD_NAME + ", " +
            TABLE_NAME + "." + FIELD_PARENT_CAT + ", " +
            TABLE_NAME + "." + FIELD_CHILD_CAT + ", " +
            TABLE_NAME + "." + FIELD_STATUS + " ";

        private SqlConnection _connection;

        public CategoryRelRepository(IConfiguration configuration, IMemoryCache memoryCache) : base(configuration, memoryCache)
        {
        }

        public static string GetBasicSelectSql()
        {
            return "SELECT " + ALL_FIELDS + " FROM " + TABLE_NAME;
        }

        private static string GetSingleBasicSelectSql()
        {
            return "SELECT TOP 1 " + ALL_FIELDS + " FROM " + TABLE_NAME;
        }

        public override Task<CategoryRel> GetById(long id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<CategoryRel> CreateRelFromCategories(Category parentCategory, Category childCategory)
        {
            var rel = CategoryRel.Instanciate(parentCategory.GetId(), childCategory.GetId(), "ACTIVE");

            var error = await Save(rel);

            if (error.ErrorCode == BaseError.Success)
            {
                return rel;
            }
            throw new NotImplementedException();
        }

        public override async Task<BaseError> Create(CategoryRel entity)
        {
            try
            {
                _connection = await GetOpenConnection();

                const string insertSql = "INSERT INTO " + TABLE_NAME +
                                         " (" + FIELD_PARENT_CAT +
                                         ", " + FIELD_CHILD_CAT +
                                         ", " + FIELD_STATUS + ") OUTPUT INSERTED.ID VALUES " +
                                         "(@name, @simpleName, @status )";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@name", entity.ParentCatId),
                    new SqlParameter("@simpleName", entity.ChildCatId),
                    new SqlParameter("@status", entity.Status)
                };

                var command = new SqlCommand(insertSql, _connection);
                foreach (var sqlParameter in parameters)
                {
                    command.Parameters.Add(sqlParameter);
                }

                entity.SetId(Convert.ToInt32(await command.ExecuteScalarAsync()));

            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                _connection.Close();
            }
            return entity.GetId() == 0 ? new BaseError(BaseError.Fail) : new BaseError(BaseError.Success);
        }

        public override Task<BaseError> Update(CategoryRel entity)
        {
            throw new System.NotImplementedException();
        }

        
    }
}