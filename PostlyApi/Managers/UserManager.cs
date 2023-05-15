using PostlyApi.Contexts;

namespace PostlyApi.Managers
{
    public class UserManager
    {
        private readonly PostlyContext _db;

        public UserManager(PostlyContext dbContext)
        {
            _db = dbContext;
        }
    }
}
