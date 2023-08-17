using System.Security.Claims;
using System.Web.Mvc;
using Hw4.Data;
using Hw4.Models;
using Hw4.Models.UserViewModels;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ControllerBase = Microsoft.AspNetCore.Mvc.ControllerBase;
using ModelStateDictionary = Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary;

namespace Hw4.Controllers
{
    [EnableCors("AllowAll")]
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public UsersController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            var modelStateCheck = CheckModelState(ModelState);
            if (!modelStateCheck.IsValid)
                return BadRequest(new { Errors = modelStateCheck.Errors });
            var user = CreateNewUser(model);
            var result = await _userManager.CreateAsync(user, model.Password);
            return await ProcessCreationResult(result, user);
        }

        private ModelCheckResult CheckModelState(ModelStateDictionary modelState)
        {
            if (modelState.IsValid)
                return new ModelCheckResult { IsValid = true };
            var errors = ModelState
                .SelectMany(x => x.Value.Errors.Select(e => e.ErrorMessage))
                .ToList();
            return new ModelCheckResult { IsValid = false, Errors = errors };
        }

        private ApplicationUser CreateNewUser(RegisterViewModel model)
        {
            return new ApplicationUser
            {
                UserName = model.Name,
                Email = model.EmailAddress,
                RegistrationTime = DateTime.Now,
            };
        }

        private async Task<IActionResult> ProcessCreationResult(IdentityResult result, ApplicationUser user)
        {
            if (result.Succeeded)
                return await UpdateUserStatus(user);
            return GetErrorResult(result);
        }

        private async Task<IActionResult> UpdateUserStatus(ApplicationUser user)
        {
            user.Status = UserStatus.Active;
            var updateResult = await _userManager.UpdateAsync(user);
            if (updateResult.Succeeded)
                return Ok(new { Message = "User created successfully", UserId = user.Id });
            return GetErrorResult(updateResult);
        }

        private IActionResult GetErrorResult(IdentityResult result)
        {
            var identityErrors = result.Errors.Select(e => e.Description).ToList();
            return BadRequest(new { Errors = identityErrors });
        }

        private class ModelCheckResult
        {
            public bool IsValid { get; set; }
            public List<string> Errors { get; set; }
        }


        [Microsoft.AspNetCore.Mvc.HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var modelStateCheck = CheckModelState(ModelState);
            if (!modelStateCheck.IsValid)
                return BadRequest(new { Errors = modelStateCheck.Errors });
            var user = await FindUser(model.EmailOrUsername);
            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });
            if (user.Status == UserStatus.Blocked)
                return StatusCode(403, new { message = "Your account is blocked" });
            return await ProcessSignIn(user, model.Password);
        }

        private async Task<ApplicationUser> FindUser(string emailOrUsername)
        {
            if (emailOrUsername.Contains('@'))
                return await _userManager.FindByEmailAsync(emailOrUsername);
            return await _userManager.FindByNameAsync(emailOrUsername);
        }

        private async Task<IActionResult> ProcessSignIn(ApplicationUser user, string password)
        {
            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (result.Succeeded) return await UpdateLastLoginTimeAndSignIn(user);
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return BadRequest(ModelState);
        }

        private async Task<IActionResult> UpdateLastLoginTimeAndSignIn(ApplicationUser user)
        {
            user.LastLoginTime = DateTime.Now;
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return GetErrorResult(updateResult);
            await _signInManager.SignInAsync(user, isPersistent: false);
            return Ok(new { Message = "Login successful" });
        }

        
        [Microsoft.AspNetCore.Mvc.HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userDetailViewModel = CreateUserDetailViewModel(user);
            return Ok(userDetailViewModel);
        }

        [Microsoft.AspNetCore.Mvc.HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();

            var userDetailViewModels = users.Select(user => CreateUserDetailViewModel(user)).ToList();

            return Ok(userDetailViewModels);
        }

        private UserDetailViewModel CreateUserDetailViewModel(ApplicationUser user)
        {
            return new UserDetailViewModel
            {
                Id = user.Id,
                Name = user.UserName,
                Email = user.Email,
                RegistrationTime = user.RegistrationTime,
                LastLoginTime = user.LastLoginTime,
                Status = user.Status
            };
        }

        
        [Microsoft.AspNetCore.Mvc.HttpPost("block/{id}")]
        public async Task<IActionResult> BlockUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();
            user.Status = UserStatus.Blocked;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return Ok(user);
            var identityErrors = result.Errors.Select(e => e.Description).ToList();
            return BadRequest(new { Errors = identityErrors });
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("unblock/{id}")]
        public async Task<IActionResult> UnblockUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();
            user.Status = UserStatus.Active;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return Ok();
            return BadRequest(result.Errors);
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
                return Ok(new { Message = "User deleted successfully" });
            var identityErrors = result.Errors.Select(e => e.Description).ToList();
            return BadRequest(new { Errors = identityErrors });
        }

    }
}
