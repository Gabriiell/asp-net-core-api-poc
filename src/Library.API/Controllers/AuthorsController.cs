using Library.API.Helpers;
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
            var authors = _libraryRepository.GetAuthors()
                .Select(a => new AuthorDto
                {
                    Id = a.Id,
                    Name = $"{a.FirstName} {a.LastName}",
                    Age = a.DateOfBirth.GetCurrentAge(),
                    Genre = a.Genre
                });

            return new JsonResult(authors);
        }
    }
}
