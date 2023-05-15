using Microsoft.AspNetCore.Mvc;
using PostlyApi.Contexts;
using PostlyApi.Entities;
using PostlyApi.Managers;
using PostlyApi.Models.PostModels;
using PostlyApi.Models.UserDataModels;

namespace PostlyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly UserManager _userManager;

        public UserController(PostlyContext dbContext)
        {
            _userManager = new UserManager(dbContext);
        }


        /* Creates a new user */
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public ActionResult CreateUser([FromBody] UserCreateModel data)
        {
            return Ok();
        }


        /* Returns the user with the given Id */
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(UserViewModel))]
        [ProducesResponseType(404)]
        public ActionResult<UserViewModel> GetUser(long id)
        {
            return Ok();
        }

        /* Updates a users profile data */
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public ActionResult UpdateUser(long id, [FromBody] UserUpdateModel data)
        {
            return Ok();
        }

        /* Deletes the user with the given Id */
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public ActionResult DeleteUser(long id)
        {
            return Ok();
        }


        /* Returns the users that follow the given user */ 
        [HttpGet("{id}/followers")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<List<UserViewModel>> GetFollowers(long id)
        {
            // only return ids?
            return Ok();
        }


        /* Returns the users that the given user is following */
        [HttpGet("{id}/following")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<List<UserViewModel>> GetFollowing(long id)
        {
            // only return ids?
            return Ok();
        }

        /* Follows a given user */
        [HttpPost("{id}/following")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public ActionResult FollowUser(long id, [FromBody] long targetUserId)
        {
            return Ok();
        }


        /* Returns all posts published by the given user */
        [HttpGet("{id}/posts")]
        [ProducesResponseType(200, Type = typeof(List<PostViewModel>))]
        [ProducesResponseType(404)]
        public ActionResult<List<PostViewModel>> GetOwnPosts(long id)
        {
            return Ok();
        }


        /* Returns all posts upvoted by the given user */
        [HttpGet("{id}/upvotes")]
        [ProducesResponseType(200, Type = typeof(List<PostViewModel>))]
        [ProducesResponseType(404)]
        public ActionResult<List<PostViewModel>> GetUpvotedPosts(long id)
        {
            return Ok();
        }

        /* Upvotes the given post */
        [HttpPost("{id}/upvotes")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public ActionResult UpvotePost(long id, [FromBody] long postId)
        {
            return Ok();
        }

        /* Removes the upvote from the given post */
        [HttpDelete("{id}/upvotes")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public ActionResult RemoveUpvote(long id, [FromBody] long postId)
        {
            return Ok();
        }


        /* Returns all posts downvoted by the given user */
        [HttpGet("{id}/downvotes")]
        [ProducesResponseType(200, Type = typeof(List<PostViewModel>))]
        [ProducesResponseType(404)]
        public ActionResult<List<PostViewModel>> GetDownvotedPosts(long id)
        {
            return Ok();
        }

        /* Downvotes the given post */
        [HttpPost("{id}/downvotes")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public ActionResult DownvotePost(long id, [FromBody] long postId)
        {
            return Ok();
        }

        /* Removes the downvote from the given post */
        [HttpDelete("{id}/downvotes")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public ActionResult RemoveDownvote(long id, [FromBody] long postId)
        {
            return Ok();
        }
    }
}
