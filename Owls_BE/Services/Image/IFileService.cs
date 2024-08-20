namespace Owls_BE.Services.Image
{
    public interface IFileService
    {
        Task<string> SaveImageAsync(IFormFile file, string name);
        Task<string> SaveImageAsync(string base64, string name);

        void DeleteImage(string name);
        Task<byte[]> GetImageAsync(string name);

    }
}
