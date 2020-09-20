using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WebTeste3.MVC.Models;

namespace WebTeste3.MVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly  ApiTeste2 _apiTeste2;
        private readonly IHttpContextAccessor _http;
        public HomeController(ILogger<HomeController> logger, 
            IOptions<ApiTeste2> apiTeste2, 
            IHttpContextAccessor http)
        {

            _logger = logger;
            _apiTeste2 = apiTeste2.Value;
            _http = http;
        }

        [Authorize(Roles = "usuario, admin")]
        public IActionResult Index()
        {
            var usuario = User;
            var token = HttpContext.GetTokenAsync("access_token");


            var client = new HttpClient();
            client.ClientApi(_apiTeste2.BaseUrl, _http.HttpContext.GetTokenAsync("access_token").Result);
            var lista = new List<tbTeste2>();
            HttpResponseMessage response = client.GetAsync("api/Teste2").Result;

            if (response.IsSuccessStatusCode)
            {
                lista = JsonConvert.DeserializeObject<List<tbTeste2>>(response.Content.ReadAsStringAsync().Result);
            }


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
    }
}
