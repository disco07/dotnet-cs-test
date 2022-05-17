using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace quest_web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DefaultController : Controller
    {
        private readonly ILogger<DefaultController> _logger;
        public DefaultController(ILogger<DefaultController> logger)
        {
            _logger = logger;
        }

        [HttpGet("/testSuccess")]
        public ActionResult testSuccess()
        {
            return StatusCode(200, "success");
        }

        [HttpGet("/testNotFound/")]
        public ActionResult testNotFound()
        {
            return StatusCode(404, "not found");
        }

        [HttpGet("/testError")]
        public ActionResult testError()
        {
            return StatusCode(500, "error");
        }
    }
}