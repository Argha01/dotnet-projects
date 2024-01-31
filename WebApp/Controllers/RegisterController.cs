using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp.Data.Account;
using WebApp.Models;
using WebApp.Service;

namespace WebApp.Controllers
{
    public class RegisterController : Controller
    {

        [BindProperty]
        public RegisterUserViewModel RegisterUserViewModel { get; set; }

        [BindProperty]
        public string? Message { get; set; }    

        private readonly UserManager<User> userManager;
        private readonly IEmailService emailService;
        public RegisterController(UserManager<User> userManager, IEmailService emailService)
        {
            this.userManager = userManager;
            this.emailService = emailService;
            this.RegisterUserViewModel = new RegisterUserViewModel();
        }

        public IActionResult RegisterUser()
        {
            return View();
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    Message = "Email address is confirmed";
                    return View(model: new {  Message });
                }
            }

            Message = "Failed  to validate email.";
            return View(model : new { Message });
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterUserViewModel RegisterUserViewModel)
        {
            if (!ModelState.IsValid) return View();

            var user = new User
            {
                Email = RegisterUserViewModel.Name,
                UserName = RegisterUserViewModel.Email,
                //Department = RegisterUserViewModel.Department,
                //Position = RegisterUserViewModel.Position
            };

            var claimDepartment = new Claim("Department", RegisterUserViewModel.Department);
            var claimPosition = new Claim("Position", RegisterUserViewModel.Position);

            var result = await userManager.CreateAsync(user, RegisterUserViewModel.Password);

            if (result.Succeeded)
            {
                await this.userManager.AddClaimsAsync(user, new[] { claimDepartment, claimPosition });

                var confirmationToken = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
                var url = Url.ActionLink(controller: "Register", action: "ConfirmEmail", values: new { userId = user.Id, token = confirmationToken }) ?? string.Empty;

                //var message = new MailMessage("argha.sen93@gmail.com",
                //    user.Email,
                //    "Confirmation mail",
                //    $"Please click on the below link to confirm {url}");

                //await emailService.SendEmailAsync(message);
                return Redirect(url);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Register", error.Description);
                }
            }

            return View();

        }
    }
}
