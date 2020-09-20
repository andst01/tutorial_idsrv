using AspNetCore.AsyncInitialization;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApiTeste1
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


        //public static async Task HandleToken(this HttpClient client)
        //{

        //    var accessToken = await client.GetRefreshTokenAsync();
        //   // var token = ctx.HttpContext.GetTokenAsync("access_token");
        //    if (accessToken != null)
        //        client.SetBearerToken(accessToken);
        //}

        //private static async Task<string> GetRefreshTokenAsync(this HttpClient client)
        //{
            
        //    var appSetings = new ApiCadastro();
        //    var disco = await client.GetDiscoveryDocumentAsync(appSetings.BaseUrl);
        //    if (disco.IsError) throw new Exception(disco.Error);

        //    var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        //    {
        //        Address = disco.TokenEndpoint,
        //        ClientId = appSetings.ClientId,
        //        ClientSecret = appSetings.Secret,
        //        Scope = appSetings.Scope
        //    });

        //    if (!tokenResponse.IsError) return tokenResponse.AccessToken;
        //    return null;
        //}
       
    }
}

