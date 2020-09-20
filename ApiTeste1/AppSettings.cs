using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiTeste1
{
    public class AppSettings
    {
        public ApiCadastro ApiCadastro { get; set; }
        public ApiTeste2 ApiTeste2 { get; set; }
    }

    public class ApiCadastro
    {
        public string BaseUrl { get; set; } 
        public string ClientId { get; set; } 
        public string Secret { get; set; } 
        public string Scope { get; set; }
    }

    public class ApiTeste2
    {
        public string BaseUrl { get; set; }
        public string ClientId { get; set; }
        public string Secret { get; set; }
        public string Scope { get; set; }
    }
}
