using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostlyApi.Enums;
using PostlyApi.Models;
using PostlyApi.Models.DTOs;
using PostlyApi.Models.DTOs.FilterModels;
using PostlyApi.Utilities;

namespace PostlyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly PostlyContext _db;

        public SearchController(PostlyContext dbContext)
        {
            _db = dbContext;
        }

        /// <summary>
        /// Returns a list of users matching the given username
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserDTO>))]
        public IEnumerable<UserDTO> Get(
            [FromQuery] string username = "",
            [FromQuery] bool limited = false)
        {
            IEnumerable<UserDTO> result;

            if (limited)
            {
                result = _db.Users
                    .Where(_ => _.Username.StartsWith(username))
                    .OrderBy(_ => _.Username)
                    .Select(_ => DbUtilities.GetUserDTO(_))
                    .AsEnumerable();
            }
            else
            {
                result = _db.Users
                    .Where(_ => _.Username.Contains(username))
                    .OrderBy(_ => _.Username)
                    .Select(_ => DbUtilities.GetUserDTO(_))
                    .AsEnumerable();

                var prio1 = result
                    .Where(_ => _.Username.StartsWith(username, StringComparison.OrdinalIgnoreCase));

                var prio2 = result
                    .Where(_ => !_.Username.StartsWith(username, StringComparison.OrdinalIgnoreCase));

                result = prio1.Concat(prio2);
            }

            return result;
        }

        /// <summary>
        /// Returns a list of users matching the given filter parameters
        /// </summary>
        [HttpGet("filter")]
        [Authorize(Roles = nameof(Role.Admin) + "," + nameof(Role.Moderator))]
        public IEnumerable<UserDTO> GetFiltered([FromQuery] UserFilterModel filter)
        {
            var result = filter
                .GetMatches(_db.Users)
                .Select(_ => DbUtilities.GetUserDTO(_));

            return result;
        }
    }
}
