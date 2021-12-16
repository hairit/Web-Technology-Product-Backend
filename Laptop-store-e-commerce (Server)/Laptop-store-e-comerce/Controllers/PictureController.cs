using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Laptop_store_e_comerce.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Laptop_store_e_comerce.Controllers
{
    [Route("data/[controller]")]
    [ApiController]
    public class PictureController : ControllerBase
    {
        private readonly IWebHostEnvironment hostEnvironment;
        private readonly StoreContext database;
        public PictureController(IWebHostEnvironment hostEnvironment, StoreContext _context)
        {
            this.hostEnvironment = hostEnvironment;
            this.database = _context;
        }
        [HttpPost]
        public async Task<ActionResult<Product>> postImage([FromForm] Image image)
        {
            var idPro = image.idProduct;
            if (!database.Products.Any(pro => pro.Id == image.idProduct)) return NotFound();
            try
            {
                var pro = await database.Products.FirstOrDefaultAsync(pro => pro.Id == image.idProduct);
                pro.Nameimage = new String(Path.GetFileName(image.file.FileName));
                database.Entry(pro).State = EntityState.Modified;
                database.SaveChangesAsync();
                return await saveImage(image.file);
            }
            catch (Exception) { return BadRequest(); }
        }
        public async Task<ActionResult> saveImage(IFormFile imageFile)
        {
            //string imageName = new String(Path.GetFileNameWithoutExtension(image.file.FileName).Take(10).ToArray()).Replace(' ', '-');
            string imageName = new String(Path.GetFileName(imageFile.FileName));
            //imageName = imageName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(image.file.FileName);
            var imagePath = Path.Combine(hostEnvironment.ContentRootPath, "Images/Products", imageName);
            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            { await imageFile.CopyToAsync(fileStream); }
            return NoContent();
        }
        public class Image
        {
            public string idProduct { get; set; }
            public IFormFile file { get; set; }
        }
    }
}
