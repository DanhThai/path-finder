using System.Linq.Expressions;

namespace Common.Domain
{
    public class TableQueryParameter<T>
    {
        public Pager Pager { get; set; } = new Pager();
        public Expression<Func<T, bool>> Filter { get; set; }
        public Sorter<T, object> Sorter { get; set; } = new Sorter<T, object>();
        public Func<IQueryable<T>, IQueryable<T>> CustomQuerable = null;
    }

    public class Pager
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public class Sorter<T, TResult>
    {
        public bool IsAscending { get; set; }
        public Expression<Func<T, TResult>> SortBy { get; set; }
        public Expression<Func<T, TResult>> ThenSortBy { get; set; }
    }
}
