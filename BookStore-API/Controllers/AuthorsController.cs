using AutoMapper;
using BookStore_API.Contracts;
using BookStore_API.Data;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_API.Controllers
{
    /// <summary>
    /// Endpoint used to interact with the Authors in the book store's database
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // overall requirement, overruled by any statement at endpoint level
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;

        public AuthorsController(IAuthorRepository authorRepository, ILoggerService logger, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all Authors
        /// </summary>
        /// <returns>List of Authors</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthors()
        {
            try
            {
                Info($"Attempted GetAuthors");
                var authors = await _authorRepository.FindAll();
                var response = _mapper.Map<IList<AuthorDTO>>(authors);
                Info($"Successfully got all authors");
                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError(e);
            }
        }

        /// <summary>
        /// Gets the author identified by a given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Author record</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthor(int id)
        {
            try
            {
                Info($"Attempted GetAuthor({id})");
                var author = await _authorRepository.FindById(id);
                if (author == null)
                {
                    Warn($"No author {id}");
                    return NotFound();
                }
                var response = _mapper.Map<AuthorDTO>(author);
                Info($"Successfully got author {id}");
                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError(e);
            }
        }

        /// <summary>
        /// Create a new author
        /// </summary>
        /// <param name="authorDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] AuthorCreateDTO authorDTO)
        {
            try
            {
                Info($"Author creation attempted");
                if (authorDTO == null)
                {
                    Warn($"Empty request was submitted");
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                {
                    Warn($"Invalid modelstate '{ModelState}'");
                    return BadRequest(ModelState);
                }
                var author = _mapper.Map<Author>(authorDTO);
                var isSuccess = await _authorRepository.Create(author);
                if (!isSuccess)
                    return InternalError($"Author {author} creation failed.");
                Info($"Author created");
                return Created("Create", new { author });

            }
            catch (Exception e)
            {
                return InternalError(e);
            }

        }

        /// <summary>
        /// This endpoint updates the author object
        /// </summary>
        /// <param name="id"></param>
        /// <param name="authorDTO"></param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] AuthorUpdateDTO authorDTO)
        {
            try
            {
                Info($"Author update attempted");
                if (id < 1 || authorDTO == null || id != authorDTO.id)
                {
                    Warn($"Empty request or id < 1 was submitted");
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                {
                    Warn($"Invalid modelstate '{ModelState}'");
                    return BadRequest(ModelState);
                }
                var author = _mapper.Map<Author>(authorDTO);
                var isSuccess = await _authorRepository.Update(author);
                if (!isSuccess)
                    return InternalError($"Author {id} '{author}' update failed.");
                Info($"Author updated");
                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError(e);
            }

        }
         /// <summary>
         /// This endpoint deletes an Author
         /// </summary>
         /// <param name="id"></param>
         /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                Info($"Author delete attempted");
                if (id < 1 )
                {
                    Warn($"id {id} < 1 was submitted");
                    return BadRequest(ModelState);
                }
                var doesExist = await _authorRepository.doesExist(id);
                if (!doesExist)
                {
                    Warn($"Unknown author {id}");
                    return NotFound();
                }
                var author = await _authorRepository.FindById(id);
                if (author == null)
                {
                    Warn($"Should not occur after previous check!");
                    return NotFound();
                }
                var isSuccess = await _authorRepository.Delete(author);
                if (!isSuccess)
                    return InternalError($"{By()}Author {id} delete failed.");
                Info($"Author deleted");
                return NoContent();

            }
            catch (Exception e) 
            {
                return InternalError(e);
            }

        }
        private void Error(string message)
        {
            _logger.LogError($"{By()}: {message}");
        }
        private void Info(string message)
        {
            _logger.LogInfo($"{By()}: {message}");
        }
        private void Warn(string message)
        {
            _logger.LogWarn($"{By()}: {message}");
        }
        private string By()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;
            return $"{controller} - {action}";
        }
        private ObjectResult InternalError(Exception e)
        {
            _logger.LogError($"{By()}: {e.Message} - {e.InnerException}");
            return StatusCode(500, "Something went wrong. Please contact the Administrator");
        }
        private ObjectResult InternalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, "Something went wrong. Please contact the Administrator");

        }
    }
}
