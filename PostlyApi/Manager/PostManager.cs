using PostlyApi.Entities;
using PostlyApi.Models;

namespace PostlyApi.Manager
{
    public class PostManager : BaseManager
    {
        public PostManager(PostlyContext dbContext) : base(dbContext)
        {
        }

        public Post? Get(int id)
        {
            return _db.Posts.FirstOrDefault(_ => _.Id == id);
        }
    }
}
