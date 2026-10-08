using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace MoviesAdmin.Services
{
    public class AzureBlobService : IAzureBlobService
    {
        private readonly IConfiguration _configuration;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string containerName;

        public AzureBlobService(IConfiguration configuration)
        {
            _configuration = configuration;

            containerName = _configuration["AZ_STORAGE_CONTAINER_NAME"];
            string connectionString = _configuration.GetConnectionString("AZ_STORAGE_CONNECTION_STRING");

            _blobServiceClient = new BlobServiceClient(connectionString);
        }
        public async Task<string> UploadFileAsync(IFormFile file, string filename)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(filename);

            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, overwrite: true);

            return blobClient.Uri.ToString();
        }
    }
}
