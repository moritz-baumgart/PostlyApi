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
        /// Retrieves a list with the given maximum length, which contains post that were posted between the from and to date. It is ordered from new to old. All timestamps should be provided in UTC.
        /// </summary>
        /// <param name="from">The "from" date, all retrieved posts will be older or equally old as this.</param>
        /// <param name="to">The "to" date, all retrieved posts will be newer or equally new as this. Defaults to the current time.</param>
        /// <param name="maxNumber">Maximum number of posts to retrieve.</param>
        /// <returns>A list of <see cref="Models.DTOs.PostDTO"/>s according to the discription.</returns>
        [HttpGet("public")]
        public IEnumerable<PostDTO> GetPublic([Required] DateTime from, DateTime? to, int maxNumber = 25)
        {
            if (to == null)
            {
                to = DateTime.UtcNow;
            }

            var user = DbUtilities.GetUserFromContext(HttpContext, _db);

            if (user != null)
            {
                return _db.Posts
                          .Include(p => p.UpvotedBy)
                          .Include(p => p.DownvotedBy)
                          .Where(p => p.CreatedAt >= from && p.CreatedAt <= to)
                          .OrderByDescending(p => p.CreatedAt)
                          .Take(maxNumber).Select(p => new PostDTO
                          {
                              Id = p.Id,
                              Content = p.Content,
                              Author = new AuthorDTO
                              {
                                  Id = p.Author.Id,
                                  Username = p.Author.Username,
                                  DisplayName = p.Author.DisplayName
                              },
                              CreatedAt = p.CreatedAt,
                              UpvoteCount = p.UpvotedBy.Count,
                              DownvoteCount = p.DownvotedBy.Count,
                              CommentCount = p.Comments.Count,
                              Vote = p.UpvotedBy.Contains(user) ? VoteInteractionType.Upvote : p.DownvotedBy.Contains(user) ? VoteInteractionType.Downvote : VoteInteractionType.Remove,
                              HasCommented = p.Comments.Any(c => c.Author == user)
                          });
            }
            else
            {
                return _db.Posts
                          .Where(p => p.CreatedAt >= from && p.CreatedAt <= to)
                          .OrderByDescending(p => p.CreatedAt)
                          .Take(maxNumber).Select(p => new PostDTO
                          {
                              Id = p.Id,
                              Content = p.Content,
                              Author = new AuthorDTO
                              {
                                  Id = p.Author.Id,
                                  Username = p.Author.Username,
                                  DisplayName = p.Author.DisplayName
                              },
                              CreatedAt = p.CreatedAt,
                              UpvoteCount = p.UpvotedBy.Count,
                              DownvoteCount = p.DownvotedBy.Count,
                              CommentCount = p.Comments.Count,
                          });
            }
        }
    }
}
