using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using DataModel.Base;
using Microsoft.Extensions.Caching.Memory;
using Rankt.Api.Repositories;
using Trakker.Api.Repositories;

namespace TrakkerApp.Api.Repositories.Categories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        private const string TABLE_NAME = "dbo.tblCategory";
        private const string ID_FIELD_NAME = "ID";
        private const string FIELD_NAME = "Name";
        private const string FIELD_SIMPLE_NAME = "SimpleName";
        private const string FIELD_STATUS = "Status";

        private const string ALL_FIELDS =
            TABLE_NAME + "." + ID_FIELD_NAME + ", " +
            TABLE_NAME + "." + FIELD_NAME + ", " +
            TABLE_NAME + "." + FIELD_SIMPLE_NAME + ", " +
            TABLE_NAME + "." + FIELD_STATUS + " ";

        private SqlConnection _connection;

        public CategoryRepository(IConfiguration configuration, IMemoryCache memoryCache) : base(configuration, memoryCache)
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

        public override Task<Category> GetById(long id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Category> GetBySimpleName(string name)
        {
            Category category = null;
            try
            {
                _connection = await GetOpenConnection();

                var sqlQuery = GetSingleBasicSelectSql() + " WHERE " +
                               TABLE_NAME + "." + FIELD_NAME + " = @passedName";

                var command = new SqlCommand(sqlQuery, _connection);
                command.Parameters.Add(new SqlParameter("@passedName", name));
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    category = Category.InstanciateFromReader(
                        Int32.Parse(reader[ID_FIELD_NAME].ToString()),
                        reader[FIELD_NAME].ToString(),
                        reader[FIELD_SIMPLE_NAME].ToString(),
                        reader[FIELD_STATUS].ToString());
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                _connection.Close();
            }
            return category;
        }

        public override async Task<BaseError> Create(Category entity)
        {
            try
            {
                _connection = await GetOpenConnection();

                const string insertSql = "INSERT INTO " + TABLE_NAME +
                                         " (" + FIELD_NAME +
                                         ", " + FIELD_SIMPLE_NAME +
                                         ", " + FIELD_STATUS + ") OUTPUT INSERTED.ID VALUES " +
                                         "(@name, @simpleName, @status )";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@name", entity.Name),
                    new SqlParameter("@simpleName", entity.SimpleName),
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
            return entity.GetId() == 0 ? new BaseError(BaseError.Fail) : new BaseError(BaseError.Success);
        }

        public override Task<BaseError> Update(Category entity)
        {
            throw new System.NotImplementedException();
        }

        
    }
}