using AutoMapper;
using AutoMapper.QueryableExtensions;
using Library.API.Helpers;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Controllers
{
    [Route("api/authors")]
    public class AuthorsController : Controller
    {
        private ILibraryRepository _libraryRepository;

        public AuthorsController(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        public IActionResult GetAuthors()
        {
            var authors = _libraryRepository.GetAuthors();
            var authorsDto = Mapper.Map<IEnumerable<AuthorDto>>(authors);

            return new JsonResult(authorsDto);
        }
    }
}
