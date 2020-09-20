using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiCadastro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiCadastro.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class CadastroController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<tbCadastro> Get()
        {
            List<tbCadastro> listaCadastro = new List<tbCadastro>()
            {
                new tbCadastro(){Id = Guid.NewGuid(), Descricao = "Cadastro 1"},
                new tbCadastro(){Id = Guid.NewGuid(), Descricao = "Cadastro 2"},
                new tbCadastro(){Id = Guid.NewGuid(), Descricao = "Cadastro 3"},
                new tbCadastro(){Id = Guid.NewGuid(), Descricao = "Cadastro 4"}
            };

            return listaCadastro;
        }
    }
}
