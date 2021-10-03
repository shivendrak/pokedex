using Microsoft.AspNetCore.Mvc;

namespace Pokedex.Controllers
{
    
    [ApiController]
    public class HomeController
    {
        [Route("health")]
        [HttpGet]
        public bool Get()
        {
            return true;
        }
    }
}