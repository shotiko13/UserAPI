using Hw4.Data;
using Hw4.Models;
using Hw4.Models.UserViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Hw4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UsersController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(x => x.Value.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(new { Errors = errors });
            }

            var user = new ApplicationUser
            {
                UserName = model.Name,
                Email = model.EmailAddress,
                RegistrationTime = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok(new { Message = "User created successfully", UserId = user.Id });
            }
            else
            {
                var identityErrors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest(new { Errors = identityErrors });
            }

        }

   
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(x => x.Value.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(new { Errors = errors });
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    return Ok(new { Message = "Login successful", User = user });
                }
            }

            return Unauthorized(new { Message = "Invalid login attempt" });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = _userManager.Users;

            return Ok(users);
        }

        [HttpPost("block/{id}")]
        public async Task<IActionResult> BlockUser(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null) 
            {
                return NotFound();
            }

            user.Status = UserStatus.Blocked;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(user);
            }

            var identityErrors = result.Errors.Select(e => e.Description).ToList();
            return BadRequest(new { Errors = identityErrors });
        }

        [HttpPost("unblock/{id}")]
        public async Task<IActionResult> UnblockUser(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return NotFound();
            }

            user.Status = UserStatus.Active;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return Ok(new { Message = "User deleted successfully" });
            }

            var identityErrors = result.Errors.Select(e => e.Description).ToList();
            return BadRequest(new { Errors = identityErrors });
        }

    }
}
