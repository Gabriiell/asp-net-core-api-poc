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
        private readonly ILibraryRepository _libraryRepository;
        private readonly IMapper _mapper;

        public AuthorsController(ILibraryRepository libraryRepository, IMapper mapper)
        {
            _libraryRepository = libraryRepository;
            _mapper = mapper;
        }

        public IActionResult GetAuthors()
        {
            var authors = _libraryRepository.GetAuthors();
            var authorsDto = _mapper.Map<IEnumerable<AuthorDto>>(authors);

            return new JsonResult(authorsDto);
        }
    }
}
