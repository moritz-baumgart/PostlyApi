using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostlyApi.Enums;
using PostlyApi.Models;
using PostlyApi.Utilities;

namespace PostlyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = nameof(Role.Admin) + "," + nameof(Role.Moderator))]
    public class StatisticController : ControllerBase
    {
        private readonly PostlyContext _db;

        public StatisticController(PostlyContext dbContext)
        {
            _db = dbContext;
        }

        #region user statistics

        /// <summary>
        /// Returns the total number of users
        /// </summary>
        [HttpGet("user/total")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<int> GetUsersTotal()
        {

            var result = _db.Users.Count();
            return Ok(result);
        }

        /// <summary>
        /// Returns the number of users per day for a given timespan
        /// </summary>
        [HttpGet("user/perday")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDictionary<DateTime, int>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<IDictionary<DateTime, int>> GetUsersPerDay(
            [FromQuery] DateTimeOffset? start,
            [FromQuery] DateTimeOffset? end)
        {
            var endDate = (end == null) ? DateTimeOffset.UtcNow.Date : end.Value.Date;
            var startDate = (start == null) ? endDate.AddMonths(-1) : start.Value.Date;

            var result = _db.Users
                .Where(_ => _.CreatedAt.Date >= startDate && _.CreatedAt.Date <= endDate)
                .GroupBy(_ => _.CreatedAt.Date)
                .ToDictionary(g => g.Key, g => g.Count());

            return Ok(result);
        }

        /// <summary>
        /// Returns the gender distribution in users
        /// </summary>
        [HttpGet("user/gender")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDictionary<Gender, int>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<IDictionary<Gender, int>> GetGenderCounts()
        {
            var result = _db.Users
                .GroupBy(u => u.Gender)
                .ToDictionary(g => g.Key, g => g.Count());

            return Ok(result);
        }

        #endregion


        #region post statistics

        /// <summary>
        /// Returns the total number of posts
        /// </summary>
        [HttpGet("post/total")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<int> GetPostsTotal()
        {
            var result = _db.Posts.Count();

            return result;
        }

        /// <summary>
        /// Returns the number of posts per day in a given timespan
        /// </summary>
        [HttpGet("post/perday")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDictionary<DateTime, int>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<IDictionary<DateTime, int>> GetPostsPerDay(
            [FromQuery] DateTimeOffset? start,
            [FromQuery] DateTimeOffset? end)
        {
            var endDate = (end == null) ? DateTimeOffset.UtcNow.Date : end.Value.Date;
            var startDate = (start == null) ? endDate.AddMonths(-1) : start.Value.Date;

            var result = _db.Posts
                .Where(_ => _.CreatedAt.Date >= startDate && _.CreatedAt.Date <= endDate)
                .GroupBy(_ => _.CreatedAt.Date)
                .ToDictionary(g => g.Key, g => g.Count());

            return Ok(result);
        }

        #endregion


        #region comment statistics

        /// <summary>
        /// Returns the total number of comments
        /// </summary>
        [HttpGet("comment/total")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<int> GetCommentsTotal()
        {
            var result = _db.Comments.Count();

            return Ok(result);
        }

        /// <summary>
        /// Returns the number of comments per day for a given timespan
        /// </summary>
        [HttpGet("comment/perday")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDictionary<DateTime, int>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<IDictionary<DateTime, int>> GetCommentsPerDay(
            [FromQuery] DateTimeOffset? start, 
            [FromQuery] DateTimeOffset? end)
        {
            var endDate = (end == null) ? DateTimeOffset.UtcNow.Date : end.Value.Date;
            var startDate = (start == null) ? endDate.AddMonths(-1) : start.Value.Date;

            var result = _db.Comments
                .Where(_ => _.CreatedAt.Date >= startDate && _.CreatedAt.Date <= endDate)
                .GroupBy(_ => _.CreatedAt.Date)
                .ToDictionary(g => g.Key, g => g.Count());

            return Ok(result);
        }

        #endregion


        #region login statistics

        /// <summary>
        /// Returns the total number of logins
        /// </summary>
        [HttpGet("login/total")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<int> GetLoginsTotal()
        {
            var result = _db.Logins.Count();

            return Ok(result);
        }

        /// <summary>
        /// Returns the number of logins per day in a given timespan
        /// </summary>
        [HttpGet("login/perday")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDictionary<DateTime, int>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<IDictionary<DateTime, int>> GetLoginsPerDay([FromQuery] DateTimeOffset? start, DateTimeOffset? end)
        {
            var endDate = (end == null) ? DateTimeOffset.UtcNow.Date : end.Value.Date;
            var startDate = (start == null) ? endDate.AddMonths(-1) : start.Value.Date;

            var result = _db.Logins
                .Where(_ => _.CreatedAt.Date >= startDate && _.CreatedAt.Date <= endDate)
                .GroupBy(_ => _.CreatedAt.Date)
                .ToDictionary(g => g.Key, g => g.Count());

            return Ok(result);
        }

        #endregion
    }
}
