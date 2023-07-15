using PostlyApi.Entities;
using PostlyApi.Models;

namespace PostlyApi.Manager
{
    public class UserManager : BaseManager
    {
        public UserManager(PostlyContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// Returns the <see cref="User"/> with the given id
        /// </summary>
        public User? Get(long id)
        {
            return _db.Users.FirstOrDefault(_ => _.Id == id);
        }

        /// <summary>
        /// Returns the <see cref="User"/> with the given username
        /// </summary>
        public User? Get(string username)
        {
            return _db.Users.FirstOrDefault(_ => _.Username == username);
        }
    }
}
