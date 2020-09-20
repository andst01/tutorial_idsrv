using ApiCadastro.Models;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace ApiCadastro.Infra
{

    public class ProfileService : IProfileService
    {

        private List<tbUsuario> RetornaListaUsuario()
        {
            List<tbUsuario> Users = new List<tbUsuario>();
            List<TestUser> testUsers = new List<TestUser>();

            Users.Add(new tbUsuario()
            {
                Id = 1,
                SubjectId = "1",
                Nome = "Amanda",
                Senha = "amanda",
                Login = "amanda",
                Email = "amanda@gmail.com",
                Perfil = "admin",
                Username = "amanda",
                Password = "amanda"
                //Claims =
                //{
                //    new Claim("Email",  "amanda@gmail.com"),
                //    new Claim(JwtClaimTypes.Role, "admin")
                //}

            });

            Users.Add(new tbUsuario()
            {
                Id = 2,
                SubjectId = "2",
                Nome = "Ana Paula",
                Senha = "anap",
                Login = "anap",
                Email = "anapaula@gmail.com",
                Perfil = "usuario",
                Username = "anap",
                Password = "anap"
                //Claims =
                //{
                //    new Claim("Email",  "anapaula@gmail.com"),
                //    new Claim(JwtClaimTypes.Role, "usuario")
                //}

            });
            testUsers.AddRange(Users);

            return Users;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.FindFirst("sub")?.Value;

            if (sub != null)
            {
                var usuario = RetornaListaUsuario().FirstOrDefault(x => x.SubjectId == sub);

                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Email, usuario.Email));
                claims.Add(new Claim(ClaimTypes.Role, usuario.Perfil));
                claims.Add(new Claim(ClaimTypes.NameIdentifier, usuario.Login));
                claims.Add(new Claim(ClaimTypes.Name, usuario.Nome));

                context.IssuedClaims.AddRange(claims);

            }

            await Task.FromResult(0);
            //throw new NotImplementedException();
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.FromResult(0);
        }
    }
}
