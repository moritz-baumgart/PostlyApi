using Microsoft.AspNetCore.Mvc;
using PostlyApi.Models;
using PostlyApi.Models.DTOs;
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

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserDTO>))]
        public IEnumerable<UserDTO> GetUser([FromQuery] string username)
        {
            var result = _db.Users
                .Where(_ => _.Username.StartsWith(username))
                .OrderBy(_ => _.Username)
                .Select(_ => DbUtilities.GetUserDTO(_));

            return result;
        }
    }
}
