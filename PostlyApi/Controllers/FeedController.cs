using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostlyApi.Enums;
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
        public ActionResult<IEnumerable<PostDTO>> GetPublic([Required] DateTime paginationStart, [Required] int pageNumber, int pageSize = 10)
        {
            var user = DbUtilities.GetUserFromContext(HttpContext, _db);

            return Ok(_db.Posts
                      .Where(p => p.CreatedAt <= paginationStart)
                      .Include(p => p.UpvotedBy)
                      .Include(p => p.DownvotedBy)
                      .OrderByDescending(p => p.CreatedAt)
                      .Skip(pageNumber * pageSize)
                      .Take(pageSize).Select(p => new PostDTO
                      {
                          Id = p.Id,
                          Content = p.Content,
                          Author = new UserDTO
                          {
                              Id = p.Author.Id,
                              Username = p.Author.Username,
                              DisplayName = p.Author.DisplayName
                          },
                          CreatedAt = p.CreatedAt.ToUniversalTime(),
                          UpvoteCount = p.UpvotedBy.Count,
                          DownvoteCount = p.DownvotedBy.Count,
                          CommentCount = p.Comments.Count,
                          Vote = user != null ? p.UpvotedBy.Contains(user) ? VoteInteractionType.Upvote : p.DownvotedBy.Contains(user) ? VoteInteractionType.Downvote : VoteInteractionType.Remove : null,
                          HasCommented = p.Comments.Any(c => c.Author == user)
                      }));
        }

        /// <summary>
        /// Retrieves a page of posts authored by users that the user the request was made by follows.
        /// </summary>
        /// <param name="paginationStart">The start date and time at which the pagination should start in UTC time.</param>
        /// <param name="pageNumber">The page to retrieve.</param>
        /// <param name="pageSize">The size of the page, defaults to 10.</param>
        /// <returns>The page of posts as <see cref="IEnumerable{T}"/> of <see cref="Models.DTOs.PostDTO"/></returns>
        [HttpGet("private")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PostDTO>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<IEnumerable<PostDTO>> GetPrivate([Required] DateTime paginationStart, [Required] int pageNumber, int pageSize = 10)
        {
            var user = DbUtilities.GetUserFromContext(HttpContext, _db);

            if (user == null)
            {
                return Unauthorized();
            }

            _db.Entry(user).Collection(u => u.Following).Load();

            var potentialAuthors = user.Following.Select(u => u.Id);

            var result = _db.Posts
                .Where(p => potentialAuthors.Contains(p.UserId) && p.CreatedAt <= paginationStart)
                .Include(p => p.UpvotedBy)
                .Include(p => p.DownvotedBy)
                .OrderByDescending(p => p.CreatedAt)
                .Skip(pageNumber * pageSize)
                .Take(pageSize).Select(p => new PostDTO
                {
                    Id = p.Id,
                    Content = p.Content,
                    Author = new UserDTO
                    {
                        Id = p.Author.Id,
                        Username = p.Author.Username,
                        DisplayName = p.Author.DisplayName
                    },
                    CreatedAt = p.CreatedAt.ToUniversalTime(),
                    UpvoteCount = p.UpvotedBy.Count,
                    DownvoteCount = p.DownvotedBy.Count,
                    CommentCount = p.Comments.Count,
                    Vote = p.UpvotedBy.Contains(user) ? VoteInteractionType.Upvote : p.DownvotedBy.Contains(user) ? VoteInteractionType.Downvote : VoteInteractionType.Remove,
                    HasCommented = p.Comments.Any(c => c.Author == user)
                });

            return Ok(result);
        }
    }
}
