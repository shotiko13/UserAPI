using System.ComponentModel.DataAnnotations;

namespace Hw4.Models.UserViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress] 
        public string EmailAddress { get; set;}

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set;}

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The Passwords do not match")]
        public string ConfirmPassword { get; set;}
    }
}
