using PostlyApi.Contexts;

namespace PostlyApi.Managers
{
    public class PostManager
    {
        private readonly PostlyContext _db;

        public PostManager(PostlyContext dbContext)
        {
            _db = dbContext;
        }
    }
}
