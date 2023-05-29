using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PostlyApi.Entities;
using PostlyApi.Enums;
using PostlyApi.Models;
using PostlyApi.Models.Errors;
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

        /// <summary>
        /// Tries to login a user with given credentials and generates a jwt if the user exists and the provided password is correct.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>A <see cref="PostlyApi.Models.SuccessResult{T, E}"/> with true and the jwt as string if the login was sucessful, otherwise false and no value is returned.</returns>

        [HttpPost("Login")]
        public SuccessResult<string, object> Login([FromBody] LoginOrRegisterRequest request)
        {
            // Query the database for the user
            var user = _db.Users.FirstOrDefault(u => u.Username.Equals(request.Username));

            // If the user exists continue
            if (user != null)
            {

                // If the password was correct, generate the jwt
                if (PasswordUtilities.VerifyPassword(request.Password, user.PasswordHash))
                {

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
                    return new SuccessResult<string, object>(true, new JwtSecurityTokenHandler().WriteToken(token));
                }
            }

            // Return error if something was not right above
            return new SuccessResult<string, object>(false, "");
        }

        /// <summary>
        /// Registers a new user with given credentials.
        /// </summary>
        /// <param name="username">The username of the user, has to be available.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>A <see cref="PostlyApi.Models.SuccessResult{T, E}"/> with true and no value if the registration was successful, otherwise false and a <see cref="Models.Errors.RegisterError"/>.</returns>
        [HttpPost("register")]
        public SuccessResult<object, RegisterError> Register([FromBody] LoginOrRegisterRequest request)
        {
            // Check if the user already exists, if so return error
            if (_db.Users.Any(u => u.Username.Equals(request.Username)))
            {
                return new SuccessResult<object, RegisterError>(false, RegisterError.UsernameAlreadyInUse);
            }

            // Otherwise add a new user and return success
            _db.Users.Add(new User(request.Username, PasswordUtilities.ComputePasswordHash(request.Password)));
            _db.SaveChanges();

            return new SuccessResult<object, RegisterError>(true, RegisterError.None);
        }

        [HttpDelete("{userId}")]
        [Authorize]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteAccount([FromRoute] long userId)
        {
            var currentUser = DbUtilities.GetUserFromContext(HttpContext, _db);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var targetUser = _db.Users.FirstOrDefault(u => u.Id == userId);
            if (targetUser == null)
            {
                return NotFound(Result.UserNotFound.ToString());
            }

            // if the current user doesn't have permission to delete this account:
            if (!(currentUser == targetUser || currentUser.Role > 0))
            {
                return Forbid();
            }

            _db.Remove(targetUser);
            _db.SaveChanges();

            return Ok();
        }

        [HttpPut("{userId}/username")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult ChangeUsername([FromRoute] long userId, [FromBody] string newUsername)
        {
            var currentUser = DbUtilities.GetUserFromContext(HttpContext, _db);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var targetUser = _db.Users.FirstOrDefault(u => u.Id == userId);
            if (targetUser == null)
            {
                return NotFound(Result.UserNotFound.ToString());
            }

            // if the username is the same as before:
            if (newUsername.Equals(targetUser.Username))
            {
                return Ok();
            }

            // if the current user doesn't have permission to change this username:
            if (!(currentUser == targetUser || currentUser.Role > 0))
            {
                return Forbid();
            }

            // if the username is already taken:
            if (_db.Users.Any(u => u.Username.Equals(newUsername)))
            {
                return BadRequest(Result.UsernameAlreadyInUse.ToString());
            }

            targetUser.Username = newUsername;
            _db.SaveChanges();

            return Ok();
        }

        [HttpPut("{userId}/password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult ChangePassword([FromRoute] long userId, [FromBody] PasswordUpdateRequest request)
        {
            var currentUser = DbUtilities.GetUserFromContext(HttpContext, _db);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var targetUser = _db.Users.FirstOrDefault(u => u.Id == userId);
            if (targetUser == null)
            {
                return NotFound(Result.UserNotFound.ToString());
            }

            // if the current user doesn't have permission to change this password:
            if (!(currentUser.Id == userId || currentUser.Role > 0))
            {
                return Forbid();
            }

            // if the old password was wrong:
            if (!PasswordUtilities.VerifyPassword(request.OldPassword, targetUser.PasswordHash))
            {
                return BadRequest(Result.PasswordIncorrect.ToString());
            }

            targetUser.PasswordHash = PasswordUtilities.ComputePasswordHash(request.NewPassword);
            _db.SaveChanges();

            return Ok();
        }

        [HttpPut("{userId}/role")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateRole([FromRoute] long userId, [FromBody] Role role)
        {
            var currentUser = DbUtilities.GetUserFromContext(HttpContext, _db);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var targetUser = _db.Users.FirstOrDefault(u => u.Id == userId);
            if (targetUser == null)
            {
                return NotFound(Result.UserNotFound.ToString());
            }

            // if the current user is not an admin:
            if (currentUser.Role != Role.Admin)
            {
                return Forbid();
            }

            // if an admin tries to update themselves:
            if (currentUser == targetUser)
            {
                return Ok();
            }

            targetUser.Role = role;
            _db.SaveChanges();

            return Ok();
        }


        [HttpPut("{userId}/profile")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IEnumerable<ProfileUpdateError>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateProfile([FromRoute] long userId, ProfileUpdateRequest request)
        {
            var currentUser = DbUtilities.GetUserFromContext(HttpContext, _db);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var targetUser = _db.Users.FirstOrDefault(u => u.Id == userId);
            if (targetUser == null)
            {
                return NotFound(Result.UserNotFound.ToString());
            }

            // if the current user doesn't have permission to change this profile:
            if (!(currentUser.Id == userId || currentUser.Role > 0))
            {
                return Forbid();
            }

            List<ProfileUpdateError> errors = new();

            if (request.DisplayName != null)
            {
                // TODO: validation
                targetUser.DisplayName = request.DisplayName;
            }

            if (request.Email != null)
            {
                // TODO: validation
                targetUser.Email = request.Email;
            }

            if (request.PhoneNumber != null)
            {
                // TODO: validation
                targetUser.PhoneNumber = request.PhoneNumber;
            }

            if (request.Birthday != null)
            {
                // TODO: validation
                targetUser.Birthday = request.Birthday;
            }

            if (request.Gender != null)
            {
                // TODO: validation
                targetUser.Gender = request.Gender;
            }

            if (!errors.IsNullOrEmpty())
            {
                return BadRequest(errors);
            }

            _db.SaveChanges();

            return Ok();
        }

        [HttpDelete("me")]
        [Authorize]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult DeleteAccount()
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

        [HttpPut("me/username")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
            if (_db.Users.Any(u => u.Username.Equals(newUsername)))
            {
                return BadRequest(Result.UsernameAlreadyInUse.ToString());
            }

            currentUser.Username = newUsername;
            _db.SaveChanges();

            return Ok();
        }

        [HttpPut("me/password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
                return BadRequest(Result.PasswordIncorrect.ToString());
            }

            currentUser.PasswordHash = PasswordUtilities.ComputePasswordHash(request.NewPassword);
            _db.SaveChanges();

            return Ok();
        }

        [HttpPut("me/profile")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IEnumerable<ProfileUpdateError>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateProfile(ProfileUpdateRequest request)
        {
            var currentUser = DbUtilities.GetUserFromContext(HttpContext, _db);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            List<ProfileUpdateError> errors = new();

            if (request.DisplayName != null)
            {
                // TODO: validation
                currentUser.DisplayName = request.DisplayName;
            }

            if (request.Email != null)
            {
                // TODO: validation
                currentUser.Email = request.Email;
            }

            if (request.PhoneNumber != null)
            {
                // TODO: validation
                currentUser.PhoneNumber = request.PhoneNumber;
            }

            if (request.Birthday != null)
            {
                // TODO: validation
                currentUser.Birthday = request.Birthday;
            }

            if (request.Gender != null)
            {
                // TODO: validation
                currentUser.Gender = request.Gender;
            }

            if (!errors.IsNullOrEmpty())
            {
                return BadRequest(errors);
            }

            _db.SaveChanges();

            return Ok();
        }

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
    }
}
