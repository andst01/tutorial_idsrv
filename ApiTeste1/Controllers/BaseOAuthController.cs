using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using IdentityModel.Client;
using System.Net.Http;
using System.Threading;
using System.Globalization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace ApiTeste1.Controllers
{
    [Route("[controller]")]
    public class BaseOAuthController : Controller
    {

        public  async Task ObterTokenAsync()
        {
            var client = new  HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:65216");
            if (disco.IsError) throw new Exception(disco.Error);

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "api.teste1",
                ClientSecret = "teste",
                Scope = "ApiCadastro"
            });

            if (!tokenResponse.IsError)
            {
                var expiresAt = (DateTime.UtcNow + TimeSpan.FromSeconds(tokenResponse.ExpiresIn)).ToString("o", CultureInfo.InvariantCulture);

                var authService = HttpContext.RequestServices.GetRequiredService<IAuthenticationService>();

                AuthenticateResult authenticateResult = await authService.AuthenticateAsync(HttpContext, null);

                AuthenticationProperties properties = authenticateResult.Properties;

                properties.UpdateTokenValue(OpenIdConnectParameterNames.RefreshToken, tokenResponse.RefreshToken);
                properties.UpdateTokenValue(OpenIdConnectParameterNames.AccessToken, tokenResponse.AccessToken);
                properties.UpdateTokenValue(OpenIdConnectParameterNames.ExpiresIn, expiresAt);

                await authService.SignInAsync(HttpContext, null, authenticateResult.Principal, authenticateResult.Properties);
            }
                
                
            //    return tokenResponse.AccessToken;
           // return null;
        }

        [HttpGet]
        [Route("authorize")]
        public IActionResult Authorize(
           string response_type, // authorization flow type 
           string client_id, // client id
           string redirect_uri,
           string scope, // what info I want = email,grandma,tel
           string state,
           string access_token) // random string generated to confirm that we are going to back to the same client
        {
            // ?a=foo&b=bar
            var query = new QueryBuilder();
           // query.Add(redirect_uri);
            query.Add("state", state);

            //return Redirect("http://localhost:54626/api/weatherForecast");
            return View(model: query.ToString());
        }

        [HttpPost]
        public IActionResult Authorize(
            string username,
            string redirectUri,
            string state)
        {
            const string code = "BABAABABABA";

            var query = new QueryBuilder();
            query.Add("code", code);
            query.Add("state", state);


            return Redirect($"{redirectUri}{query.ToString()}");
        }
        [HttpGet]
        [Route("token")]
        public async Task<IActionResult> Token(
         string grant_type, // flow of access_token request
         string code, // confirmation of the authentication process
         string redirect_uri,
         string client_id,
         string refresh_token,
         string access_token)
        {
            // some mechanism for validating the code

            var claims = new[]
          {
                new Claim(JwtRegisteredClaimNames.Sub, "some_id"),
                new Claim("granny", "cookie")
            };

            var secretBytes = Encoding.UTF8.GetBytes(Constants.Secret);
            var key = new SymmetricSecurityKey(secretBytes);
            var algorithm = SecurityAlgorithms.HmacSha256;

            var signingCredentials = new SigningCredentials(key, algorithm);

            var token = new JwtSecurityToken(
                Constants.Issuer,
                Constants.Audiance,
                claims,
                notBefore: DateTime.Now,
                expires: grant_type == "refresh_token"
                    ? DateTime.Now.AddMinutes(5)
                    : DateTime.Now.AddMilliseconds(1),
                signingCredentials);

           // var access_token = new JwtSecurityTokenHandler().WriteToken(token);

            var responseObject = new
            {
                access_token,
                token_type = "Bearer",
                raw_claim = "oauthTutorial",
                refresh_token = "RefreshTokenSampleValueSomething77"
            };

            var responseJson = JsonConvert.SerializeObject(responseObject);
            var responseBytes = Encoding.UTF8.GetBytes(responseJson);

            await Response.Body.WriteAsync(responseBytes, 0, responseBytes.Length);

            return Redirect(redirect_uri);
        }

        //[Authorize]
        //public IActionResult Validate()
        //{
        //    if (HttpContext.Request.Query.TryGetValue("access_token", out var accessToken))
        //    {

        //        return Ok();
        //    }
        //    return BadRequest();
        //}


    }
}
