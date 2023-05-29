using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PostlyApi.Entities;
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
            var user = _db.Users.Single(u => u.Username.Equals(request.Username));

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
            if (_db.Users.Any(u => u.Username == request.Username))
            {
                return new SuccessResult<object, RegisterError>(false, RegisterError.UsernameAlreadyInUse);
            }

            // Otherwise add a new user and return success
            _db.Users.Add(new User(request.Username, PasswordUtilities.ComputePasswordHash(request.Password)));
            _db.SaveChanges();

            return new SuccessResult<object, RegisterError>(true, RegisterError.None);
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
