using AutoMapper;
using BookStore_API.Contracts;
using BookStore_API.Data;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_API.Controllers
{
    /// <summary>
    /// Endpoint to interactwith the books inthe bookstore API
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;

        public BooksController(IBookRepository bookRepository, ILoggerService loggerService, IMapper mapper, IAuthorRepository authorRepository)
        {
            _bookRepository = bookRepository;
            _logger = loggerService;
            _mapper = mapper;
            _authorRepository = authorRepository;
        }
        /// <summary>
        /// Get all Books
        /// </summary>
        /// <returns>List of Books</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBooks()
        {
            try
            {
                _logger.LogInfo($"{By()}Attempted GetBooks");
                var books = await _bookRepository.FindAll();
                var response = _mapper.Map<IList<BookDTO>>(books);
                _logger.LogInfo($"{By()}Successfully got all books");
                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError(e);
            }
        }

        /// <summary>
        /// Gets the book identified by a given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Book record</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBook(int id)
        {
            try
            {
                _logger.LogInfo($"{By()}Attempted GetBook({id})");
                var book = await _bookRepository.FindById(id);
                if (book == null)
                {
                    _logger.LogWarn($"{By()}No book {id}");
                    return NotFound();
                }
                var response = _mapper.Map<BookDTO>(book);
                _logger.LogInfo($"{By()}Successfully got book {id}");
                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError(e);
            }
        }

        /// <summary>
        /// Create a new book
        /// </summary>
        /// <param name="bookDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] BookCreateDTO bookDTO)
        {
            try
            {
                _logger.LogInfo($"{By()}Book creation attempted");
                if (bookDTO == null)
                {
                    _logger.LogWarn($"{By()}Empty request was submitted");
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"{By()}Invalid modelstate '{ModelState}'");
                    return BadRequest(ModelState);
                }
                var book = _mapper.Map<Book>(bookDTO);
                var isSuccess = await _bookRepository.Create(book);
                if (!isSuccess)
                    return InternalError($"{By()}Book {book} creation failed.");
                _logger.LogInfo($"{By()}Book created");
                return Created("Create", new { book });

            }
            catch (Exception e)
            {
                return InternalError(e);
            }

        }

        /// <summary>
        /// This endpoint updates the book object
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bookDTO"></param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] BookUpdateDTO bookDTO)
        {
            try
            {
                _logger.LogInfo($"{By()}Book update attempted");
                if (id < 1 || bookDTO == null || id != bookDTO.id)
                {
                    _logger.LogWarn($"{By()}Empty request or id < 1 was submitted");
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"{By()}Invalid modelstate '{ModelState}'");
                    return BadRequest(ModelState);
                }
                var book = _mapper.Map<Book>(bookDTO);
                var isSuccess = await _bookRepository.Update(book);
                if (!isSuccess)
                    return InternalError($"{By()}Book {id} '{book}' update failed.");
                _logger.LogInfo($"{By()}Book updated");
                return NoContent();

            }
            catch (Exception e)
            {
                return InternalError(e);
            }

        }
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInfo($"{By()}Book delete attempted");
                if (id < 1)
                {
                    _logger.LogWarn($"{By()}id < 1 was submitted");
                    return BadRequest(ModelState);
                }
                var doesExist = await _bookRepository.doesExist(id);
                if (!doesExist)
                {
                    _logger.LogWarn($"{By()}Unknown book {id}");
                    return NotFound();
                }
                var book = await _bookRepository.FindById(id);
                if (book == null)
                {
                    _logger.LogWarn($"{By()}Should not occur after previous check!");
                    return NotFound();
                }
                var isSuccess = await _bookRepository.Delete(book);
                if (!isSuccess)
                    return InternalError($"{By()}Book {id} delete failed.");
                _logger.LogInfo($"{By()}Book deleted");
                return NoContent();

            }
            catch (Exception e)
            {
                return InternalError(e);
            }

        }

        private string By()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action= ControllerContext.ActionDescriptor.ActionName;
            return $"{controller} - {action}: ";
        }
        private ObjectResult InternalError(Exception e)
        {
            _logger.LogError($"{By()}{e.Message} - {e.InnerException}");
            return StatusCode(500, "Something went wrong. Please contact the Administrator");
        }
        private ObjectResult InternalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, "Something went wrong. Please contact the Administrator");
        }

    }
}
