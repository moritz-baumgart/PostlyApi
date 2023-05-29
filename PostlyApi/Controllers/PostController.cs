using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostlyApi.Entities;
using PostlyApi.Enums;
using PostlyApi.Models;
using PostlyApi.Models.DTOs;
using PostlyApi.Models.Errors;
using PostlyApi.Models.Requests;
using PostlyApi.Utilities;

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
        /// Creates a new post.
        /// </summary>
        /// <param name="content">The text content the post should have.</param>
        /// <returns>The id of the created post</returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<int> AddPost([FromBody] string content)
        {
            var user = DbUtilities.GetUserFromContext(HttpContext, _db);
            if (user == null) { return Unauthorized(); }

            var newPost = _db.Posts.Add(new Post(content, user, DateTime.UtcNow));
            _db.SaveChanges();
            return Ok(newPost.Entity.Id);
        }

        /// <summary>
        /// Gets the post with a given post id
        /// </summary>
        /// <param name="postId">The id of the targeted post</param>
        /// <returns>The post as a <see cref="PostDTO"/></returns>
        [HttpGet("{postId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PostDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<PostDTO> GetPost([FromRoute] long postId)
        {
            var post = _db.Posts
                .Where(p => p.Id == postId)
                .Include(p => p.Author)
                .FirstOrDefault();

            if (post == null) { return NotFound(Result.PostNotFound.ToString()); }

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
        /// Deletes the post with a given post id
        /// </summary>
        /// <param name="postId">The id of the targeted post</param>
        /// <returns></returns>
        [HttpDelete("{postId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult DeletePost([FromRoute] int postId)
        {
            var post = _db.Posts.FirstOrDefault(p => p.Id == postId);
            if (post == null) { return NotFound(Result.PostNotFound.ToString()); }

            var user = DbUtilities.GetUserFromContext(HttpContext, _db);
            if (user == null) { return Unauthorized(); }

            // is user author of post or has mod permission? If not, missing permission
            if (!(user.Id == post.UserId || user.Role > 0)) { return Forbid(); }

            _db.Posts.Remove(post);
            _db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Retrieves the users who have upvoted a post with given post id
        /// </summary>
        /// <param name="postId">The post id of the targeted post</param>
        /// <returns>A list of <see cref="UserDTO"/>s</returns>
        [HttpGet("{postId}/upvotes")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<UserDTO>> GetUpvotes([FromRoute] long postId)
        {
            var post = _db.Posts
                .Where(p => p.Id == postId)
                .Include(p => p.UpvotedBy)
                .FirstOrDefault();

            if (post == null) { return NotFound(Result.PostNotFound.ToString()); }

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
        /// <param name="postId">The post id of the targeted post</param>
        /// <returns>A list of <see cref="UserDTO"/>s</returns>
        [HttpGet("{postId}/downvotes")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<UserDTO>> Downvotes([FromRoute] long postId)
        {
            var post = _db.Posts
                .Where(p => p.Id == postId)
                .Include(p => p.DownvotedBy)
                .FirstOrDefault();

            if (post == null) { return NotFound(Result.PostNotFound.ToString()); }

            var result = post.DownvotedBy.Select(u => new UserDTO
            {
                Id = u.Id,
                Username = u.Username,
                DisplayName = u.DisplayName
            });

            return Ok(result);
        }

        /// <summary>
        /// Adds the specified reaction to the post with given post id.
        /// </summary>
        /// <param name="request">A <see cref="Models.Requests.InteractionRequest"/>.</param>
        /// <returns>A <see cref="PostlyApi.Models.SuccessResult{T, E}"/> with true, no value and <see cref="Models.Errors.InteractionError.None"/> if the vote was successful, otherwise false, no value and a <see cref="Models.Errors.InteractionError"/>.</returns>
        [HttpPost("{postId}/vote")]
        [Authorize]
        public SuccessResult<object, InteractionError> UpOrDownvote([FromBody] InteractionRequest request)
        {
            var user = DbUtilities.GetUserFromContext(HttpContext, _db);

            if (user == null) { return new SuccessResult<object, InteractionError>(false, InteractionError.UserNotFound.ToString()); }

            var post = _db.Posts
                .Where(p => p.Id == request.PostId)
                .Include(p => p.UpvotedBy)
                .Include(p => p.DownvotedBy)
                .FirstOrDefault();

            if (post == null) { return new SuccessResult<object, InteractionError>(false, InteractionError.PostNotFound.ToString()); }

            switch (request.Type)
            {
                case VoteInteractionType.Upvote:
                    if (post.UpvotedBy.Contains(user)) { return new SuccessResult<object, InteractionError>(false, InteractionError.InteractionAlreadyMade.ToString()); }
                    post.UpvotedBy.Add(user);
                    post.DownvotedBy.Remove(user);
                    break;
                case VoteInteractionType.Downvote:
                    if (post.DownvotedBy.Contains(user)) { return new SuccessResult<object, InteractionError>(false, InteractionError.InteractionAlreadyMade.ToString()); }
                    post.DownvotedBy.Add(user);
                    post.UpvotedBy.Remove(user);
                    break;
                case VoteInteractionType.Remove:
                    post.UpvotedBy.Remove(user);
                    post.DownvotedBy.Remove(user);
                    break;
            }

            _db.SaveChanges();

            return new SuccessResult<object, InteractionError>(true, InteractionError.None.ToString());
        }


        /// <summary>
        /// Retrieves the comments of a post with given post id.
        /// </summary>
        /// <param name="postId">The post id of the post that the comments should be retrieved for.</param>
        /// <returns>Return status 200 with an <see cref="IEnumerable{T}"/> with <see cref="Entities.CommentDTO"/>s if successful. If the post was not found status 404.</returns>
        [HttpGet("{postId}/comments")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CommentDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<CommentDTO>> Comments([FromRoute] long postId)
        {
            var post = _db.Posts
                .Include(p => p.Comments)
                    .ThenInclude(c => c.Author)
                .Where(p => p.Id == postId)
                .FirstOrDefault();

            if (post == null) { return NotFound(Result.PostNotFound.ToString()); }

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

        /// <summary>
        /// Adds the given comment text to the post with given id.
        /// </summary>
        /// <param name="request">A <see cref="Models.Requests.CommentCreateRequest"/>.</param>
        /// <returns>A <see cref="PostlyApi.Models.SuccessResult{T, E}"/> with true, no value and <see cref="Models.Errors.CommentError.None"/> if the operation was successful, otherwise false, no value and a <see cref="Models.Errors.InteractionError"/>.</returns>
        [HttpPost("comment")]
        [Authorize]
        public SuccessResult<object, CommentError> Comment([FromBody] CommentCreateRequest request)
        {
            var user = DbUtilities.GetUserFromContext(HttpContext, _db);

            if (user == null) { return new SuccessResult<object, CommentError>(false, CommentError.UserNotFound.ToString()); }

            var post = _db.Posts.FirstOrDefault(p => p.Id == request.PostId);
            if (post == null) { return new SuccessResult<object, CommentError>(false, CommentError.PostNotFound.ToString()); }

            post.Comments.Add(new Comment(user, post, request.CommentContent));
            _db.SaveChanges();

            return new SuccessResult<object, CommentError>(true, CommentError.None.ToString());
        }

        /// <summary>
        /// Deletes the comment with a given comment id
        /// </summary>
        /// <param name="commentId">The id of the targeted comment</param>
        /// <returns></returns>
        [HttpDelete("comment/{commentId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteComment([FromRoute] int commentId)
        {
            var comment = _db.Comments.FirstOrDefault(c => c.Id == commentId);
            if (comment == null) { return NotFound(Result.CommentNotFound.ToString()); }

            var user = DbUtilities.GetUserFromContext(HttpContext, _db);
            if (user == null) { return Unauthorized(); }

            // is user author of comment or has mod permission? If not, missing permission
            // TODO: let author of post delete commments?
            if (!(user.Id == comment.UserId || user.Role > 0)) { return Forbid(); }

            _db.Comments.Remove(comment);
            _db.SaveChanges();

            return Ok();
        }
    }
}
