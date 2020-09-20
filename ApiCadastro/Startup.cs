using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiCadastro.Infra;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.Owin.Security.Infrastructure;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ApiCadastro
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddIdentityServer()
               .AddDeveloperSigningCredential()
               .AddInMemoryApiScopes(Config.GetApiScopes())
               .AddInMemoryIdentityResources(Config.GetIdentityResource())
               .AddInMemoryApiResources(Config.GetAllApiResources())
               .AddInMemoryClients(Config.GetClients())
               .AddTestUsers(Config.GetUsers())
               .AddProfileService<ProfileService>();

            services.AddControllers();
            
            services.AddCors();
           
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
            .AddIdentityServerAuthentication(options =>
            {
                    // required audience of access tokens
                options.ApiName = "ApiCadastro";
               
                options.RequireHttpsMetadata = false;
                    // auth server base endpoint (this will be used to search for disco doc)
                options.Authority = "http://localhost:65216";

            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {

                    Title = "Api Cadastro",
                    Version = "v1"
                });

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows()
                    {

                        ClientCredentials = new OpenApiOAuthFlow()
                        {
                            AuthorizationUrl = new Uri("http://localhost:65216/connect/authorize"),
                            TokenUrl = new Uri("http://localhost:65216/connect/token"),

                            Scopes = new Dictionary<string, string>
                            {
                                { "ApiCadastro", "ApiCadastro" },
                                { "ApiTeste2", "ApiTeste2" },


                            }

                        }

                    }


                });


                options.OperationFilter<CheckAuthorizeOperationFilter>();
            });



            services.Configure<IISServerOptions>(options =>
            {
                options.AutomaticAuthentication = false;
            });

            services.AddTransient<IProfileService, ProfileService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();
            // app.UseStaticFiles();
            //app.UseMvcWithDefaultRoute();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("./swagger/v1/swagger.json", "Api Cadastro");
                options.OAuthClientId("api.cadastro");
                options.OAuthClientSecret("testeCadastro");
                options.OAuthAppName("Swagger Api Cadastro");

                options.RoutePrefix = string.Empty;
             

            });


            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(opt => opt.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    internal class CheckAuthorizeOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {



            var hasAuthorize = context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

            if (hasAuthorize)
            {
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        [
                            new OpenApiSecurityScheme {Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "oauth2"}
                            }
                        ] = new[] { "ApiCadastro" }
                    }
                };
            };


        }


    }
}
