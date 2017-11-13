using System.Data.SqlClient;
using System.Threading.Tasks;
using DataModel.Base;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Rankt.Api.Repositories
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly string ConnectionString;
        protected readonly IConfiguration Configuration;
        protected readonly IMemoryCache MemoryCache;


        protected BaseRepository(IConfiguration configuration, IMemoryCache memoryCache)
        {
            Configuration = configuration;
            ConnectionString = configuration["ConnectionStrings:DefaultConnection"];
            MemoryCache = memoryCache;
        }

        protected SqlConnection GetConnection()
        {
            var connection = new SqlConnection(ConnectionString);
            return connection;
        }

        protected async Task<SqlConnection> GetOpenConnection()
        {
            var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();
            return connection;
        }

        public abstract Task<T> GetById(long id);

        public abstract Task<BaseError> Create(T entity);
        public abstract Task<BaseError> Update(T entity);

        public virtual Task<BaseError> BeforeCreate(T entity)
        {
            return null;
        }

        public virtual Task<BaseError> AfterCreate(T entity)
        {
            return null;
        }

        public virtual Task<BaseError> BeforeUpdate(T entity)
        {
            return null;
        }

        public virtual Task<BaseError> AfterUpdate(T entity)
        {
            return null;
        }

        public async Task<BaseError> Save(T entity)
        {
            BaseError error;

            //beforeSave
            if (entity.GetId() != 0)
            {
                //before update
                error = await Update(entity);
                if (error.ErrorCode == BaseError.Fail)
                {
                    return error;
                }
                //after update
            }
            else
            {
                //before create
                error = await Create(entity);
                if (error.ErrorCode == BaseError.Fail)
                {
                    return error;
                }
                //after create
            }
            //after save

            return error;
        }
    }
    public interface IBaseRepository<T>
    {
        Task<BaseError> BeforeCreate(T entity);
        Task<BaseError> Create(T entity);
        Task<BaseError> AfterCreate(T entity);
        Task<BaseError> BeforeUpdate(T entity);
        Task<BaseError> Update(T entity);
        Task<BaseError> AfterUpdate(T entity);
        Task<BaseError> Save(T entity);

        Task<T> GetById(long id);
    }


}
