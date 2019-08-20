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
    [Route("api/authors/{authorId}/books")]
    public class BooksController : Controller
    {
        private readonly ILibraryRepository _libraryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IUrlHelper _urlHelper;

        public BooksController(ILibraryRepository libraryRepository, ILogger<BooksController> logger, IMapper mapper, IUrlHelper urlHelper)
        {
            _libraryRepository = libraryRepository;
            _logger = logger;
            _mapper = mapper;
            _urlHelper = urlHelper;
        }

        [HttpGet(Name = "GetBooksForAuthor")]
        public IActionResult GetBooksForAuthor(Guid authorId, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var books = _libraryRepository.GetBooksForAuthor(authorId);

            var booksDto = _mapper.Map<IEnumerable<BookDto>>(books);
            
            if (mediaType == "application/vnd.gabriel.hateoas+json")
            {
                booksDto.ToList().ForEach(b => CreateLinksForBook(b));
                var wrapper = new LinkedCollectionResourceWrapperDto<BookDto>(booksDto);
                CreateLinksForBook(wrapper);
                return Ok(wrapper);
            }

            return Ok(booksDto);
        }

        [HttpGet("{id}", Name = "GetBookForAuthor")]
        public IActionResult GetBookForAuthor(Guid authorId, Guid id)
        {
            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var book = _libraryRepository.GetBookForAuthor(authorId, id);

            if (book == null)
            {
                return NotFound();
            }

            var bookDto = _mapper.Map<BookDto>(book);

            CreateLinksForBook(bookDto);

            return Ok(bookDto);
        }

        [HttpPost]
        public IActionResult CreateBookForAuthor(Guid authorId, [FromBody] BookForCreationDto book)
        {
            if (book == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookEntity = _mapper.Map<Entities.Book>(book);

            _libraryRepository.AddBookForAuthor(authorId, bookEntity);

            if (!_libraryRepository.Save())
            {
                throw new Exception($"Creating a book for author {authorId} failed on save.");
            }

            var bookDto = _mapper.Map<BookDto>(bookEntity);

            return CreatedAtRoute("GetBookForAuthor", new
            {
                authorId,
                id = bookEntity.Id
            }, bookDto);
        }

        [HttpDelete("{id}", Name = "DeleteBookForAuthor")]
        public IActionResult DeleteBookForAuthor(Guid authorId, Guid id)
        {
            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var book = _libraryRepository.GetBookForAuthor(authorId, id);

            if (book == null)
            {
                return NotFound();
            }

            _libraryRepository.DeleteBook(book);

            if (!_libraryRepository.Save())
            {
                throw new Exception($"Deleting book ${id} for author {authorId} failed on save.");
            }

            _logger.LogInformation(100, $"Deleted book {id} for author {authorId}.");

            return NoContent();
        }

        [HttpPut("{id}", Name = "UpdateBookForAuthor")]
        public IActionResult UpdateBookForAuthor(Guid authorId, Guid id, [FromBody] BookForUpdateDto book)
        {
            if (book == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookEntity = _libraryRepository.GetBookForAuthor(authorId, id);

            if (bookEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(book, bookEntity);

            _libraryRepository.UpdateBookForAuthor(bookEntity);

            if (!_libraryRepository.Save())
            {
                throw new Exception($"Updating book {id} for author {authorId} failed on save.");
            }

            return NoContent();
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateBookForAuthor")]
        public IActionResult PartiallyUpdateBookForAuthor(Guid authorId, Guid id, [FromBody] JsonPatchDocument<BookForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookEntity = _libraryRepository.GetBookForAuthor(authorId, id);

            if (bookEntity == null)
            {
                return NotFound();
            }

            var bookToPatch = _mapper.Map<BookForUpdateDto>(bookEntity);

            patchDoc.ApplyTo(bookToPatch, ModelState);

            TryValidateModel(bookToPatch);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            _mapper.Map(bookToPatch, bookEntity);

            _libraryRepository.UpdateBookForAuthor(bookEntity);

            if (!_libraryRepository.Save())
            {
                throw new Exception($"Patching book {id} for author {authorId} failed on save.");
            }

            return NoContent();
        }

        private BookDto CreateLinksForBook(BookDto book)
        {
            book.Links.Add(new LinkDto
            (
                _urlHelper.Link("GetBookForAuthor", new { id = book.Id, authorId = book.AuthorId }),
                "self",
                "GET"
            ));

            book.Links.Add(new LinkDto
            (
                _urlHelper.Link("DeleteBookForAuthor", new { id = book.Id, authorId = book.AuthorId }),
                "delete_book",
                "DELETE"
            ));

            book.Links.Add(new LinkDto
            (
                _urlHelper.Link("UpdateBookForAuthor", new { id = book.Id, authorId = book.AuthorId }),
                "update_book",
                "PUT"
            ));

            book.Links.Add(new LinkDto
            (
                _urlHelper.Link("PartiallyUpdateBookForAuthor", new { id = book.Id, authorId = book.AuthorId }),
                "partially_update_book",
                "PATCH"
            ));

            return book;
        }

        private LinkedCollectionResourceWrapperDto<BookDto> CreateLinksForBook(LinkedCollectionResourceWrapperDto<BookDto> booksWrapper)
        {
            booksWrapper.Links.Add(new LinkDto
            (
                _urlHelper.Link("GetBooksForAuthor", new { }),
                "self",
                "GET"
            ));

            return booksWrapper;
        }
    }
}
