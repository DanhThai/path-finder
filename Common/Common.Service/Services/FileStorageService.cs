using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using Common.Domain;
using Common.Repository;

namespace Common.Service
{
    public class FileStorageService : IFileStorageService
    {
        private List<string> AllowedExtensions = new List<string> { ".jpg", ".png", ".pdf",".doc", ".docx", };
        private readonly Cloudinary _cloudinary;
        private readonly IDBRepository _repository;
        public FileStorageService(IDBRepository repository)
        {
            var cloudinaryConfig = RuntimeContext.Config.Storage.Cloudinary;
            var account = new Account(
                cloudinaryConfig.CloudName,
                cloudinaryConfig.ApiKey,
                cloudinaryConfig.ApiSecret
            );

            _cloudinary = new Cloudinary(account);
            _repository = repository;
        }

        public async Task<DocumentProperty> UploadFile(IFormFile file, StorageCategory category)
        {
            ValidateFile(file);

            await using var stream = file.OpenReadStream();
            var fileName = FileHelper.GenerateFileName(file.FileName);
            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(fileName, stream),
                Folder = $"PathFinder/{category.ToString()}",
                PublicId = fileName
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new WarningHandleException(uploadResult.Error.Message);
            }

            var document = new StorageDocumentEntity
            {
                Id = Guid.NewGuid(),
                Category = category,
                FileName = uploadResult.DisplayName,
                PublicID = uploadResult.PublicId,
                URL = uploadResult.SecureUrl.ToString(),
            };

            await _repository.AddAsync(document);

            var result = new DocumentProperty
            {
                Id = document.Id,
                FileName = document.FileName,
                URL = document.URL,
            };
            return result;
        }

        public async Task<bool> SaveDocumentFile(DocumentProperty file, Guid resourceId)
        {
            var document = await _repository.FindAsync<StorageDocumentEntity>(p => p.Id == file.Id) ?? throw new NotExistException("File");
            document.ResourceId = resourceId;

            await _repository.UpdateAsync(document);

            return true;
        }

        public async Task<bool> SaveDocumentFiles(List<DocumentProperty> files, Guid resourceId)
        {
            var documents = await _repository.GetAsync<StorageDocumentEntity>(p => files.Select(s => s.Id).Contains(p.Id));
            if (!documents.Any())
            {
                throw new NotExistException("File");
            }
            documents.ForEach(i => i.ResourceId = resourceId);

            await _repository.UpdateRangeAsync(documents);

            return true;
        }

        public async Task<DocumentProperty> GetDocumentFile(Guid fileId)
        {
            var document = await _repository.FindAsync<StorageDocumentEntity>(p => p.Id == fileId) ?? throw new NotExistException("File");

            return new DocumentProperty
            {
                Id = fileId,
                FileName = document.FileName,
                URL = document.URL,
            };
        }

        public async Task<bool> DeleteDocumentFiles(List<Guid> fileIds)
        {
            var documents = await _repository.GetAsync<StorageDocumentEntity>(p => fileIds.Contains(p.Id));

            foreach (var document in documents)
            {
                var deleteParams = new DeletionParams(document.PublicID);
                await _cloudinary.DestroyAsync(deleteParams);
            }

            await _repository.DeleteAsync(documents);

            return true;
        }

        #region Private function
        private void ValidateFile(IFormFile file)
        {
            ArgumentNullException.ThrowIfNull(file);

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!AllowedExtensions.Contains(extension))
            {
                throw new InvalidDataException($"File type {extension} is not allowed.");
            }

            if (file.Length > 100 * 1024 * 1024)
            {
                throw new WarningHandleException("File too large");
            }
        }
        #endregion
    }
}
