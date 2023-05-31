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

        public static VoteInteractionType? GetVoteInteractionTypeOfUserForPost(User? user, Post post)
        {
            if (user == null)
            {
                return null;
            }
            else
            {
                if (post.UpvotedBy.Contains(user))
                {
                    return VoteInteractionType.Upvote;
                }
                else if (post.DownvotedBy.Contains(user))
                {
                    return VoteInteractionType.Downvote;
                }
                else
                {
                    return VoteInteractionType.Remove;
                }
            }
        }
    }
}
