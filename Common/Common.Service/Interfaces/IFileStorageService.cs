using Common.Domain;
using Microsoft.AspNetCore.Http;

namespace Common.Service
{
    public interface IFileStorageService : IScopeDependency
    {
        Task<DocumentProperty> UploadFile(IFormFile file, StorageCategory category);
        Task<bool> SaveDocumentFile(DocumentProperty file, Guid resourceId);
        Task<bool> SaveDocumentFiles(List<DocumentProperty> files, Guid resourceId);
        Task<DocumentProperty> GetDocumentFile(Guid fileId);
        Task<bool> DeleteDocumentFiles(List<Guid> fileIds);
    }
}