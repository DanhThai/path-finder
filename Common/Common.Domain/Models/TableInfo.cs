
namespace Common.Domain
{
    public class TableInfo<T>
    {
        public List<T> Items { get; set; }
        public int PageCount { get; set; }
        public int TotalItems { get; set; }
    }
}