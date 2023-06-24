using Microsoft.IdentityModel.Tokens;
using PostlyApi.Entities;
using PostlyApi.Enums;
using PostlyApi.Utilities;

namespace PostlyApi.Models.Requests
{
    public class UserDataUpdateRequest
    {
        // public data: 
        public string? DisplayName { get; set; }
        public DateTimeOffset? Birthday { get; set; }
        public Gender Gender { get; set; }

        // private data:
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }


        /// <summary>
        /// Validates all data of the request.
        /// </summary>
        /// <param name="errors">Out param that contains a list of validation erros.</param>
        /// <param name="dbContext">Some validations need database access, for these the dbContext should be passed.</param>
        /// <param name="targetUser">Some validations need the target user.></param>
        /// <returns>True if everything is valid, false otherwise.</returns>
        public bool IsValid(out List<Error> errors)
        {
            errors = new();

            if (Birthday != null && Birthday > DateTimeOffset.UtcNow)
            {
                errors.Add(Error.InvalidBirthday);
            }

            if (Email != null && Email.Length > 0 && !MiscUtilities.IsValidEmail(Email))
            {
                errors.Add(Error.InvalidEmail);
            }

            // TODO: Phone number validation

            return errors.IsNullOrEmpty();
        }

        /// <summary>
        /// Applies the changes of this update request to the given user.
        /// </summary>
        /// <param name="user">The user to be updated.</param>
        public void UpdateUser(User user)
        {
            user.DisplayName = DisplayName;
            user.Birthday = Birthday;
            user.Gender = Gender;
            user.Email = Email;
            user.PhoneNumber = PhoneNumber;
        }
    }
}
