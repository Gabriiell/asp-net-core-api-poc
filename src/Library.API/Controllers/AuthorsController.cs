using AutoMapper;
using Library.API.Entities;
using Library.API.Helpers;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Library.API.Controllers
{
    [Route("api/authors")]
    public class AuthorsController : Controller
    {
        private readonly ILibraryRepository _libraryRepository;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;
        private readonly IPropertyMappingService _propertyMappingService;

        public AuthorsController(ILibraryRepository libraryRepository, IMapper mapper, IUrlHelper urlHelper, IPropertyMappingService propertyMappingService)
        {
            _libraryRepository = libraryRepository;
            _mapper = mapper;
            _urlHelper = urlHelper;
            _propertyMappingService = propertyMappingService;
        }

        [HttpGet(Name = "GetAuthors")]
        public IActionResult GetAuthors(AuthorsResourceParameters authorsResourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<AuthorDto, Author>(authorsResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var authors = _libraryRepository.GetAuthors(authorsResourceParameters);

            string previousPageLink = null;
            string nextPageLink = null;

            if (authors.HasPrevious)
            {
                previousPageLink = _urlHelper.Link("GetAuthors", new
                {
                    orderBy = authorsResourceParameters.OrderBy,
                    genre = authorsResourceParameters.Genre,
                    search = authorsResourceParameters.Search,
                    pageNumber = authors.CurrentPage - 1,
                    pageSize = authors.PageSize
                });
            }

            if (authors.HasNext)
            {
                nextPageLink = _urlHelper.Link("GetAuthors", new
                {
                    orderBy = authorsResourceParameters.OrderBy,
                    genre = authorsResourceParameters.Genre,
                    search = authorsResourceParameters.Search,
                    pageNumber = authors.CurrentPage + 1,
                    pageSize = authors.PageSize
                });
            }

            var paginationMetadata = new
            {
                totalCount = authors.TotalCount,
                pageSize = authors.PageSize,
                currentPage = authors.CurrentPage,
                totalPages = authors.TotalPages,
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));

            var authorsDto = _mapper.Map<IEnumerable<AuthorDto>>(authors);
            return Ok(authorsDto);
        }

        [HttpGet("{id}", Name = "GetAuthor")]
        public IActionResult GetAuthor(Guid id)
        {
            var author = _libraryRepository.GetAuthor(id);

            if (author == null)
            {
                return NotFound();
            }

            var authorDto = _mapper.Map<AuthorDto>(author);

            return Ok(authorDto);
        }

        [HttpPost]
        public IActionResult CreateAuthor([FromBody] AuthorForCreationDto author)
        {
            if (author == null)
            {
                return BadRequest();
            }

            var authorEntity = _mapper.Map<Author>(author);

            _libraryRepository.AddAuthor(authorEntity);

            if (!_libraryRepository.Save())
            {
                throw new Exception("Creating author failed on save.");
            }

            var authorDto = _mapper.Map<AuthorDto>(authorEntity);

            return CreatedAtRoute("GetAuthor", new { id = authorDto.Id }, authorDto);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAuthor(Guid id)
        {
            var author = _libraryRepository.GetAuthor(id);

            if (author == null)
            {
                return NotFound();
            }

            _libraryRepository.DeleteAuthor(author);

            if (!_libraryRepository.Save())
            {
                throw new Exception($"Deleting author {id} failed on save.");
            }

            return NoContent();
        }
    }
}
