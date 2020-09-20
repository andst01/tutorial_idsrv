using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel.Client;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.Net.Http.Headers;
using Thinktecture.IdentityModel.Client;
using SameSiteMode = Microsoft.Net.Http.Headers.SameSiteMode;

namespace WebTeste3.MVC
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
            services.AddControllersWithViews();

            services.AddOptions();
            //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.Configure<ApiTeste2>(options => Configuration.GetSection("ApiTeste2").Bind(options));


            services.AddHttpContextAccessor();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();


            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";

            }).AddCookie()
             .AddOpenIdConnect("oidc", options =>
             {

                 options.Authority = "http://localhost:65216/";
                 options.RequireHttpsMetadata = false;
                 options.ClientId = "mvc.portal.evento-oidc";
                 options.ClientSecret = "secret";
                 options.GetClaimsFromUserInfoEndpoint = true;
                 options.ClaimActions.MapUniqueJsonKey("email", "email", "email");
                 options.ClaimActions.MapUniqueJsonKey("role", "role", "role");
                 options.ResponseType = "code id_token";
                 options.Scope.Add("openid");
                 options.Scope.Add("profile");
                 options.Scope.Add("offline_access");
                 options.Scope.Add("ApiCadastro");
                 options.SaveTokens = true;
                 options.Events = new Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectEvents()
                 {
                     OnRedirectToIdentityProvider = b =>
                     {
                         var client = new HttpClient();
                         var discoRO =  client.GetDiscoveryDocumentAsync("http://localhost:65216").Result;
                         //TokenClient tokenClient = new TokenClient(discoRO.TokenEndpoint, "api.teste1", "teste");
                         var tokenResponse = client.RequestPasswordTokenAsync(new PasswordTokenRequest()
                         {
                             Address = "http://localhost:65216/connect/token",
                             Password = "anap",
                             UserName = "anap",
                             Scope = "openid profile offline_access ApiCadastro",
                             GrantType = GrantType.ResourceOwnerPassword,
                             ClientSecret = "secret",
                             ClientId = "mvc.portal.evento-oidc",
                             
                         }).Result;

                         var handler = new JwtSecurityTokenHandler();
                         var jsonToken = handler.ReadToken(tokenResponse.AccessToken);
                         var tokenS = handler.ReadToken(tokenResponse.AccessToken) as JwtSecurityToken;


                         var claimRole = tokenS.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);
                         var claimName = tokenS.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);
                         var claimNameIdentity = tokenS.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                         var claimEmail = tokenS.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);

                         var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                         identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, claimNameIdentity.Value));
                         identity.AddClaim(new Claim(ClaimTypes.Role, claimRole.Value));
                         identity.AddClaim(new Claim(ClaimTypes.Name, claimName.Value));

                         var principal = new ClaimsPrincipal(identity);
                         var properties = new AuthenticationProperties();
                         var tokens = new List<AuthenticationToken>();
                         tokens.Add(new AuthenticationToken() { Name="access_token", Value = tokenResponse.AccessToken });
                         tokens.Add(new AuthenticationToken() { Name = "refresh_token", Value = tokenResponse.RefreshToken });

                         properties.ExpiresUtc = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResponse.ExpiresIn);
                         properties.StoreTokens(tokens);
                         properties.IsPersistent = true;

                         b.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);



                         var user = b.HttpContext.User;
                        

                         //b.Properties.StoreTokens(tokens);

                         return Task.FromResult(0);
                     },
                     

                 };

                 
             });


            services.AddResponseCaching();

            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "image/svg+xml" });
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });

            services.AddDistributedMemoryCache();

            
           

            services.Configure<CookiePolicyOptions>(opt =>
            {
                opt.CheckConsentNeeded = context => true;
                opt.MinimumSameSitePolicy = (Microsoft.AspNetCore.Http.SameSiteMode)SameSiteMode.None;
            });

            services.AddCors();
            services.AddDirectoryBrowser();


            services.AddSession(opt =>
            {
                opt.Cookie.IsEssential = false;
                opt.Cookie.Name = "Cookies";
            });

            IdentityModelEventSource.ShowPII = true;

            services.Configure<IISServerOptions>(options =>
            {
                options.AutomaticAuthentication = false;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseAuthentication();
            app.UseSession();
            app.UseResponseCaching();
            app.UseCookiePolicy();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();


            //app.UseOpenIdConnectAuthentication()

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
