using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PostlyApi.Entities;
using PostlyApi.Enums;
using PostlyApi.Models;
using PostlyApi.Models.Errors;
using PostlyApi.Models.Requests;
using PostlyApi.Utilities;

namespace PostlyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InteractionController : ControllerBase
    {
        private readonly PostlyContext _db;

        public InteractionController(PostlyContext dbContext)
        {
            _db = dbContext;
        }

        /// <summary>
        /// Adds the specified reaction to the post with given post id.
        /// </summary>
        /// <param name="request">A <see cref="Models.Requests.InteractionRequest"/>.</param>
        /// <returns>A <see cref="PostlyApi.Models.SuccessResult{T, E}"/> with true, no value and <see cref="Models.Errors.InteractionError.None"/> if the vote was successful, otherwise false, no value and a <see cref="Models.Errors.InteractionError"/>.</returns>
        [HttpPost("vote")]
        [Authorize]
        public SuccessResult<object, InteractionError> UpOrDownvote([FromBody] InteractionRequest request)
        {
            var user = DbUtilities.GetUserFromContext(HttpContext, _db);

            if (user == null) { return new SuccessResult<object, InteractionError>(false, InteractionError.UserNotFound); }

            var post = _db.Posts
                .Where(p => p.Id == request.PostId)
                .Include(p => p.UpvotedBy)
                .Include(p => p.DownvotedBy)
                .Single();

            if (post == null) { return new SuccessResult<object, InteractionError>(false, InteractionError.PostNotFound); }

            switch (request.Type)
            {
                case VoteInteractionType.Upvote:
                    if (post.UpvotedBy.Contains(user)) { return new SuccessResult<object, InteractionError>(false, InteractionError.InteractionAlreadyMade); }
                    post.UpvotedBy.Add(user);
                    post.DownvotedBy.Remove(user);
                    break;
                case VoteInteractionType.Downvote:
                    if (post.DownvotedBy.Contains(user)) { return new SuccessResult<object, InteractionError>(false, InteractionError.InteractionAlreadyMade); }
                    post.DownvotedBy.Add(user);
                    post.UpvotedBy.Remove(user);
                    break;
                case VoteInteractionType.Remove:
                    post.UpvotedBy.Remove(user);
                    post.DownvotedBy.Remove(user);
                    break;
            }

            _db.SaveChanges();

            return new SuccessResult<object, InteractionError>(true, InteractionError.None);
        }



        /// <summary>
        /// Adds the given comment text to the post with given id.
        /// </summary>
        /// <param name="request">A <see cref="Models.Requests.CommentRequest"/>.</param>
        /// <returns>A <see cref="PostlyApi.Models.SuccessResult{T, E}"/> with true, no value and <see cref="Models.Errors.CommentError.None"/> if the operation was successful, otherwise false, no value and a <see cref="Models.Errors.InteractionError"/>.</returns>
        [HttpPost("comment")]
        [Authorize]
        public SuccessResult<object, CommentError> Comment([FromBody] CommentRequest request)
        {
            var user = DbUtilities.GetUserFromContext(HttpContext, _db);

            if (user == null) { return new SuccessResult<object, CommentError>(true, CommentError.UserNotFound); }

            var post = _db.Posts.Single(p => p.Id == request.PostId);
            if (post == null) { return new SuccessResult<object, CommentError>(true, CommentError.PostNotFound); }

            post.Comments.Add(new Comment(user, post, request.CommentContent));
            _db.SaveChanges();

            return new SuccessResult<object, CommentError>(true, CommentError.None);
        }
    }
}
