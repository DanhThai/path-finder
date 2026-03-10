namespace Common.Domain
{
    public class CTableParameter
    {
        public Dictionary<string, List<string>> Filters { get; set; } = new Dictionary<string, List<string>>();
        public string SortKey { get; set; } = string.Empty;

        public bool IsAscending { get; set; }

        public string SearchContent { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

    }
}
