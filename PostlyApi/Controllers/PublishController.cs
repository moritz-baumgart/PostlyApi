using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostlyApi.Entities;
using PostlyApi.Models;
using PostlyApi.Models.Errors;
using PostlyApi.Utilities;

namespace PostlyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublishController : ControllerBase
    {
        private readonly PostlyContext _db;

        public PublishController(PostlyContext dbContext)
        {
            _db = dbContext;
        }


        /// <summary>
        /// Creates a new post.
        /// </summary>
        /// <param name="content">The text content the post should have.</param>
        /// <returns>A <see cref="PostlyApi.Models.SuccessResult{T, E}"/> with true, no value and <see cref="Models.Errors.NewPostError.None"/> if the creation was successful, otherwise false, no value and a <see cref="Models.Errors.NewPostError"/>.</returns>
        [HttpPost("newpost")]
        [Authorize]
        public SuccessResult<object?, NewPostError> NewPost([FromBody] string content)
        {
            var user = DbUtilities.GetUserFromContext(HttpContext, _db);
            if (user == null)
            {
                return new SuccessResult<object?, NewPostError>(false, NewPostError.UserNotFound);
            }

            _db.Posts.Add(new Post(content, user, DateTime.UtcNow));
            _db.SaveChanges();
            return new SuccessResult<object?, NewPostError>(true, NewPostError.None);
        }
    }
}
