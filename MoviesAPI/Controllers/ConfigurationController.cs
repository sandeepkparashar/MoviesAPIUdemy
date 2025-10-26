using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/configuration")]
    public class ConfigurationController:ControllerBase
    {
        private readonly IConfiguration configuration;

        public ConfigurationController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(configuration["lastname"]);
        }
    }
}
