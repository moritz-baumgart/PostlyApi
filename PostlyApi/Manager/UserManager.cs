using PostlyApi.Entities;
using PostlyApi.Models;

namespace PostlyApi.Manager
{
    public class UserManager : BaseManager
    {
        public UserManager(PostlyContext dbContext) : base(dbContext)
        {
        }

        public User? Get(long id)
        {
            return _db.Users.FirstOrDefault(_ => _.Id == id);
        }

        public User? Get(string username)
        {
            return _db.Users.FirstOrDefault(_ => _.Username == username);
        }
    }
}
