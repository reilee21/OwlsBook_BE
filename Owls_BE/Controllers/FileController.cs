using Microsoft.AspNetCore.Mvc;
using Owls_BE.Services.Image;

namespace Owls_BE.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileService fileService;

        public FileController(IFileService fileService)
        {
            this.fileService = fileService;
        }

        [HttpGet]
        public async Task<IActionResult> Image(string image)
        {
            byte[] img = await fileService.GetImageAsync(image);
            if (img == null) return NotFound();
            return File(img, "image/jpeg");
        }

        /* [HttpPost]
         public async Task<IActionResult> Upload([FromForm]FilesUpload files)
         {
             List<string> rs = new List<string>();
             for (var i = 0; i < files.Files.Count(); i++) 
             {
                 if (files.Files[i] == null) continue;
                 string fname = files.Name + "_" + i;
                 await fileService.SaveImageAsync(files.Files[i],fname);
                 rs.Add(fname);
             }
             return Ok(rs);
         }*/

    }
}
