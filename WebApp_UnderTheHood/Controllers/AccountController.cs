using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp_UnderTheHood.Models;

namespace WebApp_UnderTheHood.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> logger;

        public AccountController(ILogger<AccountController> logger)
        {
            this.logger = logger;
        }
        [BindProperty]
        public CredentialViewModel CredentialViewModel { get; set; } = new CredentialViewModel();

        public async Task<IActionResult> Login() {
            logger.LogInformation("Login Action");
            await DemoFunction(logger);
            return View();
        }
        private static async Task DemoFunction(ILogger<AccountController> log)
        {
            log.LogInformation("Static Method");
        }
        public async Task<IActionResult> AccessDenied() => View();

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Index","Home");
        }

        [HttpPost]
        public async Task<IActionResult> Login(CredentialViewModel CredentialViewModel)
        {
            if (!ModelState.IsValid) return  View("Login", CredentialViewModel);
            logger.LogInformation($"Remember me {CredentialViewModel.RememberMe}");
            if (CredentialViewModel.UserName == "admin" && CredentialViewModel.Password == "password")
            {
                var claims = new List<Claim>
                {
                    new (ClaimTypes.Name, "admin"),
                    new (ClaimTypes.Email, "admin@email.com"),
                    new ("admin", "true")
                };

                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                var claimsPrincipal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal, new AuthenticationProperties { IsPersistent = CredentialViewModel.RememberMe });
                return RedirectToAction("Index", "Home");
            }

            if (CredentialViewModel.UserName == "hradmin" && CredentialViewModel.Password == "password")
            {
                var claims = new List<Claim>
                {
                    new (ClaimTypes.Name, "hradmin"),
                    new (ClaimTypes.Email, "hradmin@email.com"),
                    new ("hradmin","true"),
                    new ("Department", "HR"),
                    new ("EmploymentDate", "2022-05-04")
                };

                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                var claimsPrincipal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal , new AuthenticationProperties { IsPersistent = CredentialViewModel.RememberMe  } );

                return RedirectToAction("HumanResource", "Home");
            }

            return RedirectToAction("Login");
        }
    }
}
