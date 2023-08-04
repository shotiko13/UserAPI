using Hw4.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;

namespace Hw4.Middleware
{
    public class CheckBlockedMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;

        public CheckBlockedMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                var blockedUserResponse = await CheckUserAndTerminateSessionIfBlocked(userId, context);
                if (blockedUserResponse) return;
            }
            await _next(context);
        }

        private async Task<bool> CheckUserAndTerminateSessionIfBlocked(string userId, HttpContext context)
        {
            using var scope = _serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByIdAsync(userId);
            if (user == null || user.Status != UserStatus.Blocked)
                return false;

            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("User is blocked");
            return true;
        }
    }
}
