
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace MoviesAPI.Services
{
    public class AzureStorageService : IFileStorageService
    {
        private readonly string connectionString;
        public AzureStorageService(IConfiguration configuration) {
            connectionString = configuration.GetConnectionString("AzureConnectionString");
        }
        public async Task DeleteFile(string fileRoute, string containerName)
        {
            if (fileRoute != null)
            {
                var client = new BlobServiceClient(connectionString);
                var containerClient = client.GetBlobContainerClient(containerName);
                var blobName = Path.GetFileName(fileRoute);
                var blobClient = containerClient.GetBlobClient(blobName);
                await blobClient.DeleteAsync(DeleteSnapshotsOption.None);
            }
        }

        public async Task<string> EditFile(byte[] content, string extension, string containerName, string fileRoute, string contentType)
        {
            await DeleteFile(fileRoute, containerName);
            return await SaveFile(content, extension,containerName,contentType);
        }

        public async Task<string> SaveFile(byte[] content, string extension, string containerName, string contentType)
        {
            var client= new BlobServiceClient(connectionString);
            var containerClient = client.GetBlobContainerClient(containerName);
            //var container = client.CreateBlobContainerAsync(containerName, PublicAccessType.BlobContainer);
            await containerClient.CreateIfNotExistsAsync();
            var fileName=$"{Guid.NewGuid()}{extension}";
            
            var blobClient=containerClient.GetBlobClient(fileName);
            using var stream = new MemoryStream(content);
            await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType });
            return blobClient.Uri.ToString();
        }
    }
}
