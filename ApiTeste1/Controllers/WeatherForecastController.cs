using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ApiTeste1.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IHttpContextAccessor httpContext;
        private readonly ApiCadastro _appSettings;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, 
            IHttpContextAccessor httpContext,
            IOptions<ApiCadastro> appSettings)
        {
            _logger = logger;
            this.httpContext = httpContext;
            _appSettings = appSettings.Value;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            
            var client = new HttpClient();
            client.ClientApi(_appSettings.BaseUrl, this.httpContext.HttpContext.GetTokenAsync("access_token").Result);

            var dados = new List<WeatherForecast>();
            HttpResponseMessage response =  client.GetAsync("api/WeatherForecast").Result;
            
            if (response.IsSuccessStatusCode)
            {
                 dados = JsonConvert.DeserializeObject<List<WeatherForecast>>(response.Content.ReadAsStringAsync().Result);
            }

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
