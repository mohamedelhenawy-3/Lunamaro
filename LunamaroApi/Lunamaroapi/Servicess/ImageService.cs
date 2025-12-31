using Lunamaroapi.Services.Interfaces;

namespace Lunamaroapi.Services
{
    public class ImageService : IImageServices
    {

        private readonly IWebHostEnvironment _env;


        public ImageService(IWebHostEnvironment env)
        {
            _env = env;
        }
        public async Task<string> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0) throw new ArgumentException("File is requirezd");


            var uploadsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(),"wwwroot"), "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);



            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);



            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return relative path or URL
            return $"/uploads/{uniqueFileName}";

        }
    }
}
