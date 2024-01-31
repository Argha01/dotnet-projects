using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.Data.Account;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class AuthenticatorWithTokenController : Controller
    {
        [BindProperty]
        public AuthenticatorMFAViewModel AuthenticatorMFAViewModel { get; set; }
        private readonly SignInManager<User> signInManager;

        public AuthenticatorWithTokenController(SignInManager<User> signInManager) 
        {
            this.signInManager = signInManager;
            this.AuthenticatorMFAViewModel = new AuthenticatorMFAViewModel();
        }
        public IActionResult AuthenticatorWIthToken(bool rememberme)
        {
            this.AuthenticatorMFAViewModel.RememberMe = rememberme;
            this.AuthenticatorMFAViewModel.SecurityCode = string.Empty;

            return View();
        }

        public async Task<IActionResult> AuthenticatorSignIn()
        {
            if(!ModelState.IsValid) return View("AuthenticatorWIthToken");

            var result = await signInManager.TwoFactorAuthenticatorSignInAsync(AuthenticatorMFAViewModel.SecurityCode, AuthenticatorMFAViewModel.RememberMe, false);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("LoginMFA", "You are locked out.");
                }
                else
                {
                    ModelState.AddModelError("LoginMFA", "Failed to login.");
                }
                return View("AuthenticatorWIthToken");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
