using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using WebApp.Data.Account;
using WebApp.Models;
using static QRCoder.QRCodeGenerator;

namespace WebApp.Controllers
{
    [Authorize]
    public class AuthenticatorSetupController : Controller
    {
        [BindProperty]
        public SetupMFAViewModel SetupMFAViewModel { get; set; }

        private readonly UserManager<User> userManager;

        public AuthenticatorSetupController(UserManager<User> userManager)
        {
            this.userManager = userManager;
            this.SetupMFAViewModel = new SetupMFAViewModel();
        }

        public async Task<IActionResult> AuthenticatorSetupPost()
        {
            ViewBag.Succeeded = false;
            if (!ModelState.IsValid) return View(nameof(AuthenticatorSetup), SetupMFAViewModel);

            var user = await userManager.GetUserAsync(User);
            if (user != null && await userManager.VerifyTwoFactorTokenAsync(user, userManager.Options.Tokens.AuthenticatorTokenProvider, SetupMFAViewModel.SecurityCode))
            {
                await userManager.SetTwoFactorEnabledAsync(user, true);
                ViewBag.Succeeded = true;
            }
            else
            {
                ModelState.AddModelError("MFA", "Something went wrong with Authenticator Setup.");
                ViewBag.Succeeded = false;
            }

            return View(nameof(AuthenticatorSetup), SetupMFAViewModel);

        }
        public async Task<IActionResult> AuthenticatorSetup()
        {
            var user = await userManager.GetUserAsync(User);
            if (user != null)
            {
                await userManager.ResetAuthenticatorKeyAsync(user);
                var key = await userManager.GetAuthenticatorKeyAsync(user);
                SetupMFAViewModel.SecurityKey = key ?? string.Empty;
                SetupMFAViewModel.QRCode = GenerateQRCode(
                                            "my web app",
                                            SetupMFAViewModel.SecurityKey,
                                            user.Email ?? string.Empty);
            }
            ViewBag.Succeeded = false;
            return View(SetupMFAViewModel);
        }

        private byte[] GenerateQRCode(string provider, string key, string userEmail)
        {
            var qrCodeGenerator = new QRCodeGenerator();
            var qrCodeData = qrCodeGenerator.CreateQrCode($"otpauth://totp/{provider}:{userEmail}?secret={key}&issuer={provider}", ECCLevel.Q);

            var qrCode = new PngByteQRCode(qrCodeData);

            return qrCode.GetGraphic(20);
        }
    }
}
