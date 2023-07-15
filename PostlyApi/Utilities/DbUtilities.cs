using PostlyApi.Entities;
using PostlyApi.Enums;
using PostlyApi.Models;
using PostlyApi.Models.DTOs;
using System.Security.Claims;

namespace PostlyApi.Utilities
{
    public class DbUtilities
    {
        /// <summary>
        /// Returns the currently logged in user
        /// </summary>
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

        /// <summary>
        /// Returns the <see cref="VoteType"/> of a given user on a given post. Null if there is no vote>
        /// </summary>
        public static VoteType? GetVoteTypeOfUserForPost(User? user, Post post)
        {
            if (user == null || post == null) return null;

            var vote = post.Votes
                .Where(v => v.UserId == user.Id)
                .FirstOrDefault();

            if (vote == null) return null;

            return vote.VoteType;
        }

        /// <summary>
        /// Returns true if a given user has commented on a given post, otherwise false
        /// </summary>
        public static bool HasUserCommentedOnPost(User? user, Post post)
        {
            if (user == null || post == null) return false;

            return post.Comments.Any(c => c.UserId == user.Id);
        }

        /// <summary>
        /// Maps a given <see cref="User"/> to a <see cref="UserDTO"/>
        /// </summary>
        public static UserDTO GetUserDTO(User user)
        {
            var result = new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                DisplayName = user.DisplayName,
                ProfileImageUrl = user.ImageId != null ? $"/image/{user.ImageId}" : null,
            };

            return result;
        }

        /// <summary>
        /// Maps a given <see cref="User"/> to a <see cref="UserProfileViewModel"/>.
        /// If a logged in user is requesting this information, their user interaction data will also be retrieved
        /// </summary>
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
                ProfileImageUrl = user.ImageId != null ? $"/image/{user.ImageId}" : null,
                Follow = currentUser != null ? user.Follower.Contains(currentUser) : null
            };

            return result;
        }

        /// <summary>
        /// Maps a given <see cref="User"/> to a <see cref="UserDataViewModel"/>
        /// </summary>
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
                ProfileImageUrl = user.ImageId != null ? $"/image/{user.ImageId}" : null,
            };

            return result;
        }

        /// <summary>
        /// Maps a given <see cref="Post"/> to a <see cref="PostDTO"/>.
        /// If a <see cref="User"/> is given, their post interaction data will also be retrieved
        /// </summary>
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
                AttachedImageUrl = post.ImageId != null ? $"/image/{post.ImageId}" : null,
                UpvoteCount = post.Votes.Where(v => v.VoteType == VoteType.Upvote).Count(),
                DownvoteCount = post.Votes.Where(v => v.VoteType == VoteType.Downvote).Count(),
                CommentCount = post.Comments.Count,
                Vote = GetVoteTypeOfUserForPost(user, post),
                HasCommented = HasUserCommentedOnPost(user, post)
            };

            return result;
        }

        /// <summary>
        /// Maps a given <see cref="Comment"/> to a <see cref="CommentDTO"/>
        /// </summary>
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
