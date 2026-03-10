namespace Common.Domain
{
    public static class SearchBuilder
    {
        public static (string Pattern, string EscapeCharacter) BuildContent(string searchContent)
        {
            if (string.IsNullOrWhiteSpace(searchContent)) return (string.Empty, string.Empty);

            string escapeCharacter = "";
            searchContent = searchContent.Trim();
            if (searchContent.Contains("%") || searchContent.Contains("_"))
            {
                escapeCharacter = "\\";
            }
            searchContent = searchContent
                .Replace("\\", "\\\\")
                .Replace("_", "\\_")
                .Replace("%", "\\%");

            var pattern = $"%{searchContent}%";

            return (pattern, escapeCharacter);
        }
    }
}
