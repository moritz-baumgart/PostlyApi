using PostlyApi.Entities;
using PostlyApi.Models;

namespace PostlyApi.Manager
{
    public class ImageManager : BaseManager
    {
        public ImageManager(PostlyContext dbContext) : base(dbContext) 
        { 
        }

        /// <summary>
        /// Adds a new image to the database
        /// </summary>
        public Image Add(byte[] data, string contentType)
        {
            var result = _db.Images.Add(new Image
            {
                Data = data,
                ContentType = contentType,
            }).Entity;

            _db.SaveChanges();

            return result;
        }

        /// <summary>
        /// Returns the <see cref="Image"/> with the given id
        /// </summary>
        public Image? Get(Guid? id)
        {
            return _db.Images.FirstOrDefault(_ => _.Id == id);
        }

        /// <summary>
        /// Returns the profile image of the given user
        /// </summary>
        public Image? Get(User user)
        {
            return Get(user.ImageId);
        }

        /// <summary>
        /// Returns the image attached to the given post
        /// </summary>
        public Image? Get(Post post)
        {
            return Get(post.ImageId);
        }

        /// <summary>
        /// Updates the image with the given id
        /// </summary>
        public Image? Update(Guid id, byte[] data, string contentType)
        {
            var image = new Image
            {
                Id = id,
                Data = data,
                ContentType = contentType,
            };

            return Update(image);
        }

        /// <summary>
        /// Updates the profile image of the given user
        /// </summary>
        public Image Update(User user, byte[] data, string contentType)
        {
            var oldImage = Get(user);
            Delete(oldImage);

            var image = new Image
            {
                Data = data,
                ContentType = contentType,
            };

            user.ProfileImage = image;

            return Update(image);
        }

        /// <summary>
        /// Updates the image attached to the given post
        /// </summary>
        public Image Update(Post post, byte[] data, string contentType)
        {
            var oldImage = Get(post);
            Delete(oldImage);

            var image = new Image
            {
                Data = data,
                ContentType = contentType,
            };

            post.AttachedImage = image;

            return Update(image);
        }

        /// <summary>
        /// Updates the given image
        /// </summary>
        public Image Update(Image image)
        {
            var result = _db.Images.Update(image).Entity;
            _db.SaveChanges();

            return result;
        }

        /// <summary>
        /// Deletes the <see cref="Image"/> with the given id
        /// </summary>
        public Image? Delete(Guid id)
        {
            var image = Get(id);

            return Delete(image);
        }

        /// <summary>
        /// Deletes the given <see cref="Image"/> from the database
        /// </summary>
        public Image? Delete(Image? image)
        {
            if (image == null) return null;

            var result = _db.Images.Remove(image).Entity;
            _db.SaveChanges();

            return result;
        }
    }
}
