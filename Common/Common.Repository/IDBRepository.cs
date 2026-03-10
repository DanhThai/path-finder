using System.Data.Common;
using System.Data;
using System.Linq.Expressions;
using Common.Domain;

namespace Common.Repository
{
    public interface IDBRepository
    {
        #region Create
        Task<Guid> AddAsync<T>(T entity) where T : BaseEntity;
        Task<int> AddRangeAsync<T>(IEnumerable<T> entities) where T : BaseEntity;
        #endregion

        #region Update
        Task<bool> UpdateAsync<T>(T entity) where T : class;
        Task<int> UpdateRangeAsync<T>(IEnumerable<T> entities) where T : class;

        Task<int> BulkUpdateAsync<T>(Expression<Func<T, bool>> predicate, Dictionary<Expression<Func<T, object>>, Expression<Func<object>>> updates) where T : BaseEntity;
        #endregion

        #region Delete
        Task<int> DeleteAsync<T>(T entity) where T : class;
        Task<int> DeleteRangeAsync<T>(IEnumerable<T> entities) where T : class;

        #endregion

        #region Retrieve
        Task<List<T>> GetAsync<T>(Expression<Func<T, bool>> predicate = null) where T : class;
        Task<List<R>> GetAsync<T, R>(Expression<Func<T, R>> selector, Expression<Func<T, bool>> predicate = null) where T : class;
        Task<T> FindForUpdateAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
        Task<T> FindAsync<T>(Expression<Func<T, bool>> predicate) where T : class;

        Task<bool> AnyAsync<T>(Expression<Func<T, bool>> predicate = null) where T : class;
        Task<TableInfo<T>> GetWithPagingAsync<T>(TableQueryParameter<T> queryParameter) where T : class;
        Task<TableInfo<R>> GetWithPagingAsync<T, R>(TableQueryParameter<T> queryParameter, Expression<Func<T, R>> selector) where T : class;
        #endregion

        #region Query
        IQueryable<T> GetSet<T>(Expression<Func<T, bool>> predicate = null) where T : class;
        IQueryable<T> GetSetAsTracking<T>(Expression<Func<T, bool>> predicate = null) where T : class;
        #endregion

        #region Raw query
        Task<object> ExecuteScalar(string sql, List<DbParameter> commandParams = null, CommandType commandType = CommandType.Text);
        Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters);
        Task<List<T>> ExecuteReaderAsync<T>(string sql, List<DbParameter> commandParams = null, CommandType commandType = CommandType.Text)
            where T : class, new();
        #endregion

        Task ActionInTransaction(Func<Task> action);
    }
}
