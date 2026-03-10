using Common.Domain;
using Common.Service;
using Microsoft.AspNetCore.Mvc;

namespace Server.API
{
    [ApiVersion("1")]
    public class StorageController : APIBaseController
    {
        private IFileStorageService _fileStorageService;
        public StorageController(IHttpContextAccessor accessor, IFileStorageService fileStorageService) : base(accessor)
        {
            _fileStorageService = fileStorageService;
        }

        [HttpPost("storage/upload")]
        public async Task<DocumentProperty> UploadFile(IFormFile file, StorageCategory category)
        {
            return await _fileStorageService.UploadFile(file, category);
        }

        [HttpGet("storage/file")]
        public async Task<DocumentProperty> GetDocumentFile(Guid fileId)
        {
            return await _fileStorageService.GetDocumentFile(fileId);
        }
    }
}
