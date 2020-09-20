using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace ApiCadastro.Models
{
    public  class tbUsuario : TestUser
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }

        public string Perfil { get; set; }
        public string Login { get; set; }
        public string Senha { get; set; }

       

    }

 
   
}
