
namespace Common.Domain
{
    public static class FileHelper
    {
        public static string GenerateFileName(string originalFileName)
        {
            var fileName = Path.GetFileNameWithoutExtension(originalFileName);
            var extension = Path.GetExtension(originalFileName);

            return $"{fileName}_{DateTime.UtcNow:yyyyMMddHHmmss}{extension}";
        }
    }
}
