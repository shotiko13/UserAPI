using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Hw4.Models
{
    public enum UserStatus
    {
        Active,
        Blocked,
    }
    public class ApplicationUser : IdentityUser
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public DateTime? RegistrationTime { get; set; }

        [Required]
        public UserStatus Status { get; set; }

    }

}
