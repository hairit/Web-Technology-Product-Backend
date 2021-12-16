using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Laptop_store_e_comerce.Controllers
{
    [Route("data/[controller]")]
    [ApiController]
    public class PostAnhController : ControllerBase
    {
        private readonly IWebHostEnvironment hostEnvironment;
        public PostAnhController(IWebHostEnvironment hostEnvironment)
        {
            this.hostEnvironment = hostEnvironment;
        }
        [HttpPost]
        public async Task<ActionResult> postImage([FromForm]Image image)
        {
            var a = image.idProduct;
            //string imageName = new String(Path.GetFileNameWithoutExtension(image.file.FileName).Take(10).ToArray()).Replace(' ', '-');
            string imageName = new String(Path.GetFileName(image.file.FileName));
            //imageName = imageName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(image.file.FileName);
            var imagePath = Path.Combine(hostEnvironment.ContentRootPath, "Images/Panels", imageName);
            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            { await image.file.CopyToAsync(fileStream); }
            return NoContent();
        }
        public class Image
        {
            public string idProduct { get; set; }
            public IFormFile file { get; set; }
    }
    }
}
