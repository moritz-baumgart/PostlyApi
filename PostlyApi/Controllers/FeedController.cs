using Microsoft.AspNetCore.Mvc;
using PostlyApi.Models;
using PostlyApi.Models.DTOs;
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

            return _db.Posts
                .Select(p => new PostDTO
                {
                    Id = p.Id,
                    Content = p.Content,
                    Author = new AuthorDTO
                    {
                        Id = p.Author.Id,
                        Username = p.Author.Username,
                        DisplayName = p.Author.DisplayName
                    },
                    CreatedDate = p.CreatedDate,
                    UpvoteCount = p.UpvotedBy.Count,
                    DownvoteCount = p.DownvotedBy.Count
                })
                .Where(p => p.CreatedDate >= from && p.CreatedDate <= to)
                .OrderByDescending(p => p.CreatedDate)
                .Take(maxNumber);
        }
    }
}
