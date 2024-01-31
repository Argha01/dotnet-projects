using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using WebApp_UnderTheHood.Models;

namespace WebApp_UnderTheHood.Controllers
{

    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory httpClientFactory;
        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            this.httpClientFactory = httpClientFactory; 
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }
        [Authorize("AdminOnly")]
        public async Task<IActionResult> Privacy()
        {
            return View();
        }

        [Authorize("BelongsToHRDepartment")]
        public async Task<IActionResult> HumanResource()
        {
            JwtToken tokenObject = null;
            var strtokenobject = HttpContext.Session.GetString("TOKEN");
            var client = httpClientFactory.CreateClient("WeatherApi");

            if (string.IsNullOrEmpty(strtokenobject))
            {   
                var res = await client.PostAsJsonAsync("Auth", new CredentialViewModel { UserName = "admin", Password = "password" });
                res.EnsureSuccessStatusCode();
                string strjwt = await res.Content.ReadAsStringAsync();
                tokenObject = JsonConvert.DeserializeObject<JwtToken>(strjwt)?? new JwtToken();
                HttpContext.Session.SetString("TOKEN", JsonConvert.SerializeObject(tokenObject));
            }
            else
            {
                tokenObject = JsonConvert.DeserializeObject<JwtToken>(strtokenobject) ?? new JwtToken();    
            }
            
            if(tokenObject == null ||
                string.IsNullOrWhiteSpace(tokenObject.access_token)
                || tokenObject.expires_at < DateTime.UtcNow) 
            {
                var res = await client.PostAsJsonAsync("Auth", new CredentialViewModel { UserName = "admin", Password = "password" });
                res.EnsureSuccessStatusCode();
                string strjwt = await res.Content.ReadAsStringAsync();
                tokenObject = JsonConvert.DeserializeObject<JwtToken>(strjwt) ?? new JwtToken();

            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenObject?.access_token);
            var weatherDTOs = await client.GetFromJsonAsync<IList<WeatherDTO>>("WeatherForecast") ?? new List<WeatherDTO>();
            
            return View(weatherDTOs);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
