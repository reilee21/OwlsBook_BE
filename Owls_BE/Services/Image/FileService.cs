namespace Owls_BE.Services.Image
{
    public class FileService : IFileService
    {
        public IWebHostEnvironment hostEnvironment;

        public FileService(IWebHostEnvironment hostEnvironment)
        {
            this.hostEnvironment = hostEnvironment;
        }

        public void DeleteImage(string name)
        {
            if (name != null)
            {
                var imagePath = Path.Combine(hostEnvironment.WebRootPath, "images", name);
                if (File.Exists(imagePath))
                {
                    try
                    {
                        File.Delete(imagePath);
                    }
                    catch (IOException ex)
                    {

                        Console.WriteLine($"Error deleting image: {ex.Message}");
                    }
                }
            }
        }

        public async Task<byte[]> GetImageAsync(string name)
        {
            var imagePath = Path.Combine(hostEnvironment.WebRootPath, "images", name);
            if (File.Exists(imagePath))
            {
                byte[] imageBytes = File.ReadAllBytes(imagePath);
                return imageBytes;
            }

            return null;
        }

        public async Task<string> SaveImageAsync(IFormFile file, string name)
        {
            if (file != null && file.Length > 0)
            {
                var uploads = Path.Combine(hostEnvironment.WebRootPath, "images");
                var fileName = $"{name}.jpg";

                var filePath = Path.Combine(uploads, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                return filePath;
            }
            return String.Empty;
        }
        public async Task<string> SaveImageAsync(string base64, string name)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(base64);

                var uploads = Path.Combine(hostEnvironment.WebRootPath, "images");
                var fileName = $"{name}.jpg";

                var filePath = Path.Combine(uploads, fileName);

                await using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await fileStream.WriteAsync(bytes);
                }

                return fileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi lưu trữ ảnh: {ex.Message}");
                throw;
            }
        }
    }
}
