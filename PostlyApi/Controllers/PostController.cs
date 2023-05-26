using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostlyApi.Models;
using PostlyApi.Models.DTOs;
using PostlyApi.Models.Errors;
using System.ComponentModel.DataAnnotations;

namespace PostlyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly PostlyContext _db;

        public PostController(PostlyContext dbContext)
        {
            _db = dbContext;
        }


        /// <summary>
        /// Retrieves the comments of a post with given post id.
        /// </summary>
        /// <param name="PostId">The post id of the post that the comments should be retrieved for.</param>
        /// <returns>Return status 200 with an <see cref="IEnumerable{T}"/> with <see cref="Models.Comment"/>s if successfull. If the post was not found status 404.</returns>
        [HttpGet("comments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<Comment>> Comments([Required] int PostId)
        {
            var post = _db.Posts
                            .Include(p => p.Comments)
                                .ThenInclude(c => c.Author)
                            .FirstOrDefault(p => p.Id == PostId);

            if (post == null)
            {
                return NotFound();
            }

            return Ok(
                post.Comments
                    .Select(c => new CommentDTO
                    {
                        Id = c.Id,
                        Author = new AuthorDTO
                        {
                            Id = c.Author.Id,
                            Username = c.Author.Username,
                            DisplayName = c.Author.DisplayName
                        },
                        Content = c.Content,
                        CreatedAt = c.CreatedAt
                    }
                    )
             );
        }
    }
}
