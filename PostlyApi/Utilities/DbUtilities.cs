using PostlyApi.Entities;
using PostlyApi.Enums;
using PostlyApi.Models;
using System.Security.Claims;

namespace PostlyApi.Utilities
{
    public class DbUtilities
    {

        public static User? GetUserFromContext(HttpContext httpContext, PostlyContext dbContext)
        {
            if (httpContext.User.Identity is not ClaimsIdentity identity)
            {
                return null;
            }

            var usernameClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
            if (usernameClaim == null)
            {
                return null;
            }

            return dbContext.Users.Where(u => u.Username == usernameClaim.Value).FirstOrDefault();
        }

        public static VoteType? GetVoteTypeOfUserForPost(User? user, Post post)
        {
            if (user == null || post == null) return null;

            var vote = post.Votes
                .Where(v => v.UserId == user.Id)
                .FirstOrDefault();

            if (vote == null) return null;

            return vote.VoteType;
        }

        public static bool HasUserCommentedOnPost(User? user, Post post)
        {
            if (user == null || post == null) return false;

            return post.Comments.Any(c => c.UserId == user.Id);
        }
    }
}
