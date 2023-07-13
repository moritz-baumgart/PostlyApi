using PostlyApi.Entities;
using PostlyApi.Models;

namespace PostlyApi.Manager
{
    public class ImageManager : BaseManager
    {
        public ImageManager(PostlyContext dbContext) : base(dbContext) 
        { 
        }

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

        public Image? Get(Guid? id)
        {
            return _db.Images.FirstOrDefault(_ => _.Id == id);
        }

        public Image? Get(User user)
        {
            return Get(user.ImageId);
        }

        public Image? Get(Post post)
        {
            return Get(post.ImageId);
        }

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

        public Image Update(Image image)
        {
            var result = _db.Images.Update(image).Entity;
            _db.SaveChanges();

            return result;
        }

        public Image? Delete(Guid id)
        {
            var image = Get(id);

            return Delete(image);
        }

        public Image? Delete(Image? image)
        {
            if (image == null) return null;

            var result = _db.Images.Remove(image).Entity;
            _db.SaveChanges();

            return result;
        }
    }
}
