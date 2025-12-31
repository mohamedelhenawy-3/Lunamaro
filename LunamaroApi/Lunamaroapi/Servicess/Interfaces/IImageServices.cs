namespace Lunamaroapi.Services.Interfaces
{
    public interface IImageServices
    {
        public Task<string> UploadImage(IFormFile file);
    }
}
