using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebTeste3.MVC
{
    public class AppSettings
    {
        public ApiTeste2 ApiTeste2 { get; set; }
    }

    public class ApiTeste2
    {
        public string BaseUrl { get; set; }
        public string ClientId { get; set; }
        public string Secret { get; set; }
        public string Scope { get; set; }
    }
}
