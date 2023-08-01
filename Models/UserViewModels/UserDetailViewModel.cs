namespace Hw4.Models.UserViewModels
{
    public class UserDetailViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }   
        public string Email { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public DateTime? RegistrationTime { get; set; }
        public UserStatus Status { get; set; }
    }
}
