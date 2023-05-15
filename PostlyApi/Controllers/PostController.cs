using Microsoft.AspNetCore.Http.HttpResults;
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
    public class PostController : Controller
    {
        private readonly PostManager _postManager;

        public PostController(PostlyContext dbContext)
        {
            _postManager = new PostManager(dbContext);
        }


        /* Publishes a new post */
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public ActionResult CreatePost([FromBody] PostCreateModel data)
        {
            return Ok();
        }


        /* Returns the post with the given Id */
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(PostViewModel))]
        [ProducesResponseType(404)]
        public ActionResult<PostViewModel> GetPost(long id)
        {
            return Ok();
        }

        /* Edits the post with the given Id */
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public ActionResult UpdatePost([FromBody] PostUpdateModel data)
        {
            return Ok();
        }

        /* Deletes the post with the given Id */
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public ActionResult DeletePost(long id)
        {
            return Ok();
        }


        /* Returns the users who have upvoted the given post */
        [HttpGet("{id}/upvoters")]
        [ProducesResponseType(200, Type = typeof(List<UserViewModel>))]
        [ProducesResponseType(404)]
        public ActionResult<List<UserViewModel>> GetUpvoters(long id) 
        {
            return Ok();
        }


        /* Returns the users who have downvoted the given post */
        [HttpGet("{id}/downvoters)")]
        [ProducesResponseType(200, Type = typeof(List<UserViewModel>))]
        [ProducesResponseType(404)]
        public ActionResult<List<UserViewModel>> GetDownvoters(long id)
        {
            return Ok();
        }
    }
}
