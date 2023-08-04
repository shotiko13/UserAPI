using Hw4.Data;
using Hw4.Models;
using Hw4.Models.UserViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                RegistrationTime = DateTime.Now,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                user.Status = UserStatus.Active;
                var updateResult = await _userManager.UpdateAsync(user);

                if (!updateResult.Succeeded)
                {
                    var identityErrors = updateResult.Errors.Select(e => e.Description).ToList();
                    return BadRequest(new { Errors = identityErrors });
                }
                return Ok(new { Message = "User created successfully", UserId = user.Id });
            }
            else
            {
                var identityErrors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest(new { Errors = identityErrors });
            }

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            ApplicationUser user;
            if (model.EmailOrUsername.Contains('@'))
            {
                user = await _userManager.FindByEmailAsync(model.EmailOrUsername);
            }
            else
            {
                user = await _userManager.FindByNameAsync(model.EmailOrUsername);
            }

            if (user == null)
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }

            if (user.Status == UserStatus.Blocked)
            {
                return StatusCode(403, new { message = "You are blocked" });
            }

            if (user != null)
            {
                var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                if (result.Succeeded)
                {
                    user.LastLoginTime = DateTime.Now;

                    var updateResult = await _userManager.UpdateAsync(user);

                    if (!updateResult.Succeeded)
                    {
                        var identityErrors = updateResult.Errors.Select(e => e.Description).ToList();
                        return BadRequest(new { Errors = identityErrors });
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return Ok();
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");

            return BadRequest(ModelState);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }
            var userDetailViewModel = new UserDetailViewModel
            {
                Id = user.Id,
                Name = user.UserName,
                Email = user.Email,
                RegistrationTime = user.RegistrationTime,
                LastLoginTime = user.LastLoginTime,
                Status = user.Status
            };

            return Ok(userDetailViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();

            var userDetailViewModels = users.Select(user => new UserDetailViewModel
            {
                Id = user.Id,
                Name = user.UserName,
                Email = user.Email,
                LastLoginTime = user.LastLoginTime,
                RegistrationTime = user.RegistrationTime,
                Status = user.Status,
            }).ToList();

            return Ok(userDetailViewModels);
        }

        [HttpPost("block/{id}")]
        public async Task<IActionResult> BlockUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

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
        public async Task<IActionResult> UnblockUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

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

        [HttpPost("delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

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
