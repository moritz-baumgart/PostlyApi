using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostlyApi.Models;
using PostlyApi.Models.DTOs;
using PostlyApi.Utilities;
using System.ComponentModel.DataAnnotations;

namespace PostlyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedController : ControllerBase
    {
        private readonly PostlyContext _db;

        public FeedController(PostlyContext dbContext)
        {
            _db = dbContext;
        }

        /// <summary>
        /// Retrieves a page of posts.
        /// </summary>
        /// <param name="paginationStart">The start date and time at which the pagination should start in UTC time.</param>
        /// <param name="pageNumber">The page to retrieve.</param>
        /// <param name="pageSize">The size of the page, defaults to 10.</param>
        /// <returns>The page of posts as <see cref="IEnumerable{T}"/> of <see cref="Models.DTOs.PostDTO"/></returns>
        [HttpGet("public")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PostDTO>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<IEnumerable<PostDTO>> GetPublicFeed([Required] DateTime paginationStart, int pageSize = 10)
        {
            var user = DbUtilities.GetUserFromContext(HttpContext, _db);

            return Ok(_db.Posts
                      .Where(p => p.CreatedAt < paginationStart)
                      .OrderByDescending(p => p.CreatedAt)
                      .Take(pageSize)
                      .ToList()
                      .Select(p => DbUtilities.GetPostDTO(p, user, _db)));
        }

        /// <summary>
        /// Retrieves a page of posts authored by a given user.
        /// </summary>
        /// <param name="paginationStart">The start date and time at which the pagination should start in UTC time.</param>
        /// <param name="pageNumber">The page to retrieve.</param>
        /// <param name="pageSize">The size of the page, defaults to 10.</param>
        /// <returns>The page of posts as <see cref="IEnumerable{T}"/> of <see cref="Models.DTOs.PostDTO"/></returns>
        [HttpGet("profile/{username}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PostDTO>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<IEnumerable<PostDTO>> GetProfileFeed([FromRoute] string username, [Required] DateTime paginationStart, int pageSize = 10)
        {
            var user = DbUtilities.GetUserFromContext(HttpContext, _db);

            return Ok(_db.Posts
                      .Where(p => p.CreatedAt < paginationStart && p.Author.Username == username)
                      .OrderByDescending(p => p.CreatedAt)
                      .Take(pageSize)
                      .ToList()
                      .Select(p => DbUtilities.GetPostDTO(p, user, _db)));
        }

        /// <summary>
        /// Retrieves a page of posts authored by the current user.
        /// </summary>
        /// <param name="paginationStart">The start date and time at which the pagination should start in UTC time.</param>
        /// <param name="pageNumber">The page to retrieve.</param>
        /// <param name="pageSize">The size of the page, defaults to 10.</param>
        /// <returns>The page of posts as <see cref="IEnumerable{T}"/> of <see cref="Models.DTOs.PostDTO"/></returns>
        [HttpGet("profile/me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PostDTO>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<IEnumerable<PostDTO>> GetProfileFeed([Required] DateTime paginationStart, int pageSize = 10)
        {
            var user = DbUtilities.GetUserFromContext(HttpContext, _db);

            if (user == null)
            {
                return Unauthorized();
            }

            return Ok(_db.Posts
                      .Where(p => p.CreatedAt < paginationStart && p.Author.Id == user.Id)
                      .OrderByDescending(p => p.CreatedAt)
                      .Take(pageSize)
                      .ToList()
                      .Select(p => DbUtilities.GetPostDTO(p, user, _db)));
        }

        /// <summary>
        /// Retrieves a page of posts authored by users that the current user is following.
        /// </summary>
        /// <param name="paginationStart">The start date and time at which the pagination should start in UTC time.</param>
        /// <param name="pageNumber">The page to retrieve.</param>
        /// <param name="pageSize">The size of the page, defaults to 10.</param>
        /// <returns>The page of posts as <see cref="IEnumerable{T}"/> of <see cref="Models.DTOs.PostDTO"/></returns>
        [HttpGet("private")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PostDTO>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<IEnumerable<PostDTO>> GetPrivateFeed([Required] DateTime paginationStart, int pageSize = 10)
        {
            var user = DbUtilities.GetUserFromContext(HttpContext, _db);

            if (user == null)
            {
                return Unauthorized();
            }

            _db.Entry(user).Collection(u => u.Following).Load();

            var potentialAuthors = user.Following.Select(u => u.Id);

            var result = _db.Posts
                .Where(p => p.CreatedAt < paginationStart && potentialAuthors.Contains(p.UserId))
                .OrderByDescending(p => p.CreatedAt)
                .Take(pageSize)
                .ToList()
                .Select(p => DbUtilities.GetPostDTO(p, user, _db));

            return Ok(result);
        }
    }
}
