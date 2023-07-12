using PostlyApi.Entities;
using PostlyApi.Enums;
using PostlyApi.Models;
using PostlyApi.Models.DTOs;
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

        public static UserDTO GetUserDTO(User user)
        {
            var result = new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                DisplayName = user.DisplayName,
                ProfileImageUrl = $"api/image/user/{user.Username}",
            };

            return result;
        }

        public static UserProfileViewModel GetUserProfile(User user, PostlyContext _db, HttpContext httpContext)
        {
            _db.Entry(user).Collection(u => u.Follower).Load();
            _db.Entry(user).Collection(u => u.Following).Load();

            var currentUser = GetUserFromContext(httpContext, _db);

            var result = new UserProfileViewModel
            {
                Id = user.Id,
                CreatedAt = user.CreatedAt,
                Username = user.Username,
                DisplayName = user.DisplayName,
                Role = user.Role,
                FollowerCount = user.Follower.Count(),
                FollowingCount = user.Following.Count(),
                Birthday = user.Birthday,
                Gender = user.Gender,
                ProfileImageUrl = $"api/image/user/{user.Username}",
                Follow = currentUser != null ? user.Follower.Contains(currentUser) : null
            };

            return result;
        }

        public static UserDataViewModel GetUserData(User user)
        {
            var result = new UserDataViewModel
            {
                Id = user.Id,
                CreatedAt = user.CreatedAt,
                Username = user.Username,
                DisplayName = user.DisplayName,
                Role = user.Role,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Birthday = user.Birthday,
                Gender = user.Gender,
                ProfileImageUrl = $"api/image/user/{user.Username}",
            };

            return result;
        }

        public static PostDTO GetPostDTO(Post post, User? user, PostlyContext _db)
        {
            _db.Entry(post).Reference(p => p.Author).Load();
            _db.Entry(post).Collection(p => p.Votes).Load();
            _db.Entry(post).Collection(p => p.Comments).Load();

            var result = new PostDTO()
            {
                Id = post.Id,
                Content = post.Content,
                Author = GetUserDTO(post.Author),
                CreatedAt = post.CreatedAt,
                AttachedImageUrl = $"api/image/post/{post.Id}",
                UpvoteCount = post.Votes.Where(v => v.VoteType == VoteType.Upvote).Count(),
                DownvoteCount = post.Votes.Where(v => v.VoteType == VoteType.Downvote).Count(),
                CommentCount = post.Comments.Count,
                Vote = GetVoteTypeOfUserForPost(user, post),
                HasCommented = HasUserCommentedOnPost(user, post)
            };

            return result;
        }

        public static CommentDTO GetCommentDTO(Comment comment, PostlyContext _db)
        {
            _db.Entry(comment).Reference(p => p.Author).Load();

            var result = new CommentDTO()
            {
                Id = comment.Id,
                Author = GetUserDTO(comment.Author),
                Content = comment.Content,
                CreatedAt = comment.CreatedAt
            };

            return result;
        }

        /// <summary>
        /// Checks if a username is already in use already exists in the given context.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="username"></param>
        /// <returns>True if the user already exists, false otherwise.</returns>
        public static bool IsUsernameAlreadyInUser(PostlyContext dbContext, string username)
        {
            return dbContext.Users.Any(u => u.Username.Equals(username));
        }
    }
}
