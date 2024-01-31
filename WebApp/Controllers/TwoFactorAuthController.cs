using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using WebApp.Data.Account;
using WebApp.Models;
using WebApp.Service;

namespace WebApp.Controllers
{
    public class TwoFactorAuthController : Controller
    {
        private readonly UserManager<User> userManager;

        private readonly SignInManager<User> signInManager;

        private readonly IEmailService emailService;

        [BindProperty]
        public EmailMFAViewModel EmailMFA { get; set; }

        public TwoFactorAuthController(UserManager<User> userManager, SignInManager<User> signInManager, IEmailService emailService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailService = emailService;
            this.EmailMFA = new EmailMFAViewModel();
        }

        public async Task<IActionResult> TwoFactorToken(string Email, bool rememberme)
        {
            var user = await userManager.FindByNameAsync(Email);

            EmailMFA.RememberMe = rememberme;
            if (user != null)
            {
                var securityCode = await userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);
                
                if (securityCode != null)
                {
                    var message = new MailMessage("argha.sen93@gmail.com",
                                user?.Email,
                                "2FA Code",
                                $"Your authentication token {securityCode}");
                    await emailService.SendEmailAsync(message);
                }

            }
            return View();
        }

        public async Task<IActionResult> TwoFactorSignIn()
        {
            if(!ModelState.IsValid) return View("TwoFactorToken");

            var result = await signInManager.TwoFactorSignInAsync(TokenOptions.DefaultEmailProvider, EmailMFA.SecurityCode, EmailMFA.RememberMe, false);

            if(!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("Login2FA", "You are locked out.");
                }
                else
                {
                    ModelState.AddModelError("Login2FA", "Failed to login.");
                }
                return View("TwoFactorToken");
            }

            return RedirectToAction("Index", "Home");
        }
    }

}
