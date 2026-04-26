using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Lunamaroapi.Services.Interfaces;

namespace Lunamaroapi.Services.Implements
{
    public class ImageService : IImageServices
    {
        private readonly string _connectionString;
        private readonly string _containerName = "uploads";

        public ImageService(IConfiguration configuration)
        {
            // This looks in every possible place Azure might store the key
            _connectionString = configuration["AzureStorage:ConnectionString"]
                             ?? configuration["AzureStorage__ConnectionString"]
                             ?? configuration.GetConnectionString("AzureStorage")
                             ?? Environment.GetEnvironmentVariable("AzureStorage__ConnectionString");

            if (string.IsNullOrEmpty(_connectionString))
            {
                
                throw new Exception("CRITICAL: Azure Storage Connection String is missing from ALL sources.");
            }
        }

        public async Task<string> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is required.");

            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient("$web");

            var extension = Path.GetExtension(file.FileName);
            var cleanFileName = $"{Guid.NewGuid()}{extension}";
            var blobPath = $"uploads/{cleanFileName}";

            var blobClient = containerClient.GetBlobClient(blobPath);

            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, new BlobHttpHeaders
            {
                ContentType = file.ContentType
            });

           
            string finalUrl = blobClient.Uri.AbsoluteUri
                .Replace(".blob.core.windows.net/$web", ".z1.web.core.windows.net");

            return finalUrl;
        }
    }
}