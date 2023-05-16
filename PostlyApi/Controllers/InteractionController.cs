using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostlyApi.Models;
using PostlyApi.Models.Errors;
using PostlyApi.Utilities;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;

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
        /// <param name="postId">The id of the post that the reaction should be added to.</param>
        /// <param name="type">The type of reaction that should be added, must be a variant of <see cref="Models.VoteInteractionType"/></param>
        /// <returns>A <see cref="PostlyApi.Models.SuccessResult{T, E}"/> with true, no value and <see cref="Models.Errors.RegisterError.None"/> if the vote was successful, otherwise false, no value and a <see cref="Models.Errors.RegisterError"/>.</returns>
        [HttpPost("vote")]
        [Authorize]
        public SuccessResult<object, InteractionError> UpOrDownvote([Required] int postId, [Required] VoteInteractionType type)
        {
            var user = DbUtilities.GetUserFromContext(HttpContext, _db);
            if (user == null)
            {
                return new SuccessResult<object, InteractionError>(false, InteractionError.UserNotFound);
            }

            var post = _db.Posts
                .Include(p => p.UpvotedBy)
                .Include(p => p.DownvotedBy)
                .Single(p => p.Id == postId);

            if (post == null)
            {
                return new SuccessResult<object, InteractionError>(false, InteractionError.PostNotFound);
            }

            switch (type)
            {
                case VoteInteractionType.Upvote:
                    if (post.UpvotedBy.Contains(user))
                    {
                        return new SuccessResult<object, InteractionError>(false, InteractionError.InteractionAlreadyMade);
                    }
                    post.UpvotedBy.Add(user);
                    post.DownvotedBy.Remove(user);
                    break;
                case VoteInteractionType.Downvote:
                    if (post.DownvotedBy.Contains(user))
                    {
                        return new SuccessResult<object, InteractionError>(false, InteractionError.InteractionAlreadyMade);
                    }
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



        // TODO: Add comment
        [HttpPost("comment")]
        [Authorize]
        public SuccessResult<object, InteractionError> Comment([Required] int postId, [Required] string commentContent)
        {

            var user = DbUtilities.GetUserFromContext(HttpContext, _db);
            if (user == null)
            {
                return new SuccessResult<object, InteractionError>(false, InteractionError.UserNotFound);
            }

            var post = _db.Posts.Find(postId);
            if (post == null)
            {
                return new SuccessResult<object, InteractionError>(false, InteractionError.PostNotFound);
            }

            // TODO: Implement this and change the data model to do so.
            throw new NotImplementedException("Comments do not yet exist in Post model");

            return new SuccessResult<object, InteractionError>(true, InteractionError.None);
        }
    }
}
