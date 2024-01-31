using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp.Data.Account;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        [BindProperty]
        public UserProfileViewModel UserProfileViewModel { get; set; }

        private readonly UserManager<User> userManager;

        
        public UserProfileController(UserManager<User> userManager)
        {
            this.userManager = userManager;
            this.UserProfileViewModel = new UserProfileViewModel();
        }
        public async Task<IActionResult> UserProfile()
        {
            ViewBag.SuccessMessage = string.Empty;
            var user = await userManager.FindByNameAsync(User.Identity?.Name ?? string.Empty);
            if (user != null)
            {
                var claims = await userManager.GetClaimsAsync(user);
                var departmentClaim = claims.FirstOrDefault(x => x.Type == "Department");
                var positionClaim = claims.FirstOrDefault(x => x.Type == "Position");

                UserProfileViewModel = new UserProfileViewModel
                {
                    Email = User.Identity?.Name ?? string.Empty,
                    Department = departmentClaim?.Value ?? string.Empty,
                    Position = positionClaim?.Value ?? string.Empty,
                    EnableTwoFactorAuth = user.TwoFactorEnabled
                };
            }

            return View(UserProfileViewModel);
        }

        public async Task<IActionResult> SaveUserProfile()
        {
            if (!ModelState.IsValid)
            {
                return View("UserProfile");
            }

            try
            {
                var user = await userManager.FindByNameAsync(User.Identity?.Name ?? string.Empty);
                if (user != null)
                {
                    var claims = await userManager.GetClaimsAsync(user);
                    var departmentClaim = claims.FirstOrDefault(x => x.Type == "Department");
                    var positionClaim = claims.FirstOrDefault(x => x.Type == "Position");

                    if (departmentClaim != null)
                    {
                        await userManager.ReplaceClaimAsync(user, departmentClaim,
                                                        new Claim(departmentClaim.Type, UserProfileViewModel.Department));
                    }

                    if (positionClaim != null)
                    {
                        await userManager.ReplaceClaimAsync(user, positionClaim,
                                                        new Claim(positionClaim.Type, UserProfileViewModel.Position));
                    }

                    user.TwoFactorEnabled = UserProfileViewModel.EnableTwoFactorAuth;
                    
                    if (!UserProfileViewModel.EnableTwoFactorAuth)
                    {
                        await userManager.RemoveAuthenticationTokenAsync(user, "[AspNetUserStore]", "AuthenticatorKey");
                    }

                    await userManager.UpdateAsync(user);
                    ViewBag.SuccessMessage = "Your profile is updated successfully.";
                }
            }
            catch
            {
                ModelState.AddModelError("UserProfile", "Error occured during updating profile.");
            }

            return View("UserProfile", UserProfileViewModel);
        }
    }
}
