using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using WebApp.Data.Account;
using WebApp.Models;
using WebApp.Service;

namespace WebApp.Controllers
{
    public class AccountController : Controller
    {


        [BindProperty]
        public CredentialViewModel CredentialViewModel { get; set; }

        [BindProperty]
        public string? EmailMFAToken { get; set; }

        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;

        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.CredentialViewModel = new CredentialViewModel();
        }



        public async Task<IActionResult> Login()
        {
            ViewBag.ExternalLoginSchemes = await signInManager.GetExternalAuthenticationSchemesAsync();
            return View(CredentialViewModel);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> LoginPost()
        {
            if (!ModelState.IsValid)
            {
                return View("Login");
            }

            var result = await signInManager.PasswordSignInAsync(CredentialViewModel.Email,
                                                CredentialViewModel.Password,
                                                CredentialViewModel.RememberMe,
                                                false);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("Login", "You are locked out.");
                }
                else if (result.RequiresTwoFactor)
                {
                    //Email 2FA

                    //return RedirectToAction("TwoFactorToken", "TwoFactorAuth", new
                    //{
                    //    Email = CredentialViewModel.Email,
                    //    RememberMe = CredentialViewModel.RememberMe
                    //});

                    return RedirectToAction("AuthenticatorWIthToken", "AuthenticatorWithToken", new
                    {
                        RememberMe = CredentialViewModel.RememberMe
                    });
                }
                else
                {
                    ModelState.AddModelError("Login", "Failed to login.");
                }
                return View("Login");
            }
            else
            {
                //ModelState.AddModelError("Login", "Failed to login because 2FA is enabled.");

            }
            return RedirectToAction("Index", "Home");
        }


        public IActionResult ExternalLogin(string provider)
        {
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, null);
            properties.RedirectUri = Url.Action("ExternalLoginRedirect", "Account");

            return Challenge(properties, provider);
        }

        public async Task<IActionResult> ExternalLoginRedirect()
        {
            var loginInfo = await signInManager.GetExternalLoginInfoAsync();

            if (loginInfo != null)
            {
                var emailClaim = loginInfo.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
                var userClaim = loginInfo.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);
                var user = new User
                {
                    UserName = userClaim?.Value ?? string.Empty,
                    Email = emailClaim?.Value ?? string.Empty
                };

                var checkedUser = await userManager.FindByEmailAsync(user.Email);

                if (checkedUser != null)
                {
                    await signInManager.SignInAsync(checkedUser, false);
                }
                else
                {
                    return RedirectToAction("RegisterUser", "Register");   
                }

            }

            return RedirectToAction("Index", "Home");
        }
    }
}
