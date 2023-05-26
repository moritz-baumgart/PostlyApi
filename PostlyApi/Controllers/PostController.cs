using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PostlyApi.Entities;
using PostlyApi.Models;
using PostlyApi.Models.DTOs;
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

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PostDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<PostDTO> Post([Required] long PostId)
        {
            var post = _db.Posts
                .Where(p => p.Id == PostId)
                .Include(p => p.Author)
                .Single();

            if (post == null) { return NotFound(); }

            var result = new PostDTO()
            {
                Id = post.Id,
                Content = post.Content,
                Author = new UserDTO
                {
                    Id = post.Author.Id,
                    Username = post.Author.Username,
                    DisplayName = post.Author.DisplayName
                },
                CreatedAt = post.CreatedAt,
                UpvoteCount = post.UpvotedBy.Count,
                DownvoteCount = post.DownvotedBy.Count,
                CommentCount = post.Comments.Count
            };  

            return Ok(result);
        }

        /// <summary>
        /// Retrieves the users who have upvoted a post with given post id
        /// </summary>
        /// <param name="PostId">The post id of the targeted post</param>
        /// <returns></returns>
        [HttpGet("upvotes")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<UserDTO>> Upvotes([Required] long PostId)
        {
            var post = _db.Posts
                .Where(p => p.Id == PostId)
                .Include(p => p.UpvotedBy)
                .Single();

            if (post == null) { return NotFound(); }

            var result = post.UpvotedBy.Select(u => new UserDTO
            {
                Id = u.Id,
                Username = u.Username,
                DisplayName = u.DisplayName
            });

            return Ok(result);
        }

        /// <summary>
        /// Retrieves the users who have downvoted a post with given post id
        /// </summary>
        /// <param name="PostId">The post id of the targeted post</param>
        /// <returns></returns>
        [HttpGet("downvotes")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<UserDTO>> Downvotes([Required] long PostId)
        {
            var post = _db.Posts
                .Where(p => p.Id == PostId)
                .Include(p => p.DownvotedBy)
                .Single();

            if (post == null) { return NotFound(); }

            var result = post.DownvotedBy.Select(u => new UserDTO
            {
                Id = u.Id,
                Username = u.Username,
                DisplayName = u.DisplayName
            });

            return Ok(result);
        }


        /// <summary>
        /// Retrieves the comments of a post with given post id.
        /// </summary>
        /// <param name="PostId">The post id of the post that the comments should be retrieved for.</param>
        /// <returns>Return status 200 with an <see cref="IEnumerable{T}"/> with <see cref="Entities.CommentDTO"/>s if successful. If the post was not found status 404.</returns>
        [HttpGet("comments")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CommentDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<CommentDTO>> Comments([Required] long PostId)
        {
            var post = _db.Posts
                .Include(p => p.Comments)
                    .ThenInclude(c => c.Author)
                .Where(p => p.Id == PostId)
                .Single();

            if (post == null) { return NotFound(); }

            var result = post.Comments
                    .Select(c => new CommentDTO
                    {
                        Id = c.Id,
                        Author = new UserDTO
                        {
                            Id = c.Author.Id,
                            Username = c.Author.Username,
                            DisplayName = c.Author.DisplayName
                        },
                        Content = c.Content,
                        CreatedAt = c.CreatedAt
                    }
                    );

            return Ok(result);
        }
    }
}
