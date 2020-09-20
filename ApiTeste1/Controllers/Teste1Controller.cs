using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ApiTeste1.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ApiTeste1.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class Teste1Controller : ControllerBase
    {
        private readonly ApiCadastro _apiCadastro;
        private readonly ApiTeste2 _apiTeste2;
        private readonly IHttpContextAccessor _http;

        public Teste1Controller(IOptions<ApiCadastro> apiCadastro,
            IOptions<ApiTeste2> apiTeste2,
            IHttpContextAccessor http)
        {
            _apiCadastro = apiCadastro.Value;
            _apiTeste2 = apiTeste2.Value;
            _http = http;
        }

        [HttpGet]
       
        public IEnumerable<tbTeste1> Get()
        {

            List<tbTeste1> lista = new List<tbTeste1>()
            {
                new tbTeste1(){Id = Guid.NewGuid(), Descricao = "Teste1 1"},
                new tbTeste1(){Id = Guid.NewGuid(), Descricao = "Teste1 2"},
                new tbTeste1(){Id = Guid.NewGuid(), Descricao = "Teste1 3"},
                new tbTeste1(){Id = Guid.NewGuid(), Descricao = "Teste1 4"}
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

        [Route("GetTeste2")]
       
        [HttpGet]
        public IEnumerable<tbTeste2> GetTeste2()
        {
            var client = new HttpClient();
            client.ClientApi(_apiTeste2.BaseUrl, _http.HttpContext.GetTokenAsync("access_token").Result);
            var lista = new List<tbTeste2>();
            HttpResponseMessage response = client.GetAsync("api/Teste2").Result;

            if (response.IsSuccessStatusCode)
            {
                lista = JsonConvert.DeserializeObject<List<tbTeste2>>(response.Content.ReadAsStringAsync().Result);
            }

            return lista;
        }
    }
}
