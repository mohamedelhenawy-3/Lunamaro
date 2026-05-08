namespace Lunamaroapi.Services.Interfaces
{
    public interface IImageServices
    {
        public Task<string> UploadImage(IFormFile file);
       public Task DeleteImage(string imageUrl);  // ← add this
    }
}
