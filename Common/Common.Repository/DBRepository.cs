using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using Common.Domain;
using Newtonsoft.Json;

namespace Common.Repository
{
    public class DBRepository : IDBRepository
    {
        private readonly ApplicationDBContext _context;

        public DBRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        #region Create
        private static void InitBaseEntity<T>(T entity) where T : class
        {
            if (entity is BaseEntity baseEntity)
            {
                baseEntity.CreatedAt = baseEntity.ModifiedAt = DateTimeOffset.UtcNow;
                baseEntity.CreatedBy = baseEntity.ModifiedBy = RuntimeContext.Current?.UserId ?? CommonConstants.SYSTEMACCOUNTID;
            }
        }

        public async Task<Guid> AddAsync<T>(T entity) where T : BaseEntity
        {
            InitBaseEntity(entity);
            var entry = await _context.AddAsync(entity);
            var result = await _context.SaveChangesAsync();
            return entry.Entity.Id;
        }

        public async Task<int> AddRangeAsync<T>(IEnumerable<T> entities) where T : BaseEntity
        {
            foreach (var entity in entities)
            {
                InitBaseEntity(entity);
            }
            await _context.Set<T>().AddRangeAsync(entities);
            var result = await _context.SaveChangesAsync();
            return result;
        }

        #endregion

        #region Update
        private static void UpdateBaseEntity<T>(T entity) where T : class
        {
            if (entity is BaseEntity baseEntity)
            {
                baseEntity.ModifiedAt = DateTimeOffset.UtcNow;
                baseEntity.ModifiedBy = RuntimeContext.Current?.UserId ?? CommonConstants.SYSTEMACCOUNTID;
            }
        }

        public async Task<bool> UpdateAsync<T>(T entity) where T : class
        {
            UpdateBaseEntity(entity);
            UpdateInternal(entity);
            var result = await _context.SaveChangesAsync();
            return result == 1;
        }

        private Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<T> UpdateInternal<T>(T entity) where T : class
        {
            var result = _context.Entry(entity);

            if (result.State == EntityState.Detached)
            {
                result = _context.Set<T>().Update(entity);
            }

            return result;
        }

        public async Task<int> UpdateRangeAsync<T>(IEnumerable<T> entities) where T : class
        {
            foreach (var entity in entities)
            {
                UpdateBaseEntity(entity);
                UpdateInternal(entity);
            }
            var result = await _context.SaveChangesAsync();
            return result;
        }

        public async Task<int> BulkUpdateAsync<S, T>(IEnumerable<S> entities, Expression<Func<T, S, bool>> joinCondition, Expression<Func<Tuple<S, T>, T>> setter)
            where S : class
            where T : class
        {
            //foreach (var entity in entities)
            //{
            //    UpdateBaseEntity(entity);
            //}
            //var result = await _linq2Db.BulkUpdateAsync(entities, joinCondition, setter);
            //return result;
            throw new NotImplementedException();
        }

        public async Task<int> BulkUpdateAsync<T>(Expression<Func<T, bool>> predicate, Dictionary<Expression<Func<T, object>>, Expression<Func<object>>> updates) where T : BaseEntity
        {

            //if (updates.Count == 0)
            //{
            //    return 0;
            //}
            //var userId = RuntimeContext.Current?.UserId ?? CoreConstants.SYSTEMACCOUNTID;
            //updates[s => s.ModifiedAt] = () => DateTimeOffset.UtcNow;
            //updates[s => s.ModifiedBy] = () => userId;
            //var result = await _linq2Db.BulkUpdateAsync(predicate, updates);
            //return result;
            throw new NotImplementedException();
        }

        #endregion

        #region Delete
        public async Task<int> DeleteAsync<T>(T entity) where T : class
        {
            _context.Set<T>().Remove(entity);
            var result = await _context.SaveChangesAsync();
            //await RefreshCacheTicks<T>(true);
            return result;
        }

        public async Task<int> DeleteRangeAsync<T>(IEnumerable<T> entities) where T : class
        {
            _context.Set<T>().RemoveRange(entities);
            var result = await _context.SaveChangesAsync();
            //await RefreshCacheTicks<T>(true);
            return result;
        }

        #endregion

        #region Retrieve
        public async Task<T> FindForUpdateAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return await _context.Set<T>().AsTracking().FirstOrDefaultAsync(predicate);
        }

        public async Task<T> FindAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(predicate);
        }

        public async Task<List<T>> GetAsync<T>(Expression<Func<T, bool>> predicate = null) where T : class
        {
            if (predicate == null)
            {
                return await _context.Set<T>().ToListAsync();
            }
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<List<R>> GetAsync<T, R>(Expression<Func<T, R>> selector, Expression<Func<T, bool>> predicate = null) where T : class
        {
            if (predicate == null)
            {
                return await _context.Set<T>().Select(selector).ToListAsync();
            }
            return await _context.Set<T>().Where(predicate).Select(selector).ToListAsync();
        }

        public async Task<bool> AnyAsync<T>(Expression<Func<T, bool>> predicate = null) where T : class
        {
            if (predicate == null)
            {
                return await _context.Set<T>().AnyAsync();
            }
            return await _context.Set<T>().AnyAsync(predicate);
        }

        private async Task<(int PageCount, int TotalCount, IQueryable<T> ItemsQuery)> GetWithPagingInternalAsync<T>(TableQueryParameter<T> queryParameter, bool counting = true) where T : class
        {
            int pageCount = 0;
            int totalCount = 0;
            var dbSet = _context.Set<T>();
            int skipCount = (queryParameter.Pager.PageIndex - 1) * queryParameter.Pager.PageSize;
            IOrderedQueryable<T> allDatas;
            var set = queryParameter.Filter != null
                ? dbSet.Where<T>(queryParameter.Filter)
                : dbSet;
            if (queryParameter.CustomQuerable != null)
            {
                set = queryParameter.CustomQuerable(set);
            }
            if (queryParameter.Sorter.IsAscending)
            {
                allDatas = set.OrderBy(queryParameter.Sorter.SortBy);
                if (queryParameter.Sorter.ThenSortBy != null)
                {
                    allDatas = allDatas.ThenBy(queryParameter.Sorter.ThenSortBy);
                }
            }
            else
            {
                allDatas = set.OrderByDescending(queryParameter.Sorter.SortBy);
                if (queryParameter.Sorter.ThenSortBy != null)
                {
                    allDatas = allDatas.ThenByDescending(queryParameter.Sorter.ThenSortBy);
                }
            }

            if (counting)
            {
                var allCount = totalCount = await allDatas.CountAsync();
                if (allCount == 0)
                {
                    pageCount = 1;
                }
                else
                {
                    pageCount = allCount % queryParameter.Pager.PageSize == 0
                        ? (allCount / queryParameter.Pager.PageSize)
                        : (allCount / queryParameter.Pager.PageSize) + 1;
                }
            }
            IQueryable<T> query = skipCount == 0 && counting
                ? allDatas.Take(queryParameter.Pager.PageSize)
                : allDatas.Skip(skipCount).Take(queryParameter.Pager.PageSize);
            return new(pageCount, totalCount, query);
        }

        public async Task<List<R>> GetWithPagingAsyncWithoutCounting<T, R>(TableQueryParameter<T> queryParameter, Expression<Func<T, R>> selector) where T : class
        {
            var internalResult = await GetWithPagingInternalAsync(queryParameter, false);
            return await internalResult.ItemsQuery.Select(selector).ToListAsync();
        }

        public async Task<TableInfo<T>> GetWithPagingAsync<T>(TableQueryParameter<T> queryParameter) where T : class
        {
            TableInfo<T> result = new TableInfo<T>();
            var internalResult = await GetWithPagingInternalAsync(queryParameter);
            result.PageCount = internalResult.PageCount;
            result.TotalItems = internalResult.TotalCount;
            result.Items = await internalResult.ItemsQuery.ToListAsync();
            return result;
        }

        public async Task<TableInfo<R>> GetWithPagingAsync<T, R>(TableQueryParameter<T> queryParameter, Expression<Func<T, R>> selector) where T : class
        {
            TableInfo<R> result = new TableInfo<R>();
            var internalResult = await GetWithPagingInternalAsync(queryParameter);
            result.PageCount = internalResult.PageCount;
            result.TotalItems = internalResult.TotalCount;
            result.Items = await internalResult.ItemsQuery.Select(selector).ToListAsync();
            return result;
        }
        #endregion

        #region Query
        public IQueryable<T> GetSet<T>(Expression<Func<T, bool>> predicate = null) where T : class
        {
            if (predicate == null)
            {
                return _context.Set<T>();
            }
            return _context.Set<T>().Where(predicate);
        }

        public IQueryable<T> GetSetAsTracking<T>(Expression<Func<T, bool>> predicate = null) where T : class
        {
            if (predicate == null)
            {
                return _context.Set<T>().AsTracking();
            }
            return _context.Set<T>().Where(predicate).AsTracking();
        }

        #endregion

        #region Raw query
        public async Task<object> ExecuteScalar(string sql, List<DbParameter> commandParams = null, CommandType commandType = CommandType.Text)
        {
            var con = _context.Database.GetDbConnection();
            using (var command = con.CreateCommand())
            {
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                if (commandParams?.Any() == true)
                {
                    command.Parameters.AddRange(commandParams.ToArray());
                }
                command.CommandType = commandType;
                command.CommandText = sql;
                command.CommandTimeout = 300;
                var result = await command.ExecuteScalarAsync();
                return result;
            }
        }

        public async Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters)
        {
            return await _context.Database.ExecuteSqlRawAsync(sql, parameters);
        }
        public async Task<List<T>> ExecuteReaderAsync<T>(string sql, List<DbParameter> commandParams, CommandType commandType = CommandType.Text) where T : class, new()
        {
            var con = _context.Database.GetDbConnection();
            var dt = new DataTable();
            using (var cmd = con.CreateCommand())
            {
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                cmd.CommandType = commandType;
                cmd.CommandText = sql;
                if (commandParams != null)
                {
                    cmd.Parameters.AddRange(commandParams.ToArray());
                }
                var reader = await cmd.ExecuteReaderAsync();
                dt.Load(reader);
                reader.Close();
            }

            // Mapping data
            var props = typeof(T).GetProperties();
            var result = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                var t = new T();
                foreach (PropertyInfo p in props)
                {
                    if (dt.Columns.IndexOf(p.Name) >= 0 && row[p.Name] != DBNull.Value)
                    {
                        var columAttribute = (ColumnAttribute)Attribute.GetCustomAttribute(p, typeof(ColumnAttribute));
                        if (columAttribute?.TypeName == "jsonb")
                        {
                            p.SetValue(t, JsonConvert.DeserializeObject(row[p.Name].ToString(), p.PropertyType));
                        }
                        else
                        {
                            p.SetValue(t, row[p.Name]);
                        }
                    }
                }
                result.Add(t);
            }

            return result;
        }
        #endregion
        public async Task ActionInTransaction(Func<Task> action)
        {
            try
            {
                using (var trans = await _context.Database.BeginTransactionAsync())
                {
                    await action();
                    await trans.CommitAsync();
                }
            }
            finally
            {
            }
        }
    }
}
