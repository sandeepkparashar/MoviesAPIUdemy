using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace MoviesAPI.Services
{
    public class InAppStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment env;
        private readonly IHttpContextAccessor httpContextAccessor;

        public InAppStorageService(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            this.env = env;
            this.httpContextAccessor = httpContextAccessor;
        }
        public Task DeleteFile(string fileRoute, string containerName)
        {
            var fileName=Path.GetFileName(fileRoute);
            var fileDirectory=Path.Combine(env.WebRootPath,containerName,fileName);
            if (File.Exists(fileDirectory)) { 
                File.Delete(fileDirectory);
            }
            return Task.FromResult(0);
        }

        public async Task<string> EditFile(byte[] content, string extension, string containerName, string fileRoute, string contentType)
        {
            if (!string.IsNullOrEmpty(fileRoute))
            {
                await DeleteFile (fileRoute, containerName);
            }
            return await SaveFile (content, extension, containerName, contentType);
        }

        public async Task<string> SaveFile(byte[] content, string extension, string containerName, string contentType)
        {
            var fileName = $"{Guid.NewGuid()}{extension}";
            string folder = Path.Combine(env.WebRootPath, containerName);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            string savingPath = Path.Combine(folder, fileName);
            await File.WriteAllBytesAsync(savingPath, content);
            var currentUrl = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
            var pathForDatabase = Path.Combine(currentUrl,containerName,fileName);
            return pathForDatabase;
        }
    }
}
