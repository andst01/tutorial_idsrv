using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using IdentityModel.Client;

namespace ApiTeste2
{
    public static class HttpExtensions
    {
        public static HttpClient ClientApi(this HttpClient client, string url, string token)
        {
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.SetBearerToken(token);

            return client;
        }
    }
}
