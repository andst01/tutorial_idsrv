using System;
using System.Collections.Generic;
using System.Linq;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace ApiTeste1
{
    public class Startup
    {
        private readonly IWebHostEnvironment env;

        public Startup(IConfiguration configuration, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env)
        {
            this.env = env;
            var builder = new ConfigurationBuilder()
                .SetBasePath(this.env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{this.env.EnvironmentName}.json", optional: true);
            Configuration = configuration;

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {


            services.AddControllers();
            
            services.Configure<ApiCadastro>(options => Configuration.GetSection("ApiCadastro").Bind(options));
            services.Configure<ApiTeste2>(options => Configuration.GetSection("ApiTeste2").Bind(options));

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
           
            services.AddAuthentication(o =>
            {
                o.DefaultScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                o.DefaultAuthenticateScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;


            })
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "http://localhost:65216/";
                    options.RequireHttpsMetadata = false;
                    options.ApiSecret = "teste";
                    options.ApiName = "ApiCadastro";
                    


                });


            IdentityModelEventSource.ShowPII = true;
            //services.Add
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {

                    Title = "Api Teste 1",
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

                            }

                        }

                    }

                }); ;


                options.OperationFilter<CheckAuthorizeOperationFilter>();
            });

            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("policy", builder =>
                {
                    builder.RequireScope("ApiCadastro", "profile", "offline_access", "openid");
                });
            });

            services.AddCors();


            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;

            });
           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("./swagger/v1/swagger.json", "Api Teste 1");

                options.OAuthClientId("api.teste1");
                options.OAuthClientSecret("teste");
                options.OAuthAppName("Swagger API Teste 1 UI");

                options.RoutePrefix = string.Empty;
                // options.DocExpansion(DocExpansion.None);

            });
           

            app.UseCookiePolicy();
            app.UseRouting();
            app.UseCors(opt => opt.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            app.UseAuthentication();
            app.UseAuthorization();


            //tokenExt.InitializeAsync();

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
