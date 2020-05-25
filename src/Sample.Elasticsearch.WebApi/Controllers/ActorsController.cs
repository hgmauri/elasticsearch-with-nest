using Microsoft.AspNetCore.Mvc;
using Sample.Elasticsearch.Domain.Concrete;

namespace Sample.Elasticsearch.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ActorsController : Controller
    {
        private readonly IActorsApplication _actorsApplication;

        public ActorsController(IActorsApplication actorsApplication)
        {
            _actorsApplication = actorsApplication;
        }

        [HttpPost("postsampledata")]
        public IActionResult PostSampleData()
        {
            _actorsApplication.PostActorsSample();

            return Ok(new { Result = "Data successfully registered with Elasticsearch" });
        }

        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            var result = _actorsApplication.GetAll();

            return Json(result);
        }

        [HttpGet("getbyname")]
        public IActionResult GetByAll([FromQuery] string name)
        {
            var result = _actorsApplication.GetByName(name);

            return Json(result);
        }
    }
}
