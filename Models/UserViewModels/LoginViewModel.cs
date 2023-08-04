using System.ComponentModel.DataAnnotations;

namespace Hw4.Models.UserViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string EmailOrUsername { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }


    }
}
