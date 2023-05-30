using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostlyApi.Entities;
using PostlyApi.Enums;
using PostlyApi.Models;
using PostlyApi.Models.Requests;
using PostlyApi.Utilities;

namespace PostlyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly PostlyContext _db;

        public CommentController(IConfiguration config, PostlyContext dbContext)
        {
            _config = config;
            _db = dbContext;
        }

        /// <summary>
        /// Adds the given comment text to the post with given id.
        /// </summary>
        /// <param name="request">A <see cref="CommentCreateRequest"/>.</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
        public ActionResult Comment([FromBody] CommentCreateRequest request)
        {
            var user = DbUtilities.GetUserFromContext(HttpContext, _db);

            if (user == null) { return Unauthorized(); }

            var post = _db.Posts.FirstOrDefault(p => p.Id == request.PostId);
            if (post == null) { return NotFound(Error.PostNotFound); }

            post.Comments.Add(new Comment(user, post, request.CommentContent));
            _db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Deletes the comment with a given comment id
        /// </summary>
        /// <param name="commentId">The id of the targeted comment</param>
        /// <returns></returns>
        [HttpDelete("{commentId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteComment([FromRoute] int commentId)
        {
            var comment = _db.Comments.FirstOrDefault(c => c.Id == commentId);
            if (comment == null) { return NotFound(Error.CommentNotFound.ToString()); }

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
