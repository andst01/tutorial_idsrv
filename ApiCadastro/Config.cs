using ApiCadastro.Models;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApiCadastro
{
    public class Config
    {

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope(name: "ApiTeste1",   displayName: "ApiTeste1"),
                new ApiScope(name: "ApiTeste2",   displayName: "ApiTeste2"),
                new ApiScope(name: "ApiCadastro",
                displayName: "ApiCadastro",
                userClaims: new [] { ClaimTypes.NameIdentifier, 
                                     ClaimTypes.Name, 
                                     ClaimTypes.Email, 
                                     ClaimTypes.Role }),
                new ApiScope(name: "profile",   displayName: "profile", new string[]{

                                                              "name",
                                                              "family_name",
                                                              "given_name",
                                                              "middle_name",
                                                              "nickname",
                                                              "preferred_username",
                                                              "profile",
                                                              "picture",
                                                              "website",
                                                              "gender",
                                                              "birthdate",
                                                              "zoneinfo",
                                                              "locale",
                                                              "updated_at"
                 })

            };
        }
        public static IEnumerable<IdentityResource> GetIdentityResource()
        {
            return new List<IdentityResource>()
            {
                new IdentityResources.OpenId(),


            };
        }

        public static IEnumerable<ApiResource> GetAllApiResources()
        {

            return new List<ApiResource>()
            {

                new ApiResource()
                {
                    Name = "openid",
                    DisplayName = "openid",
                    Description = "openid",
                    UserClaims = {"sub"},


                 },
                new ApiResource("profile", "profile", new[]{ "name",
                                                              "family_name",
                                                              "given_name",
                                                              "middle_name",
                                                              "nickname",
                                                              "preferred_username",
                                                              "profile",
                                                              "picture",
                                                              "website",
                                                              "gender",
                                                              "birthdate",
                                                              "zoneinfo",
                                                              "locale",
                                                              "updated_at"}),


                new ApiResource("ApiCadastro", "ApiCadastro") { Scopes = new string[]{ "ApiCadastro" } },
                new ApiResource("ApiTeste2", "ApiTeste2") { Scopes = new string[]{ "ApiTeste2" } },
                new ApiResource("ApiTeste1", "ApiTeste1") { Scopes = new string[]{ "ApiTeste1" } }
            };

        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>()
            {
                new Client()
                {
                    ClientId = "api.teste1",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets =
                    {
                        new Secret("teste".Sha256())
                    },
                     AllowOfflineAccess = true,
                     RequireClientSecret = false,
                     RequireConsent = false,
                     AllowedCorsOrigins = { "http://localhost:54626", "http://localhost:65216" },
                     AllowedScopes = new List<string>() { "ApiTeste1", "ApiTeste2", "ApiCadastro", "profile", "offline_access", "openid" },
                      
                     
                    //RefreshTokenUsage = TokenUsage.ReUse,
                    //RefreshTokenExpiration = TokenExpiration.Absolute,
                    //AbsoluteRefreshTokenLifetime = 259200,
                    //RequireConsent = false,
                    

                },
                new Client()
                {
                    ClientId = "api.teste2",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets =
                    {
                        new Secret("apiteste2".Sha256())
                    },
                     AllowOfflineAccess = true,
                     RequireClientSecret = false,
                     RequireConsent = false,
                     AllowedCorsOrigins = { "http://localhost:51920", "http://localhost:65216" },
                     AllowedScopes = new List<string>() { "ApiTeste1", "ApiTeste2", "ApiCadastro", "profile", "offline_access", "openid" },
                     Claims = GetClaims().ToList()
                    
                     //RefreshTokenUsage = TokenUsage.ReUse,
                     //RefreshTokenExpiration = TokenExpiration.Absolute,
                     //AbsoluteRefreshTokenLifetime = 259200,
                     //RequireConsent = false,
                    

                },
                new Client()
                {
                    ClientId = "api.cadastro",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets =
                    {
                        new Secret("testeCadastro".Sha256())
                    },
                    RequireClientSecret = false,
                    RequireConsent = false,
                    AllowedScopes = new List<string>() { "ApiTeste1", "ApiTeste2", "ApiCadastro", "profile", "offline_access", "openid" },
                    Claims = GetClaims().ToList()
                   // AllowOfflineAccess = true,
                   // RedirectUris = {"http://localhost:65216/oauth2-redirect.html"},
                   // AllowedCorsOrigins = {"http://localhost:65216"},
                   //RefreshTokenUsage = TokenUsage.ReUse,
                   //RefreshTokenExpiration = TokenExpiration.Absolute,
                   //AbsoluteRefreshTokenLifetime = 259200,
                   

                },
                 new Client()
                {
                    ClientId = "mvc.portal.evento-oidc",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowOfflineAccess = true,
                    ClientSecrets = {  new Secret("secret".Sha256()) },
                    AllowedScopes = { "openid", "profile", "offline_access", "ApiCadastro"},
                    RedirectUris = new[]{ "http://localhost:49250/signin-oidc"},
                    RefreshTokenUsage = TokenUsage.ReUse,
                    RefreshTokenExpiration = TokenExpiration.Absolute,
                    AbsoluteRefreshTokenLifetime = 259200,
                    RequireConsent = false,
                    AllowAccessTokensViaBrowser = true,
                    AccessTokenLifetime = 3600,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    PostLogoutRedirectUris = { "http://localhost:49250" },
                    AllowedCorsOrigins = { "http://localhost:49250", "http://localhost:65216" },



                },

            };

        }


        public static IEnumerable<ClientClaim> GetClaims()
        {
            var claims = new List<ClientClaim>(){

                    new ClientClaim("Nome", "Alice Smith"),
                    new ClientClaim("Email", "alice.beltrao@gmail.com"),

             };

            return claims;
        }

        public static List<TestUser> GetUsers()
        {
            List<tbUsuario> Users = new List<tbUsuario>();
            List<TestUser> testUsers = new List<TestUser>();

            Users.Add(new tbUsuario()
            {
                SubjectId = "1",
                Id = 1,
                Nome = "Amanda",
                Senha = "amanda",
                Login = "amanda",
                Email = "amanda@gmail.com",
                Perfil = "admin",
                Username = "amanda",
                Password = "amanda"

            });

            Users.Add(new tbUsuario()
            {
                SubjectId = "2",
                Id = 2,
                Nome = "Ana Paula",
                Senha = "anap",
                Login = "anap",
                Email = "anapaula@gmail.com",
                Perfil = "usuario",
                Username = "anap",
                Password = "anap"


            });

            testUsers.AddRange(Users);
            return testUsers;
        }


    }
}
