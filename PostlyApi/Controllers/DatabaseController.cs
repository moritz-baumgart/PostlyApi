using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostlyApi.Enums;
using PostlyApi.Models;
using PostlyApi.Models.DTOs;

namespace PostlyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = nameof(Role.Admin))]
    public class DatabaseController : ControllerBase
    {
        private readonly PostlyContext _db;

        public DatabaseController(PostlyContext dbContext)
        {
            _db = dbContext;
        }


        /// <summary>
        /// Executes given SQL Query.
        /// </summary>
        /// <param name="query">The SQL Query to execute.</param>
        /// <returns>A <see cref="DatabaseOperationDTO"/> that contains the result.</returns>
        [HttpPost("Execute")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DatabaseOperationDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<DatabaseOperationDTO> Execute([FromBody] string query)
        {

            try
            {
                if (query.Trim().StartsWith("select", StringComparison.OrdinalIgnoreCase))
                {
                    var cmd = _db.Database.GetDbConnection().CreateCommand();
                    cmd.CommandText = query;
                    _db.Database.OpenConnection();
                    var reader = cmd.ExecuteReader();

                    var columnNames = new List<string>();
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        columnNames.Add(reader.GetName(i));
                    }

                    List<List<string>> res = new();

                    while (reader.Read())
                    {
                        var row = new List<string>();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            row.Add(reader.GetValue(i).ToString() ?? "NULL");
                        }
                        res.Add(row);
                    }

                    return Ok(new DatabaseOperationDTO(null, columnNames, res));
                }
                else
                {
                    var numberOfAffectedRows = _db.Database.ExecuteSqlRaw(query);
                    return Ok(new DatabaseOperationDTO(numberOfAffectedRows, null, null));
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
