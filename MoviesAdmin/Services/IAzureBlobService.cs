using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MoviesAdmin.Services
{
    public interface IAzureBlobService
    {
        Task<string> UploadFileAsync(IFormFile file, string filename); // Uploads a file to Azure Blob Storage and returns the URL of the uploaded file
    }
}
