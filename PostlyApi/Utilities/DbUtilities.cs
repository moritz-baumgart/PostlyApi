using PostlyApi.Entities;
using PostlyApi.Models;
using PostlyApi.Models.Errors;
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
    }
}
