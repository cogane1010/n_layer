// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using App.BookingOnline.Data.Identity;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServerHost.Quickstart.UI
{
    [SecurityHeaders]
    [Authorize]
    public class DiagnosticsController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        public DiagnosticsController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<IActionResult> Index()
        {
            var localAddresses = new string[] { "127.0.0.1", "::1", HttpContext.Connection.LocalIpAddress.ToString() };
            if (!localAddresses.Contains(HttpContext.Connection.RemoteIpAddress.ToString()))
            {
                return NotFound();
            }

            //var token = _userManager.GetAuthenticationTokenAsync()
            

           // var token = HttpContext.Request.Headers["Authorization"][0];
            var userGuid = Guid.Parse(HttpContext.User.FindFirst("sub").Value);

            var aa = await HttpContext.GetTokenAsync("access_token");
            var model = new DiagnosticsViewModel(await HttpContext.AuthenticateAsync());
            var name = model.AuthenticateResult.Principal.Claims.Where(x => x.Type == "name").FirstOrDefault().Value;
            var user = await _userManager.FindByNameAsync(name);
            var token1 = await _userManager.GetAuthenticationTokenAsync(user, "spaCodeClient", "access_token");
            return View(model);
        }
    }
}