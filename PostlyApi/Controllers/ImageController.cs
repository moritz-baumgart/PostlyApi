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


        #region image

        /// <summary>
        /// Returns the image with the given id
        /// </summary>
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

        /// <summary>
        /// Deletes the image with the given id from the database
        /// </summary>
        [HttpDelete("{imageId}")]
        public ActionResult DeleteUserImage([FromRoute] Guid imageId)
        {
            var image = _imageManager.Get(imageId);
            if (image == null)
            {
                return NotFound(Error.ImageNotFound);
            }

            _imageManager.Delete(image);

            return Ok();
        }

        #endregion image


        #region profile image

        /// <summary>
        /// Returns the profile image of a given user
        /// </summary>
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

        /// <summary>
        /// Updates the profile image of a given user
        /// </summary>
        [HttpPut("user/{username}")]
        public ActionResult<string> UpdateProfileImage([FromRoute] string username)
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

            var image = _imageManager.Update(user, data, file.ContentType);

            return Ok($"\"image/{image.Id}\"");
        }

        /// <summary>
        /// Deletes the profile image of a given user from the database
        /// </summary>
        [HttpDelete("user/{username}")]
        public ActionResult DeleteUserImage([FromRoute] string username)
        {
            var user = _userManager.Get(username);
            if (user == null)
            {
                return NotFound(Error.UserNotFound);
            }

            var image = _imageManager.Get(user);
            if (image == null)
            {
                return NotFound(Error.ImageNotFound);
            }

            _imageManager.Delete(image);

            return Ok();
        }

        #endregion profile image


        #region image attached to post

        /// <summary>
        /// Returns the image attached to a given post
        /// </summary>
        [HttpGet("post/{postId}")]
        public ActionResult GetPostImage([FromRoute] int postId)
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

        /// <summary>
        /// Updates the image attached to a given post
        /// </summary>
        [HttpPut("post/{postId}")]
        public ActionResult<string> UpdatePostImage([FromRoute] int postId)
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

            var image = _imageManager.Update(post, data, file.ContentType);

            return Ok($"\"image/{image.Id}\"");
        }

        /// <summary>
        /// Deletes the image attached to a given post from the database
        /// </summary>
        [HttpDelete("post/{postId}")]
        public ActionResult DeletePostImage([FromRoute] int postId)
        {
            var post = _postManager.Get(postId);
            if (post == null)
            {
                return NotFound(Error.PostNotFound);
            }

            var image = _imageManager.Get(post);
            if (image == null)
            {
                return NotFound(Error.ImageNotFound);
            }

            _imageManager.Delete(image);

            return Ok();
        }

        #endregion image attached to post
    }
}
