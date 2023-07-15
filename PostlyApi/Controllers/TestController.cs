using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostlyApi.Models;

namespace PostlyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly PostlyContext _db;

        public TestController(PostlyContext dbContext)
        {
            _db = dbContext;
        }

        /// <summary>
        /// Endpoint for testing backend and database connection
        /// </summary>
        [HttpGet("count")]
        [Authorize]
        public ActionResult<string> Count()
        {
            return _db.Users.Count().ToString();
        }
    }
}
