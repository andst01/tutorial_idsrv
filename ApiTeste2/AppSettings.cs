using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiTeste2
{
    public class AppSettings
    {
        public ApiCadastro ApiCadastro { get; set; }
        public ApiTeste1 ApiTeste1 { get; set; }
    }

    public class ApiCadastro
    {
        public string BaseUrl { get; set; }
        public string ClientId { get; set; }
        public string Secret { get; set; }
        public string Scope { get; set; }
    }
    public class ApiTeste1
    {
        public string BaseUrl { get; set; }
        public string ClientId { get; set; }
        public string Secret { get; set; }
        public string Scope { get; set; }
    }
}
