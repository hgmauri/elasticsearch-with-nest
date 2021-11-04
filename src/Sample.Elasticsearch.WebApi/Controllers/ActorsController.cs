using Microsoft.AspNetCore.Mvc;
using System;
using Sample.Elasticsearch.Domain.Interfaces;

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

        [HttpPost("sample")]
        public IActionResult PostSampleData()
        {
            _actorsApplication.InsertManyAsync();

            return Ok(new { Result = "Data successfully registered with Elasticsearch" });
        }

        [HttpPost("exception")]
        public IActionResult PostException()
        {
            throw new Exception("Generate sample exception");
        }

        [HttpGet("")]
        public IActionResult GetAll()
        {
            var result = _actorsApplication.GetAllAsync();

            return Json(result);
        }

        [HttpGet("name-match")]
        public IActionResult GetByNameWithMatch([FromQuery] string name)
        {
            var result = _actorsApplication.GetByNameWithMatch(name);

            return Json(result);
        }

        [HttpGet("name-multimatch")]
        public IActionResult GetByNameAndDescriptionMultiMatch([FromQuery] string term)
        {
            var result = _actorsApplication.GetByNameAndDescriptionMultiMatch(term);

            return Json(result);
        }

        [HttpGet("name-matchphrase")]
        public IActionResult GetByNameWithMatchPhrase([FromQuery] string name)
        {
            var result = _actorsApplication.GetByNameWithMatchPhrase(name);

            return Json(result);
        }

        [HttpGet("name-matchphraseprefix")]
        public IActionResult GetByNameWithMatchPhrasePrefix([FromQuery] string name)
        {
            var result = _actorsApplication.GetByNameWithMatchPhrasePrefix(name);

            return Json(result);
        }

        [HttpGet("name-term")]
        public IActionResult GetByNameWithTerm([FromQuery] string name)
        {
            var result = _actorsApplication.GetByNameWithTerm(name);

            return Json(result);
        }

        [HttpGet("name-wildcard")]
        public IActionResult GetByNameWithWildcard([FromQuery] string name)
        {
            var result = _actorsApplication.GetByNameWithWildcard(name);

            return Json(result);
        }

        [HttpGet("name-fuzzy")]
        public IActionResult GetByNameWithFuzzy([FromQuery] string name)
        {
            var result = _actorsApplication.GetByNameWithFuzzy(name);

            return Json(result);
        }

        [HttpGet("description-match")]
        public IActionResult GetByDescriptionMatch([FromQuery] string description)
        {
            var result = _actorsApplication.GetByDescriptionMatch(description);

            return Json(result);
        }

        [HttpGet("all-fields")]
        public IActionResult SearchAllProperties([FromQuery] string term)
        {
            var result = _actorsApplication.SearchInAllFiels(term);

            return Json(result);
        }

        [HttpGet("condiction")]
        public IActionResult GetByCondictions([FromQuery] string name, [FromQuery] string description, [FromQuery] DateTime? birthdate)
        {
            var result = _actorsApplication.GetActorsCondition(name, description, birthdate);

            return Json(result);
        }

        [HttpGet("term")]
        public IActionResult GetByAllCondictions([FromQuery] string term)
        {
            var result = _actorsApplication.GetActorsAllCondition(term);

            return Json(result);
        }

        [HttpGet("aggregation")]
        public IActionResult GetActorsAggregation()
        {
            var result = _actorsApplication.GetActorsAggregation();

            return Json(result);
        }
    }
}
