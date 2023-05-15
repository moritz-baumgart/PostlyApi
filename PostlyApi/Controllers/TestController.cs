using Microsoft.AspNetCore.Mvc;
using PostlyApi.Contexts;
using PostlyApi.Entities;

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

        [HttpGet]
        public ActionResult Get()
        {
            return Ok();
        }

        [HttpGet("new")]
        public ActionResult<String> New()
        {
            _db.Users.Add(new User("TestUser", "123"));
            _db.SaveChanges();

            return _db.Users.Count().ToString();
        }

        [HttpGet("count")]
        public ActionResult<String> Count()
        {
            return _db.Users.Count().ToString();
        }
    }
}
