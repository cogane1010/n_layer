// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using App.BookingOnline.Data;
using App.BookingOnline.Data.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Linq;
using System.Net;
using App.BookingOnline.Data.Repositories;
using App.Core.Repositories;
using App.BookingOnline.Data.MailService;
using IdentityServer4.Validation;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Events;
using static IdentityModel.OidcConstants;
using App.BookingOnline.Service.IService.Admin;
using App.BookingOnline.Service.Service.Admin;
using App.BookingOnline.Data.Repositories.Common;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
       
        //public IConfig _config { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            StaticConfig = configuration;
        }


        public static IConfiguration StaticConfig { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            var tokenCleanupInterval = Configuration.GetSection("loginTime").GetValue<int>("TokenCleanupInterval");
            var cookieLifetime = Configuration.GetSection("loginTime").GetValue<int>("CookieLifetime");

            services.AddDbContext<BookingOnlineDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("BookingOnlineDBContext"),
            x => x.MigrationsAssembly("App.BookingOnline.Data")));

            services.AddDbContext<PersistedGrantDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("BookingOnlineDBContext"),
            x => x.MigrationsAssembly("App.BookingOnline.Data")));

            services.AddDbContext<ConfigurationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("BookingOnlineDBContext"),
            x => x.MigrationsAssembly("App.BookingOnline.Data")));

            var lockoutTimeSpan = Configuration.GetSection("loginTime").GetValue<int>("DefaultLockoutTimeSpan");
            var failedAccessAttempts = Configuration.GetSection("loginTime").GetValue<int>("MaxFailedAccessAttempts");
            services.AddIdentity<AppUser, AspRole>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(lockoutTimeSpan);
                options.Lockout.MaxFailedAccessAttempts = failedAccessAttempts;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
            })
                .AddEntityFrameworkStores<BookingOnlineDbContext>()
                .AddDefaultTokenProviders();

            services.AddIdentityServer(options =>
            {
                options.Authentication = new IdentityServer4.Configuration.AuthenticationOptions()
                {
                    CookieLifetime = TimeSpan.FromHours(cookieLifetime), // ID server cookie timeout set to 10 hours
                    CookieSlidingExpiration = true
                };
            }).AddDeveloperSigningCredential()
                // this adds the operational data from DB (codes, tokens, consents)
                //.AddPersistedGrantStore<IPersistedGrantStore>()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(Configuration.GetConnectionString("BookingOnlineDBContext"));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseSqlServer(Configuration.GetConnectionString("BookingOnlineDBContext"));
                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = tokenCleanupInterval; // interval in seconds                    
                })
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryClients(Config.GetClients())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryPersistedGrants()
                .AddResourceOwnerValidator<ResourceOwnerPasswordValidator<AppUser>>();

            //.AddInMemoryPersistedGrants()
            //.AddInMemoryIdentityResources(Config.GetIdentityResources())
            //.AddInMemoryApiResources(Config.GetApiResources())
            //.AddInMemoryClients(Config.GetClients());


            /* We'll play with this down the road... 
                services.AddAuthentication()
                .AddGoogle("Google", options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    options.ClientId = "<insert here>";
                    options.ClientSecret = "<insert here>";
                });*/
            
            services.AddTransient<IProfileService, IdentityClaimsProfileService>();
            services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();
            services.AddTransient<IResourceStore, ResourceStore>();

            services.AddScoped<DbContext, BookingOnlineDbContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IAppUserRepository, AppUserRepository>();
            services.AddScoped<IAppUserService, AppUserService>();
            services.AddScoped<ISettingRepository, SettingRepository>();
            services.AddScoped<IMailService, MailService>();


            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader()));

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            }).SetCompatibilityVersion(CompatibilityVersion.Latest);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {

            bool seed = Configuration.GetSection("Data").GetValue<bool>("Seed");            
            if (seed)
            {
                InitializeDatabase(app);
                throw new Exception("Seeding completed. Disable the seed flag in appsettings");
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseDeveloperExceptionPage();
               // app.UseExceptionHandler("/Error");
            }

            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Lax
            });

            app.UseExceptionHandler(builder =>
            {
                builder.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

                    var error = context.Features.Get<IExceptionHandlerFeature>();
                    if (error != null)
                    {
                        //context.Response.a(error.Error.Message);
                        await context.Response.WriteAsync(error.Error.Message).ConfigureAwait(false);
                    }
                });
            });
            //app.UseDirectoryBrowser(new DirectoryBrowserOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(
            //       Path.Combine(Directory.GetCurrentDirectory(), @"logs")),
            //    RequestPath = new PathString("/logs33333")
            //});
            var serilog = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .WriteTo.File(@"authserver_log.txt");

            loggerFactory.WithFilter(new FilterLoggerSettings
                {
                    { "IdentityServer4", LogLevel.Debug },
                    { "Microsoft", LogLevel.Warning },
                    { "System", LogLevel.Warning },
                }).AddSerilog(serilog.CreateLogger());

            app.UseStaticFiles();
            app.UseCors("AllowAll");
            app.UseIdentityServer();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                //var _configService = serviceScope.ServiceProvider.GetRequiredService<IConfig>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in Config.GetClients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Config.GetApiResources())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiScopes.Any())
                {
                    foreach (var resource in Config.GetApiScopes())
                    {
                        context.ApiScopes.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }
    }

    public class ResourceOwnerPasswordValidator<TUser> : IResourceOwnerPasswordValidator
        where TUser : class
    {
        private readonly SignInManager<TUser> _signInManager;
        private IEventService _events;
        private readonly UserManager<TUser> _userManager;
        private readonly ILogger<ResourceOwnerPasswordValidator<TUser>> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceOwnerPasswordValidator{TUser}"/> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="signInManager">The sign in manager.</param>
        /// <param name="events">The events.</param>
        /// <param name="logger">The logger.</param>
        public ResourceOwnerPasswordValidator(
            UserManager<TUser> userManager,
            SignInManager<TUser> signInManager,
            IEventService events,
            ILogger<ResourceOwnerPasswordValidator<TUser>> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _events = events;
            _logger = logger;
        }

        /// <summary>
        /// Validates the resource owner password credential
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public virtual async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var user = await _userManager.FindByNameAsync(context.UserName);
            if (user != null)
            {
                var result = await _signInManager.CheckPasswordSignInAsync(user, context.Password, true);
                if (result.Succeeded)
                {
                    var sub = await _userManager.GetUserIdAsync(user);

                    _logger.LogInformation("Credentials validated for username: {username}", context.UserName);
                    await _events.RaiseAsync(new UserLoginSuccessEvent(context.UserName, sub, context.UserName, interactive: false));

                    context.Result = new GrantValidationResult(sub, AuthenticationMethods.Password);
                    return;
                }
                else if (result.IsLockedOut)
                {
                    _logger.LogInformation("Authentication failed for username: {username}, reason: locked out", context.UserName);
                    await _events.RaiseAsync(new UserLoginFailureEvent(context.UserName, "locked out", interactive: false));
                }
                else if (result.IsNotAllowed)
                {
                    _logger.LogInformation("Authentication failed for username: {username}, reason: not allowed", context.UserName);
                    await _events.RaiseAsync(new UserLoginFailureEvent(context.UserName, "not allowed", interactive: false));
                }
                else
                {
                    _logger.LogInformation("Authentication failed for username: {username}, reason: invalid credentials", context.UserName);
                    await _events.RaiseAsync(new UserLoginFailureEvent(context.UserName, "invalid credentials", interactive: false));
                }
            }
            else
            {
                _logger.LogInformation("No user found matching username: {username}", context.UserName);
                await _events.RaiseAsync(new UserLoginFailureEvent(context.UserName, "invalid username", interactive: false));
            }

            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
        }
    }
}
