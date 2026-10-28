using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
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

            containerName = _configuration["AzureStorage:ContainerName"]
                ?? throw new InvalidOperationException("Missing configuration: AzureStorage:ContainerName");

            string connectionString = _configuration["AzureStorage:ConnectionString"]
                ?? throw new InvalidOperationException("Missing configuration: AzureStorage:ConnectionString");
            
            _blobServiceClient = new BlobServiceClient(connectionString);
        }
        public async Task<string> UploadFileAsync(IFormFile file, string filename)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(filename);

            var options = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = file.ContentType
                }
            };

            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, overwrite: true);

            return blobClient.Uri.ToString();
        }
    }
}
