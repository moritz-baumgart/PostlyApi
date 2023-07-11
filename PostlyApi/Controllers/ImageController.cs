using Microsoft.AspNetCore.Mvc;
using PostlyApi.Entities;
using PostlyApi.Enums;
using PostlyApi.Manager;
using PostlyApi.Models;

namespace PostlyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly ImageManager _imageManager;
        private readonly UserManager _userManager;
        private readonly PostManager _postManager;

        public ImageController(PostlyContext dbContext)
        {
            _imageManager = new ImageManager(dbContext);
            _userManager = new UserManager(dbContext);
            _postManager = new PostManager(dbContext);
        }


        [HttpGet("{imageId}")]
        public ActionResult Get([FromRoute] Guid imageId)
        {
            var result = _imageManager.Get(imageId);

            if (result == null)
            {
                return NotFound(Error.ImageNotFound);
            }

            return File(result.Data, result.ContentType);
        }


        #region profile image

        [HttpGet("user/{username}")]
        public ActionResult GetProfileImage([FromRoute] string username)
        {
            var user = _userManager.Get(username);
            if (user == null)
            {
                return NotFound(Error.UserNotFound);
            }

            var result = _imageManager.Get(user);
            if (result == null)
            {
                return NotFound(Error.ImageNotFound);
            }

            return File(result.Data, result.ContentType);
        }

        [HttpPut("user/{username}")]
        public ActionResult<Image> UpdateProfileImage([FromRoute] string username)
        {
            var user = _userManager.Get(username);
            if (user == null)
            {
                return NotFound(Error.UserNotFound);
            }

            if (!Request.Form.Files.Any())
            {
                return BadRequest();
            }

            var file = Request.Form.Files[0];

            if (file == null)
            {
                return BadRequest();
            }

            using var memStream = new MemoryStream();
            file.CopyTo(memStream);
            var data = memStream.ToArray();

            _imageManager.Update(user, data, file.ContentType);

            return Ok();
        }

        #endregion profile image


        #region image attached to post

        [HttpGet("post/{postId}")]
        public ActionResult<Image> GetPostImage([FromRoute] int postId)
        {
            var post = _postManager.Get(postId);
            if (post == null)
            {
                return NotFound(Error.PostNotFound);
            }

            var result = _imageManager.Get(post);
            if (result == null)
            {
                return NotFound(Error.ImageNotFound);
            }

            return File(result.Data, result.ContentType);
        }

        [HttpPut("post/{postId}")]
        public ActionResult UpdatePostImage([FromRoute] int postId)
        {
            var post = _postManager.Get(postId);
            if (post == null)
            {
                return NotFound(Error.PostNotFound);
            }

            if (!Request.Form.Files.Any())
            {
                return BadRequest();
            }

            var file = Request.Form.Files[0];

            if (file == null)
            {
                return BadRequest();
            }

            using var memStream = new MemoryStream();
            file.CopyTo(memStream);
            var data = memStream.ToArray();

            _imageManager.Update(post, data, file.ContentType);

            return Ok();
        }

        #endregion image attached to post
    }
}
