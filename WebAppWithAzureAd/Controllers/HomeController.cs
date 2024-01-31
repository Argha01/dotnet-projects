using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using System.Diagnostics;
using System.Net.Http.Headers;
using WebAppWithAzureAd.Models;

namespace WebAppWithAzureAd.Controllers
{
    [Authorize]
    public class HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, ITokenAcquisition tokenAcquisition) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("WeatherApi");
        private readonly ITokenAcquisition _tokenAcquisition = tokenAcquisition;

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> HumanResource()
        {
            var scopes = new[] { "api://9cbbdee6-fce8-4c1e-943e-833b7c2f2034/Weather.Read" };
            var access_token = await _tokenAcquisition.GetAccessTokenForUserAsync(scopes);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            var weatherDTOs = await _httpClient.GetFromJsonAsync<IList<WeatherDTO>>("WeatherForecast") ?? new List<WeatherDTO>();
            return View(weatherDTOs);
        }
    }
}
