using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ApiTeste2.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ApiTeste2.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class Teste2Controller : ControllerBase
    {


        private readonly ApiCadastro _apiCadastro;
        private readonly ApiTeste1 _apiTeste1;
        private readonly IHttpContextAccessor _http;

        public Teste2Controller(IOptions<ApiCadastro> apiCadastro,
            IOptions<ApiTeste1> apiTeste2,
            IHttpContextAccessor http)
        {
            _apiCadastro = apiCadastro.Value;
            _apiTeste1 = apiTeste2.Value;
            _http = http;
        }

        [HttpGet]
        public IEnumerable<tbTeste2> Get()
        {

            var claims = User.Claims;
            List<tbTeste2> lista = new List<tbTeste2>()
            {
                new tbTeste2(){Id = Guid.NewGuid(), Descricao = "Teste2 1"},
                new tbTeste2(){Id = Guid.NewGuid(), Descricao = "Teste2 2"},
                new tbTeste2(){Id = Guid.NewGuid(), Descricao = "Teste2 3"},
                new tbTeste2(){Id = Guid.NewGuid(), Descricao = "Teste2 4"}
            };

            return lista;
        }

        [Route("GetCadastro")]
        [HttpGet]
        public IEnumerable<tbCadastro> GetCadastro()
        {
            var client = new HttpClient();
            client.ClientApi(_apiCadastro.BaseUrl, _http.HttpContext.GetTokenAsync("access_token").Result);
            var lista = new List<tbCadastro>();
            HttpResponseMessage response = client.GetAsync("api/Cadastro").Result;

            if (response.IsSuccessStatusCode)
            {
                lista = JsonConvert.DeserializeObject<List<tbCadastro>>(response.Content.ReadAsStringAsync().Result);
            }

            return lista;
        }

        [Route("GetTeste1")]
        [HttpGet]
        public IEnumerable<tbTeste1> GetTeste1()
        {
            var client = new HttpClient();
            client.ClientApi(_apiTeste1.BaseUrl, _http.HttpContext.GetTokenAsync("access_token").Result);
            var lista = new List<tbTeste1>();
            HttpResponseMessage response = client.GetAsync("api/Teste1").Result;

            if (response.IsSuccessStatusCode)
            {
                lista = JsonConvert.DeserializeObject<List<tbTeste1>>(response.Content.ReadAsStringAsync().Result);
            }

            return lista;
        }
    }
}
