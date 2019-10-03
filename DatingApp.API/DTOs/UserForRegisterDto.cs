using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.DTOs
{
    // we validate the data in here because it doesn't make sense to do it
    // in the user class as there properties there that we can't validate such as
    // the password hash and salt, and the id which is generated by the DB
    public class UserForRegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "You must specify password between 4 and 8 characters")]
        public string Password { get; set; }
    }
}