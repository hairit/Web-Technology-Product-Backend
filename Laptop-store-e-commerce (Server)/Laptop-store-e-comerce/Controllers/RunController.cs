using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Laptop_store_e_comerce.Controllers
{
    [Route("server/[controller]")]
    [ApiController]
    public class RunController : ControllerBase
    {
        [HttpGet("")]
        public string statusServer()
        {
            return "running";
        }
    }
}
