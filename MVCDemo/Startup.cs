//#define USE_IDSVR6 - put this definition in the project file
using Demo.Infrastructure;
using Demo.Shared.Interfaces;
#if USE_IDSVR6
using Microsoft.AspNetCore.Authentication;
#else
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
#endif
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
#if USE_IDSVR6
using Microsoft.IdentityModel.Tokens;
using MVCDemo.AuthHelpers;
using MVCDemo.Models;
using MVCDemo.Models.Configuration;
using System;
using System.IdentityModel.Tokens.Jwt;
#else
using Microsoft.Identity.Web;
using MVCDemo.AuthHelpers;
using MVCDemo.Middleware;
using MVCDemo.Models;
#endif
using MDS = MVCDemo.Services;
using SIO = System.IO;

namespace MVCDemo
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
            NLog.Logger logger = null;

            //NOTE: Only use nlog.nodb.config if for logging starting here with no database
            if (SIO.File.Exists("nlog.nodb.config"))
            {
                logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.nodb.config").GetCurrentClassLogger();
                logger.Debug("ConfigureServices");
            }
            else
            {
                logger = NLog.LogManager.GetCurrentClassLogger();
            }

#if DEBUG
            //*** Uncomment ONLY for local testing ***
            //Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
#endif
            //Inject settings configuration file using Options pardigm
            services.Configure<Models.Configuration.AppSettings>(options => Configuration.GetSection(nameof(Models.Configuration.AppSettings)).Bind(options));

            string authCookieName = Configuration.GetValue<string>("AppSettings:authCookieName");
            string authCookiePath = Configuration.GetValue<string>("AppSettings:authCookiePath");

#if USE_IDSVR6
            if (string.IsNullOrWhiteSpace(authCookieName))
            {
                authCookieName = MVCDemoDefs.defaultAppIdCookieName;
#if DEBUG
                logger.Debug($"authCookieName(1) = [{authCookieName}]");
#endif
            }
#else
            if (string.IsNullOrWhiteSpace(authCookieName)) authCookieName = MVCDemoDefs.Defaults.authCookieName;
#endif

            int? cookieMinutes = Configuration.GetValue<int?>("AppSettings:cookieMin");
            if (!cookieMinutes.HasValue) cookieMinutes = 10;

#if USE_IDSVR6
            services.Configure<IdentityServerOptions>(options => Configuration.GetSection("IdentityServer").Bind(options));
            IdentityServerOptions idSverSettings = Configuration.GetSection("IdentityServer").Get<IdentityServerOptions>();
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "Cookies";
                    options.DefaultChallengeScheme = "oidc";
                })
                    .AddCookie("Cookies", options =>
                    {
                        options.Cookie.Name = authCookieName;
                        if (!string.IsNullOrWhiteSpace(authCookiePath))
                        {
                            options.Cookie.Path = authCookiePath; //10/10/2022 - causes problems with Blazor applets - if used for real, needs to come from configuration
                        }
                        options.ExpireTimeSpan = TimeSpan.FromMinutes(cookieMinutes.Value);
                    })
                    .AddJwtBearer(options =>
                    {
                        options.Authority = idSverSettings.Authority;
                        options.TokenValidationParameters.ValidateAudience = false;
                        options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
                        {
                            OnAuthenticationFailed = JwtBearerEventHandlers.OnAuthenticationFailedHandler
                        };
                    })
                    .AddOpenIdConnect("oidc", options =>
                    {
                        options.Authority = idSverSettings.Authority;

                        options.ClientId = idSverSettings.ClientId;
                        options.ClientSecret = idSverSettings.ClientSecret;
                        options.ResponseType = idSverSettings.ResponseType;

                        options.SaveTokens = true;

                        options.Scope.Clear();
                        options.Scope.Add("openid");
                        options.Scope.Add("profile");
                        options.Scope.Add("role");
                        options.Scope.Add("user");
                        //options.Scope.Add("api");
                        options.Scope.Add("offline_access");

                        // role is an array and is special
                        options.ClaimActions.MapAllExcept("role", "latestauthtime");

                        options.ClaimActions.MapUniqueJsonKey("latestauthtime", "latestauthtime", JsonClaimValueTypes.Json);
                        options.ClaimActions.MapJsonKey("role", "role");

                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            NameClaimType = "name",
                            RoleClaimType = "role"
                        };

                        options.GetClaimsFromUserInfoEndpoint = true;
                    });
            }
#else
            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                //.AddMicrosoftIdentityWebApp(Configuration);
                .AddMicrosoftIdentityWebApp(x =>
                {
                    Configuration.GetSection("AzureAd").Bind(x);
                    x.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    x.Events.OnTokenValidated += OpenIdConnectEventHandlers.OnTokenValidatedFunc;
                }, options => {
                    options.Cookie.Name = authCookieName;
                    if (!string.IsNullOrWhiteSpace(authCookiePath)) options.Cookie.Path = authCookiePath;
                });
#endif

            bool blazorDetailedErrors = Configuration.GetValue<bool>($"{nameof(Models.Configuration.AppSettings)}:{nameof(Models.Configuration.AppSettings.blazorDetailedErrors)}");

#if USE_IDSVR6
            services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            })
            .AddNewtonsoftJson();
#else
            services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });
#endif
            services.AddServerSideBlazor().AddCircuitOptions(options => options.DetailedErrors = blazorDetailedErrors);
            services.AddRazorPages()
                .AddRazorRuntimeCompilation();
            services.AddDemoInfrastructure();

            services.AddScoped<CircuitHandler>((sp) =>
                new MDS.CircuitHandlerService(sp.GetRequiredService<IBlazorUserService>()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseExceptionHandler("/Home/Error"); //Use this when testing for what user will see
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
#if !USE_IDSVR6
            app.UseSignedOutMiddleware();
#endif

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoints.MapControllerRoute(
                //    name: "default",
                //    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapBlazorHub();
            });
        }
    }
}
