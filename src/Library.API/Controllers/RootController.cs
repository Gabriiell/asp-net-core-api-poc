using AutoMapper;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.API.Controllers
{
    [Route("api")]
    public class RootController : Controller
    {
        private readonly IUrlHelper _urlHelper;

        public RootController(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        [HttpGet(Name = "GetRoot")]
        public IActionResult GetRoot([FromHeader(Name = "Accept")] string mediaType)
        {
            if (mediaType != "application/vnd.gabriel.hateoas+json")
            {
                return NoContent();
            }

            var links = new List<LinkDto>();

            links.Add(new LinkDto(
                _urlHelper.Link("GetRoot", new { }),
                "self",
                "GET"
            ));

            links.Add(new LinkDto(
                _urlHelper.Link("GetAuthors", new { }),
                "authors",
                "GET"
            ));

            links.Add(new LinkDto(
                _urlHelper.Link("CreateAuthor", new { }),
                "create_author",
                "POST"
            ));

            return Ok(links);
        }
    }
}
