﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using BarcoRota.Client.Models;
using BarcoRota.Services;
using Microsoft.Extensions.Hosting;
using BarcoRota.Client.Services;

namespace BarcoRotaClient
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
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options => {
                options.SlidingExpiration = true;
            })
            .AddOpenIdConnect("BarcoCierge", options => {
                options.Authority = "https://ciergebarcorota.azurewebsites.net";
#if DEBUG
                options.RequireHttpsMetadata = false;
#else
                options.RequireHttpsMetadata = true;
#endif
                options.ClientId = "barco-rota";
                options.ResponseType = OpenIdConnectResponseType.IdTokenToken;
                options.SaveTokens = true; ; // Make false to reduce cookie size but lose JWTs
                options.GetClaimsFromUserInfoEndpoint = true;
                // !! ADDING FIELD: this will include FavColor in included claims   
                options.ClaimActions.MapUniqueJsonKey("favColor", "favColor");
                options.Scope.Add("profile");
                options.Scope.Add("openid");
                options.Scope.Add("email");
                options.Scope.Add("roles");
                //options.ClaimActions.Clear();
                options.CallbackPath = new PathString("/signin-oidc");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };
                options.Events = new OpenIdConnectEvents
                {
                    OnRedirectToIdentityProvider = (context) =>
                    {
                        context.ProtocolMessage.SetParameter("audience", "barco");

                        return Task.CompletedTask;
                    }
                };
                options.Events = new OpenIdConnectEvents
                {
                    OnRedirectToIdentityProviderForSignOut = (context) =>
                    {
                        // code from Auth0 example
                        var logoutUri = "";
                        var postLogoutUri = context.Properties.RedirectUri;
                        if (!string.IsNullOrEmpty(postLogoutUri))
                        {
                            if (postLogoutUri.StartsWith("/"))
                            {
                                // transform to absolute
                                var request = context.Request;
                                postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
                            }
                            logoutUri += $"&returnUrl={ Uri.EscapeDataString(postLogoutUri)}";
                        }

                        context.Response.Redirect(postLogoutUri);
                        context.HandleResponse();

                        return Task.CompletedTask;
                    }
                };
            });

            services.AddMvc();
            services.AddDbContext<BarcoContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddLogging();
#if DEBUG
            services.AddTransient<IEmailSender, DevMessageSender>();
#else
            services.AddTransient<IEmailSender, SmtpMessageSender>();
#endif
            services.AddSingleton<IHostedService, ShiftPusherService>();
            services.AddSingleton<IHostedService, WorkPackageFillerService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
