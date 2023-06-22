using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PostlyApi.Entities;
using PostlyApi.Enums;
using PostlyApi.Models;
using PostlyApi.Models.DTOs;
using PostlyApi.Models.Requests;
using PostlyApi.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PostlyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly PostlyContext _db;

        public AccountController(IConfiguration config, PostlyContext dbContext)
        {
            _config = config;
            _db = dbContext;
        }

        #region Login & Register
        /// <summary>
        /// Tries to login a user with given credentials and generates a jwt if the user exists and the provided password is correct.
        /// </summary>
        /// <param name="request">The login request containing credentials.</param>
        /// <returns>The JWT-Token on success, and a <see cref="Error"/> on failure</returns>
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Error))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
        public ActionResult Login([FromBody] LoginOrRegisterRequest request)
        {
            // Query the database for the user
            var user = _db.Users.FirstOrDefault(u => u.Username.Equals(request.Username));

            // if user doesn't exist:
            if (user == null)
            {
                return NotFound(Error.UserNotFound);
            }

            // if the password was incorrect:
            if (!PasswordUtilities.VerifyPassword(request.Password, user.PasswordHash))
            {
                return Unauthorized(Error.PasswordIncorrect);
            }

            // Record the login in the database
            _db.Logins.Add(new Login
            {
                CreatedAt = DateTimeOffset.UtcNow,
                UserId = user.Id,
            });
            _db.SaveChanges();

            // Get our key from config
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]));

            // specify the key and the algorithm to use
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // claims are "fields" in the jwt
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            // generate a token using everything above and the issuer and audience from config
            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credentials);

            // send token to client
            return Ok(new JwtSecurityTokenHandler().WriteToken(token));
        }

        /// <summary>
        /// Registers a new user with given credentials.
        /// </summary>
        /// <param name="request">The register request</param>
        /// <returns>The id of the created user on success, a <see cref="Error"/> on failure</returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(long))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(Error))]
        public ActionResult Register([FromBody] LoginOrRegisterRequest request)
        {
            // Check if the user already exists, if so return error
            if (DbUtilities.IsUsernameAlreadyInUser(_db, request.Username))
            {
                return Conflict(Error.UsernameAlreadyInUse);
            }

            // if the role parameter is null or the current user is not an admin, set role parameter of request to default
            var currentUser = DbUtilities.GetUserFromContext(HttpContext, _db);
            if ((request.Role == null) || (currentUser != null && currentUser.Role != Role.Admin))
            {
                request.Role = Role.User;
            }

            // Otherwise add a new user and return success
            var user = _db.Users.Add(new User(request.Username, request.Password, (Role)request.Role));
            _db.SaveChanges();

            return Ok(user.Entity.Id);
        }
        #endregion

        #region {username}
        [HttpGet("{username}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
        public ActionResult<UserDTO> GetUser([FromRoute] string username)
        {
            var targetUser = _db.Users.FirstOrDefault(u => u.Username == username);
            if (targetUser == null)
            {
                return NotFound(Error.UserNotFound);
            }

            var result = DbUtilities.GetUserDTO(targetUser);

            return Ok(result);
        }

        [HttpDelete("{username}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
        public ActionResult DeleteUser([FromRoute] string username)
        {
            var currentUser = DbUtilities.GetUserFromContext(HttpContext, _db);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var targetUser = _db.Users.FirstOrDefault(u => u.Username == username);
            if (targetUser == null)
            {
                return NotFound(Error.UserNotFound);
            }

            // if the current user doesn't have permission to delete this account:
            if (!(currentUser == targetUser || currentUser.Role == Role.Admin))
            {
                return Forbid();
            }

            _db.Remove(targetUser);
            _db.SaveChanges();

            return Ok();
        }

        [HttpGet("{username}/profile")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserProfileViewModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
        public ActionResult<UserProfileViewModel> GetUserProfile([FromRoute] string username)
        {
            var targetUser = _db.Users.FirstOrDefault(u => u.Username == username);
            if (targetUser == null)
            {
                return NotFound(Error.UserNotFound);
            }

            var result = DbUtilities.GetUserProfile(targetUser, _db);

            return Ok(result);
        }

        [HttpGet("{username}/data")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDataViewModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
        public ActionResult<UserDataViewModel> GetUserData([FromRoute] string username)
        {
            var targetUser = _db.Users.FirstOrDefault(u => u.Username == username);
            if (targetUser == null)
            {
                return NotFound(Error.UserNotFound);
            }

            var result = DbUtilities.GetUserData(targetUser);

            return Ok(result);
        }

        [HttpPatch("{username}/data")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDataViewModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IEnumerable<Error>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
        public ActionResult<UserDataViewModel> UpdateUserData([FromRoute] string username, UserDataUpdateRequest request)
        {
            var currentUser = DbUtilities.GetUserFromContext(HttpContext, _db);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var targetUser = _db.Users.FirstOrDefault(u => u.Username == username);
            if (targetUser == null)
            {
                return NotFound(Error.UserNotFound);
            }

            // if the current user doesn't have permission to change this profile:
            if (!(currentUser == targetUser || currentUser.Role == Role.Admin))
            {
                return Forbid();
            }

            if (!request.IsValid(out var errors))
            {
                return Conflict(errors);
            }

            request.UpdateUser(targetUser);

            _db.SaveChanges();

            var result = DbUtilities.GetUserData(targetUser);

            return Ok(result);
        }

        [HttpPut("{username}/username")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(Error))]
        public ActionResult ChangeUsername([FromRoute] string username, [FromBody] string newUsername)
        {
            var currentUser = DbUtilities.GetUserFromContext(HttpContext, _db);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var targetUser = _db.Users.FirstOrDefault(u => u.Username == username);
            if (targetUser == null)
            {
                return NotFound(Error.UserNotFound);
            }

            // if the current user doesn't have permission to change this username:
            if (!(currentUser == targetUser || currentUser.Role == Role.Admin))
            {
                return Forbid();
            }

            // if the username is already taken by someone else:
            if (DbUtilities.IsUsernameAlreadyInUser(_db, newUsername) && !newUsername.Equals(targetUser.Username))
            {
                return Conflict(Error.UsernameAlreadyInUse);
            }

            targetUser.Username = newUsername;
            _db.SaveChanges();

            return Ok();
        }

        [HttpPut("{username}/password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(Error))]
        public ActionResult ChangePassword([FromRoute] string username, [FromBody] PasswordUpdateRequest request)
        {
            var currentUser = DbUtilities.GetUserFromContext(HttpContext, _db);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var targetUser = _db.Users.FirstOrDefault(u => u.Username == username);
            if (targetUser == null)
            {
                return NotFound(Error.UserNotFound);
            }

            // if the current user doesn't have permission to change this password:
            if (!(currentUser.Username == username || currentUser.Role == Role.Admin))
            {
                return Forbid();
            }

            // if the old password was wrong:
            if (!PasswordUtilities.VerifyPassword(request.OldPassword, targetUser.PasswordHash))
            {
                return Conflict(Error.PasswordIncorrect);
            }

            targetUser.PasswordHash = PasswordUtilities.ComputePasswordHash(request.NewPassword);
            _db.SaveChanges();

            return Ok();
        }

        [HttpPut("{username}/role")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDataViewModel))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
        public ActionResult<UserDataViewModel> UpdateRole([FromRoute] string username, [FromBody] Role role)
        {
            var currentUser = DbUtilities.GetUserFromContext(HttpContext, _db);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var targetUser = _db.Users.FirstOrDefault(u => u.Username == username);
            if (targetUser == null)
            {
                return NotFound(Error.UserNotFound);
            }

            // if the current user is not an admin:
            if (currentUser.Role != Role.Admin)
            {
                return Forbid();
            }

            targetUser.Role = role;
            _db.SaveChanges();

            var result = DbUtilities.GetUserData(targetUser);

            return Ok(result);
        }

        [HttpGet("{username}/followers")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
        public ActionResult<IEnumerable<UserDTO>> GetFollowers([FromRoute] string username)
        {
            var targetUser = _db.Users.FirstOrDefault(u => u.Username == username);
            if (targetUser == null)
            {
                return NotFound(Error.UserNotFound);
            }

            _db.Entry(targetUser).Collection(u => u.Follower).Load();

            var result = targetUser.Follower.Select(DbUtilities.GetUserDTO);

            return Ok(result);
        }

        [HttpGet("{username}/followers/count")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
        public ActionResult<int> GetFollowerCount([FromRoute] string username)
        {
            var targetUser = _db.Users.FirstOrDefault(u => u.Username == username);
            if (targetUser == null)
            {
                return Unauthorized();
            }

            _db.Entry(targetUser).Collection(u => u.Follower).Load();

            var result = targetUser.Follower.Count();

            return Ok(result);
        }

        [HttpGet("{username}/following")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
        public ActionResult<IEnumerable<UserDTO>> GetFollowing([FromRoute] string username)
        {
            var targetUser = _db.Users.FirstOrDefault(u => u.Username == username);
            if (targetUser == null)
            {
                return NotFound(Error.UserNotFound);
            }

            _db.Entry(targetUser).Collection(u => u.Following).Load();

            var result = targetUser.Following.Select(DbUtilities.GetUserDTO);

            return Ok(result);
        }

        [HttpGet("{username}/following/count")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
        public ActionResult<int> GetFollowingCount([FromRoute] string username)
        {
            var targetUser = _db.Users.FirstOrDefault(u => u.Username == username);
            if (targetUser == null)
            {
                return Unauthorized();
            }

            _db.Entry(targetUser).Collection(u => u.Following).Load();

            var result = targetUser.Following.Count();

            return Ok(result);
        }

        [HttpPost("{sourceUsername}/following/{targetUsername}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserProfileViewModel))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
        public ActionResult<UserProfileViewModel> FollowUser([FromRoute] string sourceUsername, string targetUsername)
        {
            var currentUser = DbUtilities.GetUserFromContext(HttpContext, _db);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var sourceUser = _db.Users.FirstOrDefault(u => u.Username == sourceUsername);
            var targetUser = _db.Users.FirstOrDefault(u => u.Username == targetUsername);
            if (sourceUser == null || targetUser == null)
            {
                return NotFound(Error.UserNotFound);
            }

            // if the current user doesn't have permission to initiate this follow:
            if (!(currentUser == sourceUser || currentUser.Role == Role.Admin))
            {
                return Forbid();
            }

            if (!(sourceUser == targetUser))
            {
                _db.Entry(sourceUser).Collection(u => u.Following).Load();
                sourceUser.Following.Add(targetUser);
                _db.SaveChanges();
            }

            var result = DbUtilities.GetUserProfile(targetUser, _db);

            return Ok(result);
        }

        [HttpDelete("{sourceUsername}/following/{targetUsername}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserProfileViewModel))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
        public ActionResult<UserProfileViewModel> UnfollowUser([FromRoute] string sourceUsername, string targetUsername)
        {
            var currentUser = DbUtilities.GetUserFromContext(HttpContext, _db);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var sourceUser = _db.Users.FirstOrDefault(u => u.Username == sourceUsername);
            var targetUser = _db.Users.FirstOrDefault(u => u.Username == targetUsername);
            if (sourceUser == null || targetUser == null)
            {
                return NotFound(Error.UserNotFound);
            }

            // if the current user doesn't have permission to initiate this follow:
            if (!(currentUser == sourceUser || currentUser.Role == Role.Admin))
            {
                return Forbid();
            }

            _db.Entry(sourceUser).Collection(u => u.Following).Load();
            sourceUser.Following.Remove(targetUser);
            _db.SaveChanges();

            var result = DbUtilities.GetUserProfile(targetUser, _db);

            return Ok(result);
        }
        #endregion {username}

        #region me
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDTO))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<UserDTO> GetUser()
        {
            var currentUser = DbUtilities.GetUserFromContext(HttpContext, _db);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var result = DbUtilities.GetUserDTO(currentUser);

            return Ok(result);
        }

        [HttpDelete("me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult DeleteUser()
        {
            var currentUser = DbUtilities.GetUserFromContext(HttpContext, _db);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            _db.Remove(currentUser);
            _db.SaveChanges();

            return Ok();
        }

        [HttpGet("me/profile")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserProfileViewModel))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<UserProfileViewModel> GetProfile()
        {
            var currentUser = DbUtilities.GetUserFromContext(HttpContext, _db);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var result = DbUtilities.GetUserProfile(currentUser, _db);

            return Ok(result);
        }

        [HttpGet("me/data")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDataViewModel))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<UserDataViewModel> GetUserData()
        {
            var currentUser = DbUtilities.GetUserFromContext(HttpContext, _db);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var result = DbUtilities.GetUserData(currentUser);

            return Ok(result);
        }

        [HttpPatch("me/data")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDataViewModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IEnumerable<Error>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<UserDataViewModel> UpdateUserData(UserDataUpdateRequest request)
        {
            var currentUser = DbUtilities.GetUserFromContext(HttpContext, _db);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            if (!request.IsValid(out var errors))
            {
                return BadRequest(errors);
            }

            request.UpdateUser(currentUser);

            _db.SaveChanges();

            var result = DbUtilities.GetUserData(currentUser);

            return Ok(result);
        }

        [HttpPut("me/username")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(Error))]
        public ActionResult ChangeUsername([FromBody] string newUsername)
        {
            var currentUser = DbUtilities.GetUserFromContext(HttpContext, _db);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            // if the username is the same as before:
            if (newUsername.Equals(currentUser.Username))
            {
                return Ok();
            }

            // if the username is already taken:
            if (DbUtilities.IsUsernameAlreadyInUser(_db, newUsername))
            {
                return Conflict(Error.UsernameAlreadyInUse);
            }

            currentUser.Username = newUsername;
            _db.SaveChanges();

            return Ok();
        }

        [HttpPut("me/password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(Error))]
        public ActionResult ChangePassword([FromBody] PasswordUpdateRequest request)
        {
            var currentUser = DbUtilities.GetUserFromContext(HttpContext, _db);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            // if the old password was wrong:
            if (!PasswordUtilities.VerifyPassword(request.OldPassword, currentUser.PasswordHash))
            {
                return Conflict(Error.PasswordIncorrect);
            }

            currentUser.PasswordHash = PasswordUtilities.ComputePasswordHash(request.NewPassword);
            _db.SaveChanges();

            return Ok();
        }

        [HttpGet("me/followers")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserDTO>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<IEnumerable<UserDTO>> GetFollowers()
        {
            var targetUser = DbUtilities.GetUserFromContext(HttpContext, _db);
            if (targetUser == null)
            {
                return Unauthorized();
            }

            _db.Entry(targetUser).Collection(u => u.Follower).Load();

            var result = targetUser.Follower.Select(DbUtilities.GetUserDTO);

            return Ok(result);
        }

        [HttpGet("me/followers/count")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<int> GetFollowerCount()
        {
            var targetUser = DbUtilities.GetUserFromContext(HttpContext, _db);
            if (targetUser == null)
            {
                return Unauthorized();
            }

            _db.Entry(targetUser).Collection(u => u.Follower).Load();

            var result = targetUser.Follower.Count();

            return Ok(result);
        }

        [HttpGet("me/following")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserDTO>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<IEnumerable<UserDTO>> GetFollowing()
        {
            var targetUser = DbUtilities.GetUserFromContext(HttpContext, _db);
            if (targetUser == null)
            {
                return Unauthorized();
            }

            _db.Entry(targetUser).Collection(u => u.Following).Load();

            var result = targetUser.Following.Select(DbUtilities.GetUserDTO);

            return Ok(result);
        }

        [HttpGet("me/following/count")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<int> GetFollowingCount()
        {
            var targetUser = DbUtilities.GetUserFromContext(HttpContext, _db);
            if (targetUser == null)
            {
                return Unauthorized();
            }

            _db.Entry(targetUser).Collection(u => u.Following).Load();

            var result = targetUser.Following.Count();

            return Ok(result);
        }

        [HttpPost("me/following/{targetUsername}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserProfileViewModel))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
        public ActionResult<UserProfileViewModel> FollowUser([FromRoute] string targetUsername)
        {
            var sourceUser = DbUtilities.GetUserFromContext(HttpContext, _db);
            var targetUser = _db.Users.FirstOrDefault(u => u.Username == targetUsername);

            if (sourceUser == null)
            {
                return Unauthorized();
            }

            if (targetUser == null)
            {
                return NotFound(Error.UserNotFound);
            }

            if (!(sourceUser == targetUser))
            {
                _db.Entry(sourceUser).Collection(u => u.Following).Load();
                sourceUser.Following.Add(targetUser);
                _db.SaveChanges();
            }

            var result = DbUtilities.GetUserProfile(targetUser, _db);

            return Ok(result);
        }

        [HttpDelete("me/following/{targetUsername}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserProfileViewModel))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
        public ActionResult<UserProfileViewModel> UnfollowUser([FromRoute] string targetUsername)
        {
            var sourceUser = DbUtilities.GetUserFromContext(HttpContext, _db);
            var targetUser = _db.Users.FirstOrDefault(u => u.Username == targetUsername);

            if (sourceUser == null)
            {
                return Unauthorized();
            }

            if (targetUser == null)
            {
                return NotFound(Error.UserNotFound);
            }

            _db.Entry(sourceUser).Collection(u => u.Following).Load();
            sourceUser.Following.Remove(targetUser);
            _db.SaveChanges();

            var result = DbUtilities.GetUserProfile(targetUser, _db);

            return Ok(result);
        }
        #endregion me

        #region status
        /// <summary>
        /// This endpoint always returns true, but only for authorized i.e. logged in users. Can be used to check if a users JWT is still valid.
        /// </summary>
        /// <returns>true, always</returns>
        [HttpGet("status")]
        [Authorize]
        public bool Status()
        {
            return true;
        }
        #endregion status
    }
}
