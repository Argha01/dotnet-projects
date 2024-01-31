using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration configuration;
        public AuthController(IConfiguration configuration)
        {

            this.configuration = configuration;

        }
        [HttpPost]
        public async Task<IActionResult> Authenticate([FromBody] Credential credential)
        {
            if (credential.UserName == "admin" && credential.Password == "password")
            {
                var claims = new List<Claim>
                {
                    new (ClaimTypes.Name, "admin"),
                    new (ClaimTypes.Email, "admin@email.com"),
                    new ("admin", "true")
                };

                var expiresAt = DateTime.UtcNow.AddMinutes(10);

                return Ok(new
                {
                    access_token = CreateToken(claims,expiresAt),
                    expires_at = expiresAt
                });
            }

            ModelState.AddModelError("UnAuthorized", "You are not authorized to access e ndpoint");
            return Unauthorized(ModelState);
        }

        private string CreateToken(IEnumerable<Claim> claims, DateTime expiresAt) {
            var secretKey = Encoding.ASCII.GetBytes(this.configuration.GetValue<string>("Secretkey") ?? string.Empty);

            var jwt = new JwtSecurityToken(
                    claims: claims,
                    notBefore: DateTime.UtcNow,
                    expires: expiresAt,
                    signingCredentials: new SigningCredentials(
                        new SymmetricSecurityKey(secretKey),
                        SecurityAlgorithms.HmacSha256Signature
                    )
                );
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            return jwtSecurityTokenHandler.WriteToken(jwt);
        }
    }

    public class Credential
    {
        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
