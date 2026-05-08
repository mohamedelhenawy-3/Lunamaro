using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Lunamaroapi.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace Lunamaroapi.Services.Implements
{
    public class ImageService : IImageServices
    {
        private readonly Cloudinary _cloudinary;

        public ImageService(IConfiguration configuration)
        {
            var account = new Account(
            configuration["Cloudinary:CloudName"],
            configuration["Cloudinary:ApiKey"],
            configuration["Cloudinary:ApiSecret"]
        );
            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true; // always use https
        }

   
        public async Task<string> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is required.");

            // Step 1 — Resize and compress with ImageSharp before uploading
            using var inputStream = file.OpenReadStream();
            using var image = await Image.LoadAsync(inputStream);

            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new SixLabors.ImageSharp.Size(800, 0),  // max width 800px, height auto
                Mode = ResizeMode.Max
            }));

            // Step 2 — Save compressed image to memory stream as WebP
            using var outputStream = new MemoryStream();
            await image.SaveAsync(outputStream, new WebpEncoder { Quality = 75 });
            outputStream.Position = 0;

            // Step 3 — Upload to Cloudinary
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, outputStream),
                Folder = "lunamaro/items",   // organised in Cloudinary folder
                Transformation = new Transformation()
                    .Quality("auto")                 // Cloudinary auto-optimises
                    .FetchFormat("auto"),            // serves WebP to modern browsers
                Overwrite = false
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
                throw new Exception($"Cloudinary upload failed: {uploadResult.Error.Message}");

            // Returns the CDN URL — e.g. https://res.cloudinary.com/yourcloud/image/upload/...
            return uploadResult.SecureUrl.ToString();
        }

        public async Task DeleteImage(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;

            // Extract public_id from URL
            // URL format: https://res.cloudinary.com/{cloud}/image/upload/v123/{folder}/{publicId}.webp
            var publicId = ExtractPublicId(imageUrl);
            if (string.IsNullOrEmpty(publicId)) return;

            var deleteParams = new DeletionParams(publicId);
            await _cloudinary.DestroyAsync(deleteParams);
        }
        private string ExtractPublicId(string url)
        {
            try
            {
                var parts = url.Split("/upload/");
                if (parts.Length < 2) return string.Empty;

                var afterUpload = parts[1];

                if (afterUpload.StartsWith("v") && afterUpload.Contains("/"))
                {
                    var slashIndex = afterUpload.IndexOf('/');
                    afterUpload = afterUpload.Substring(slashIndex + 1);
                }

                // Remove file extension
                var dotIndex = afterUpload.LastIndexOf('.');
                if (dotIndex > 0)
                    afterUpload = afterUpload.Substring(0, dotIndex);

                return afterUpload; // e.g. "lunamaro/items/abc123"
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}