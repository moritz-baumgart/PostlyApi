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
            };

            return result;
        }

        public static UserProfileViewModel GetUserProfile(User user, PostlyContext _db)
        {
            _db.Entry(user).Collection(u => u.Follower).Load();
            _db.Entry(user).Collection(u => u.Following).Load();

            var result = new UserProfileViewModel
            {
                Id = user.Id,
                CreatedAt = user.CreatedAt,
                Username =  user.Username,
                DisplayName= user.DisplayName,
                Role = user.Role,
                FollowerCount = user.Follower.Count(),
                FollowingCount = user.Following.Count(),
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
            };

            return result;
        }

        public static PostDTO GetPostDTO(Post post, PostlyContext _db)
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
                UpvoteCount = post.Votes.Where(v => v.VoteType == VoteType.Upvote).Count(),
                DownvoteCount = post.Votes.Where(v => v.VoteType == VoteType.Downvote).Count(),
                CommentCount = post.Comments.Count,
                Vote = GetVoteTypeOfUserForPost(post.Author, post),
                HasCommented = HasUserCommentedOnPost(post.Author, post)
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
    }
}
