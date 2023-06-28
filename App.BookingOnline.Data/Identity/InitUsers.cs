// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using App.BookingOnline.Data;
using App.BookingOnline.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Data.Seed
{
    public class InitUsers
    {
        public static void EnsureSeedData(string connectionString)
        {
            var services = new ServiceCollection();
            services.AddDbContext<BookingOnlineDbContext>(options =>
               options.UseSqlServer(connectionString));

            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<BookingOnlineDbContext>()
                .AddDefaultTokenProviders();
            services.AddLogging();

            using (var serviceProvider = services.BuildServiceProvider())
            {
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var context = scope.ServiceProvider.GetService<BookingOnlineDbContext>();
                    context.Database.Migrate();

                    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                    var admin = userMgr.FindByNameAsync("admin").Result;
                    if (admin == null)
                    {
                        admin = new AppUser
                        {
                            UserName = "admin",
                            Email = "admin@email.com",
                            EmailConfirmed = false,
                            EnrolledDate = DateTime.Now,
                            Dob = DateTime.Now,
                            Name = "admin"
                        };
                        var result = userMgr.CreateAsync(admin, "Brg@123456").Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        result = userMgr.AddClaimsAsync(admin, new Claim[]{
                        new Claim(JwtClaimTypes.Name, "Admin"),
                        new Claim(JwtClaimTypes.GivenName, "Admin"),
                        new Claim(JwtClaimTypes.FamilyName, "Admin"),
                        new Claim(JwtClaimTypes.Email, "admin@email.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "https://localhost:4200"),
                        new Claim(JwtClaimTypes.Address, @"{ 'city': 'Hà Nội' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json)
                    }).Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        Console.WriteLine("admin created");
                    }
                    else
                    {
                        Console.WriteLine("alice already exists");
                    }

                   
                }
            }
        }
    }
}

