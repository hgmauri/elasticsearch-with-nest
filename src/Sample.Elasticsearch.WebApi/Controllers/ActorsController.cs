using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Sample.Elasticsearch.Domain.Interfaces;

namespace Sample.Elasticsearch.WebApi.Controllers;

[Route("api/[controller]")]
public class ActorsController : Controller
{
    private readonly IActorsApplication _actorsApplication;

    public ActorsController(IActorsApplication actorsApplication)
    {
        _actorsApplication = actorsApplication;
    }

    [HttpPost("sample")]
    public async Task<IActionResult> PostSampleData()
    {
       await _actorsApplication.InsertManyAsync();

        return Ok(new { Result = "Data successfully registered with Elasticsearch" });
    }

    [HttpPost("exception")]
    public IActionResult PostException()
    {
        throw new Exception("Generate sample exception");
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _actorsApplication.GetAllAsync();

        return Json(result);
    }

    [HttpGet("name-match")]
    public async Task<IActionResult> GetByNameWithMatch([FromQuery] string name)
    {
        var result = await _actorsApplication.GetByNameWithMatch(name);

        return Json(result);
    }

    [HttpGet("name-multimatch")]
    public async Task<IActionResult> GetByNameAndDescriptionMultiMatch([FromQuery] string term)
    {
        var result = await _actorsApplication.GetByNameAndDescriptionMultiMatch(term);

        return Json(result);
    }

    [HttpGet("name-matchphrase")]
    public async Task<IActionResult> GetByNameWithMatchPhrase([FromQuery] string name)
    {
        var result = await _actorsApplication.GetByNameWithMatchPhrase(name);

        return Json(result);
    }

    [HttpGet("name-matchphraseprefix")]
    public async Task<IActionResult> GetByNameWithMatchPhrasePrefix([FromQuery] string name)
    {
        var result = await _actorsApplication.GetByNameWithMatchPhrasePrefix(name);

        return Json(result);
    }

    [HttpGet("name-term")]
    public async Task<IActionResult> GetByNameWithTerm([FromQuery] string name)
    {
        var result = await _actorsApplication.GetByNameWithTerm(name);

        return Json(result);
    }

    [HttpGet("name-wildcard")]
    public async Task<IActionResult> GetByNameWithWildcard([FromQuery] string name)
    {
        var result = await _actorsApplication.GetByNameWithWildcard(name);

        return Json(result);
    }

    [HttpGet("name-fuzzy")]
    public async Task<IActionResult> GetByNameWithFuzzy([FromQuery] string name)
    {
        var result = await _actorsApplication.GetByNameWithFuzzy(name);

        return Json(result);
    }

    [HttpGet("description-match")]
    public async Task<IActionResult> GetByDescriptionMatch([FromQuery] string description)
    {
        var result = await _actorsApplication.GetByDescriptionMatch(description);

        return Json(result);
    }

    [HttpGet("all-fields")]
    public async Task<IActionResult> SearchAllProperties([FromQuery] string term)
    {
        var result = await _actorsApplication.SearchInAllFiels(term);

        return Json(result);
    }

    [HttpGet("condiction")]
    public async Task<IActionResult> GetByCondictions([FromQuery] string name, [FromQuery] string description, [FromQuery] DateTime? birthdate)
    {
        var result = await _actorsApplication.GetActorsCondition(name, description, birthdate);

        return Json(result);
    }

    [HttpGet("term")]
    public async Task<IActionResult> GetByAllCondictions([FromQuery] string term)
    {
        var result = await _actorsApplication.GetActorsAllCondition(term);

        return Json(result);
    }

    [HttpGet("aggregation")]
    public async Task<IActionResult> GetActorsAggregation()
    {
        var result = await _actorsApplication.GetActorsAggregation();

        return Json(result);
    }
}