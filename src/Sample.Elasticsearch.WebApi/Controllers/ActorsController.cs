﻿using Microsoft.AspNetCore.Mvc;
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
            _actorsApplication.PostActorsSample();

            return Ok(new { Result = "Data successfully registered with Elasticsearch" });
        }

        [HttpGet("")]
        public IActionResult GetAll()
        {
            var result = _actorsApplication.GetAll();

            return Json(result);
        }

        [HttpGet("name")]
        public IActionResult GetByName([FromQuery] string name)
        {
            var result = _actorsApplication.GetByName(name);

            return Json(result);
        }

        [HttpGet("description")]
        public IActionResult GetByDescription([FromQuery] string description)
        {
            var result = _actorsApplication.GetByDescription(description);

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
