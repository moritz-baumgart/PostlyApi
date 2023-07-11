using PostlyApi.Models;

namespace PostlyApi.Manager
{
    public class BaseManager
    {
        protected PostlyContext _db;

        public BaseManager(PostlyContext dbContext)
        {
            _db = dbContext;
        }
    }
}
