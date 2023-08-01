using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hw4.Models
{
    public enum UserStatus
    {
        Active,
        Blocked,
    }
    public class ApplicationUser : IdentityUser
    {
        public DateTime? LastLoginTime { get; set; }

        public DateTime? RegistrationTime { get; set; }

        [Required]
        public UserStatus Status { get; set; }

    }

}
