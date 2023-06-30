using PostlyApi.Entities;
using PostlyApi.Enums;
using PostlyApi.Models.DTOs.BaseModels;
using System.Linq.Expressions;

namespace PostlyApi.Models.DTOs.FilterModels
{
    public class UserFilterModel : BaseFilterModel<User>
    {
        public string Username { get; set; } = "";
        public IEnumerable<Role> Roles { get; set; } = new List<Role>();
        public IEnumerable<Role> NotRoles { get; set; } = new List<Role>();
        public IEnumerable<Gender> Genders { get; set; } = new List<Gender>();
        public IEnumerable<Gender> NotGenders { get; set; } = new List<Gender>();

        public override Expression<Func<User, bool>> GetExpression()
        {
            return _ =>
                _.Username.Contains(Username) &&
                (!Roles.Any() || Roles.Contains(_.Role)) &&
                !NotRoles.Contains(_.Role) &&
                (!Genders.Any() || Genders.Contains(_.Gender)) && 
                !NotGenders.Contains(_.Gender);
        }
    }
}
