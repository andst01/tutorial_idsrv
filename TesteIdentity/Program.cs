using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;

namespace TesteIdentity
{
    class Program
    {
        //static void Main(string[] args)
        //{
            
        //}

        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            var discoRO = await DiscoveryClient.GetAsync("http://localhost:65216");

            if (discoRO.IsError)
            {
                Console.WriteLine(discoRO.Error);
                return;
            }


            var tokenClient1 = new TokenClient(discoRO.TokenEndpoint, "api.teste1", "teste");
            var tokenResponse1 = await tokenClient1.RequestClientCredentialsAsync("ApiCadastro");

            if (tokenResponse1.IsError)
            {
                Console.WriteLine(tokenResponse1.Error);
                return;
            }

            Console.WriteLine(tokenResponse1.Json);

            var tokenClient2 = new TokenClient(discoRO.TokenEndpoint, "mvc.portal.evento-oidc", "secret");
            var tokenResponse2 = await tokenClient2.RequestResourceOwnerPasswordAsync("anap", "anap", "ApiCadastro");
            //tokenResponse2.
            //tokenResponse2.AccessToken
            if (tokenResponse2.IsError)
            {
                Console.WriteLine(tokenResponse2.Error);
                return;
            }

            Console.WriteLine(tokenResponse2.Json);



            var tokenClient = new TokenClient(discoRO.TokenEndpoint, "api.cadastro", "teste");
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("ApiCadastro");

           // var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("alice", "alice", "bankOfDotNetApi");




            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);

            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            //var customerInfo = new StringContent(JsonConvert.SerializeObject(
            //    new { Id = 5, FirstName = "Daniela", LastName = "Tavares" })
            //    , Encoding.UTF8
            //    , "application/json");

            //var createCustomerResponse = await client.PostAsync("http://localhost:58623/api/customers/", customerInfo);

            //if (createCustomerResponse.IsSuccessStatusCode)
            //{
            //    Console.WriteLine(createCustomerResponse.StatusCode);
            //}

            //var getCustomerResponse = await client.GetAsync("http://localhost:58623/api/customers/");

            //if (!getCustomerResponse.IsSuccessStatusCode)
            //{
            //    Console.WriteLine(getCustomerResponse.StatusCode);
            //}
            //else
            //{
            //    var content = await getCustomerResponse.Content.ReadAsStringAsync();
            //    Console.WriteLine(JArray.Parse(content));
            //}

            Console.Read();

        }

    }
}
