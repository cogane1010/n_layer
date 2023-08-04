// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Test;
using App.Core;
using App.BookingOnline.Data.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using App.Core.Domain;
using App.BookingOnline.Data.MailService;
using Newtonsoft.Json;
using IdentityModel.Client;
using System.Net.Http;
using System.IO;
using App.BookingOnline.Service.DTO.Common;
using App.BookingOnline.Data.Models;
using System.Runtime.Serialization.Json;
using System.Text;
using static App.Core.Enums;
using App.BookingOnline.Service.IService.Admin;

namespace IdentityServerHost.Quickstart.UI
{
    /// <summary>
    /// This sample controller implements a typical login/logout/provision workflow for local and external accounts.
    /// The login service encapsulates the interactions with the user data store. This data store is in-memory only and cannot be used for production!
    /// The interaction service provides a way for the UI to communicate with identityserver for validation and context retrieval
    /// </summary>
    //[SecurityHeaders]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly TestUserStore _users;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IEventService _events;
        private readonly IConfiguration _config;
        private readonly IMailService _mailService;
        private readonly IAppUserService _appUserService;
        private readonly IConfiguration _configuration;
        private string authUrl = string.Empty;
        public AccountController(
            SignInManager<AppUser> signInManager, UserManager<AppUser> userManager,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IEventService events,
            IConfiguration config,
            IMailService mailService,
            IAppUserService appUserService,
            IConfiguration configuration,
            TestUserStore users = null
            )
        {
            // if the TestUserStore is not in DI, then we'll just use the global users collection
            // this is where you would plug in your own custom identity management library (e.g. ASP.NET Identity)
            _users = users ?? new TestUserStore(TestUsers.Users);
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _events = events;
            _config = config;
            _mailService = mailService;
            _appUserService = appUserService;
            _configuration = configuration;
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Development")
            {
                authUrl = _configuration.GetSection("urlData").GetValue<string>("AuthUrlPro");
            }
            else
            {
                authUrl = _configuration.GetSection("urlData").GetValue<string>("AuthUrl");
            }
        }

        [HttpGet]
        [Route("api/[controller]/list")]
        public IEnumerable<AppUser> Get()
        {
            return _userManager.Users;
        }

        [HttpGet]
        [Route("api/[controller]/edit/{id}")]
        public IEnumerable<AppUser> GetById(string id)
        {
            return _userManager.Users.Where(x => x.Id == id);
        }

        /// <summary>
        /// Entry point into the login workflow
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            // build a model so we know what to show on the login page
            var vm = await BuildLoginViewModelAsync(returnUrl);

            if (vm.IsExternalLoginOnly)
            {
                // we only have one option for logging in and it's an external provider
                return RedirectToAction("Challenge", "External", new { scheme = vm.ExternalLoginScheme, returnUrl });
            }

            return View(vm);
        }

        /// <summary>
        /// Handle postback from username/password login for mobile
        /// </summary>
        [HttpPost]
        [Route("api/[controller]/LoginMobile")]
        public async Task<RespondData> LoginMobile([FromBody] LoginInputModel model)
        {
            string authUrl = _configuration.GetSection("urlData").GetValue<string>("AuthUrl");
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Development")
            {
                authUrl = _configuration.GetSection("urlData").GetValue<string>("AuthUrlPro");
            }
            // validate username/password against in-memory store
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(model.Username);
            }
            if (user == null)
            {
                var userId = await _appUserService.GetByPhoneAsync(model.Username);
                if (!userId.IsNullOrEmpty())
                {
                    user = await _userManager.FindByIdAsync(userId);
                }
            }
            if (user == null)
            {
                return new RespondData { IsSuccess = false, MsgCode = "10" };
            }

            if (user.LockoutEnd != null && user.LockoutEnd.HasValue)
            {
                if (user.LockStatus == (int)AccountStatus.moreNoShow)
                {
                    return new RespondData { IsSuccess = false, MsgCode = "70" };
                }
                if (user.LockStatus == (int)AccountStatus.locked)
                {
                    return new RespondData { IsSuccess = false, MsgCode = "71" };
                }
                return new RespondData { IsSuccess = false, MsgCode = "40" };
            }

            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                // only set explicit expiration here if user chooses "remember me". 
                // otherwise we rely upon expiration configured in cookie middleware.
                AuthenticationProperties props = null;
                if (model.RememberLogin)
                {
                    props = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.Add(AccountOptions.RememberMeLoginDuration)
                    };
                };

                // issue authentication cookie with subject ID and username
                var isuser = new IdentityServerUser(user.Id)
                {
                    DisplayName = user.UserName
                };

                await HttpContext.SignInAsync(isuser, props).ConfigureAwait(false);

                var client = new HttpClient();
                var authen = new PasswordTokenRequest
                {
                    Address = $"{authUrl}/connect/token",
                    ClientId = "clientApp",
                    GrantType = "password",
                    Scope = "openid profile api1 offline_access",
                    UserName = user.UserName,
                    Password = model.Password
                };

                var response = await client.RequestPasswordTokenAsync(authen);
                if (!response.IsError)
                {
                    using (TextReader reader = new StringReader(response.Json.ToString()))
                    using (var jsonRreader = new JsonTextReader(reader))
                    {
                        var serializer = new JsonSerializer();
                        var data = serializer.Deserialize<Token>(jsonRreader);
                        data.IssuedAt = DateTime.Now;
                        data.AccessFailedCount = user.AccessFailedCount;
                        data.ExpiresIn = _configuration.GetValue<int>("tokenExpire");
                        data.ExpiresAt = DateTime.Now.AddMinutes(data.ExpiresIn);
                        data.UserRoles = await _userManager.GetRolesAsync(user) as List<string>;
                        //data.AppVersion = _configuration.GetValue<string>("AndroidVersion");
                        data.IsForgotPass = user.IsForgotPass;
                        return new RespondData { IsSuccess = true, Data = data };
                    }
                }
                else
                {
                    return new RespondData { IsSuccess = false, MsgCode = "40" };
                }
            }
            else
            {
                var data = new Token
                {
                    AccessFailedCount = user?.AccessFailedCount ?? 0,
                };
                if (user.AccessFailedCount == 5)
                {

                }

                if (_userManager.SupportsUserLockout && await _userManager.GetLockoutEnabledAsync(user))
                {
                    try
                    {
                        await _userManager.AccessFailedAsync(user);
                    }
                    catch (Exception e)
                    {
                        return new RespondData { IsSuccess = false, Data = data, MsgCode = "67" };
                    }
                }
                return new RespondData { IsSuccess = false, Data = data, MsgCode = "35" };
            }
        }

        [HttpPost]
        [Route("api/[controller]/LogoutMobile")]
        public async Task<RespondData> LogoutMobile([FromBody] LogoutInputModel model)
        {
            //string authUrl = _configuration.GetSection("urlData").GetValue<string>("AuthUrl");
            if (!string.IsNullOrEmpty(model.LogoutId))
            {
                //string accessToken = await HttpContext.GetTokenAsync("access_token");
                //string idToken = await HttpContext.GetTokenAsync("id_token");
                //var dTokenHint = await HttpContext.GetTokenAsync(token);
                //var client = new HttpClient();
                //var url = $"{authUrl}/connect/endsession?id_token_hint=" + token;
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Bearer " + token);
                ////var response = await client.GetAsync(url);
                if (User?.Identity.IsAuthenticated == true)
                {
                    // delete local authentication cookie
                    await HttpContext.SignOutAsync();
                    // Clear the existing external cookie to ensure a clean login process
                    await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
                    await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                    // raise the logout event
                    await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
                }

                return new RespondData { IsSuccess = true };
            }
            return new RespondData { IsSuccess = false, MsgCode = "36" };
        }

        /// <summary>
        /// Handle postback from username/password login
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model, string button)
        {
            //string authUrl = _configuration.GetSection("urlData").GetValue<string>("AuthUrl");
            // check if we are in the context of an authorization request
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

            // the user clicked the "cancel" button
            if (button != "login")
            {
                if (context != null)
                {
                    // if the user cancels, send a result back into IdentityServer as if they 
                    // denied the consent (even if this client does not require consent).
                    // this will send back an access denied OIDC error response to the client.
                    await _interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);

                    // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                    if (context.IsNativeClient())
                    {
                        // The client is native, so this change in how to
                        // return the response is for better UX for the end user.
                        return this.LoadingPage("Redirect", model.ReturnUrl);
                    }
                    var returnUrl = _configuration.GetSection("urlData").GetValue<string>("ClientUrl");
                    return Redirect(returnUrl);
                }
                else
                {
                    // since we don't have a valid context, then we just go back to the home page
                    return Redirect("~/");
                }
            }

            if (ModelState.IsValid)
            {
                // validate username/password against in-memory store
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user == null)
                {
                    user = await _userManager.FindByEmailAsync(model.Username);
                }
                if (user == null)
                {
                    var userId = await _appUserService.GetByPhoneAsync(model.Username);
                    if (!userId.IsNullOrEmpty())
                    {
                        user = await _userManager.FindByIdAsync(userId);
                    }
                }

                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.Name, clientId: context?.Client.ClientId));

                    // only set explicit expiration here if user chooses "remember me". 
                    // otherwise we rely upon expiration configured in cookie middleware.
                    AuthenticationProperties props = null;
                    //if (model.RememberLogin)
                    //{
                    props = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.Add(AccountOptions.RememberMeLoginDuration)
                    };
                    //};

                    // issue authentication cookie with subject ID and username
                    var isuser = new IdentityServerUser(user.Id)
                    {
                        DisplayName = user.UserName
                    };

                    await HttpContext.SignInAsync(isuser, props);

                    if (context != null)
                    {
                        if (context.IsNativeClient())
                        {
                            // The client is native, so this change in how to
                            // return the response is for better UX for the end user.
                            return this.LoadingPage("Redirect", model.ReturnUrl);
                        }

                        // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                        return Redirect(model.ReturnUrl);
                    }

                    // request for a local page
                    if (Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else if (string.IsNullOrEmpty(model.ReturnUrl))
                    {
                        return Redirect("~/");
                    }
                    else
                    {
                        // user might have clicked on a malicious link - should be logged
                        throw new Exception("invalid return URL");
                    }
                }

                await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials", clientId: context?.Client.ClientId));
                ModelState.AddModelError(string.Empty, AccountOptions.InvalidCredentialsErrorMessage);
            }

            // something went wrong, show form with error
            var vm = await BuildLoginViewModelAsync(model);
            return View(vm);
        }


        /// <summary>
        /// Show logout page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            // build a model so the logout page knows what to display
            var vm = await BuildLogoutViewModelAsync(logoutId);

            if (vm.ShowLogoutPrompt == false)
            {
                // if the request for logout was properly authenticated from IdentityServer, then
                // we don't need to show the prompt and can just log the user out directly.
                return await Logout(vm);
            }

            return View(vm);
        }

        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel model)
        {
            // build a model so the logged out page knows what to display
            var vm = await BuildLoggedOutViewModelAsync(model.LogoutId);

            if (User?.Identity.IsAuthenticated == true)
            {
                // delete local authentication cookie
                await HttpContext.SignOutAsync();
                // Clear the existing external cookie to ensure a clean login process
                await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                // raise the logout event
                await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
            }

            // check if we need to trigger sign-out at an upstream identity provider
            if (vm.TriggerExternalSignout)
            {
                // build a return URL so the upstream provider will redirect back
                // to us after the user has logged out. this allows us to then
                // complete our single sign-out processing.
                string url = Url.Action("Logout", new { logoutId = vm.LogoutId });

                // this triggers a redirect to the external provider for sign-out
                return SignOut(new AuthenticationProperties { RedirectUri = url }, vm.ExternalAuthenticationScheme);
            }
            string clientUrl = _config.GetSection("urlData").GetValue<string>("ClientUrl");
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Development")
            {
                clientUrl = _config.GetSection("urlData").GetValue<string>("ClientUrlPro");
            }

            if (!string.IsNullOrEmpty(clientUrl))
            {
                return Redirect(clientUrl);
            }
            return View("LoggedOut", vm);
        }

        [HttpPost]
        [Route("api/[controller]")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestViewModel model)
        {
            //var aVal = 0; var blowUp = 1 / aVal;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new AppUser { UserName = model.Email, Name = model.Name, Email = model.Email };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            await _userManager.AddToRoleAsync(user, Constants.Employee);
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("userName", user.UserName));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("name", user.Name));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("email", user.Email));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("role", Constants.Employee));

            return Ok(new RegisterResponseViewModel(user));
        }


        [HttpPost]
        [Route("api/[controller]/update")]
        public async Task<IActionResult> Update([FromBody] RegisterRequestViewModel model)
        {
            //var aVal = 0; var blowUp = 1 / aVal;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _userManager.FindByIdAsync(model.Id);

            user.Email = model.Email;
            user.Name = model.Name;
            user.UserName = model.UserName;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            await _userManager.AddToRoleAsync(user, Constants.Employee);
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("userName", user.UserName));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("name", user.Name));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("email", user.Email));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("role", Constants.Employee));

            return Ok(new RegisterResponseViewModel(user));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<RespondData> ForgotPassword(ForgotPasswordViewModel forgotPasswordModel)
        {
            var errorMessage = "Cannot reset password.";
            if (!ModelState.IsValid)
                return new RespondData { IsSuccess = false, Message = errorMessage };

            var user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);
            if (user == null)
                return new RespondData { IsSuccess = false, Message = errorMessage };

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            var newPass = new String(stringChars);

            var resetPassResult = await _userManager.ResetPasswordAsync(user, token, newPass);
            var subject = "Reset password";
            var content = string.Format("New password:{0}", newPass);
            await _mailService.SendMailAsync(user.Email, "", "", subject, content, user.UserName);
            if (!resetPassResult.Succeeded)
            {
                return new RespondData { IsSuccess = false, Message = errorMessage };
            }
            return new RespondData { IsSuccess = true, Message = "Password has been reset" };
        }




        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }


        /*****************************************/
        /* helper APIs for the AccountController */
        /*****************************************/
        private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null && await _schemeProvider.GetSchemeAsync(context.IdP) != null)
            {
                var local = context.IdP == IdentityServer4.IdentityServerConstants.LocalIdentityProvider;

                // this is meant to short circuit the UI and only trigger the one external IdP
                var vm = new LoginViewModel
                {
                    EnableLocalLogin = local,
                    ReturnUrl = returnUrl,
                    Username = context?.LoginHint,
                };

                if (!local)
                {
                    vm.ExternalProviders = new[] { new ExternalProvider { AuthenticationScheme = context.IdP } };
                }

                return vm;
            }

            var schemes = await _schemeProvider.GetAllSchemesAsync();

            var providers = schemes
                .Where(x => x.DisplayName != null)
                .Select(x => new ExternalProvider
                {
                    DisplayName = x.DisplayName ?? x.Name,
                    AuthenticationScheme = x.Name
                }).ToList();

            var allowLocal = true;
            if (context?.Client.ClientId != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(context.Client.ClientId);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;

                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                    }
                }
            }

            return new LoginViewModel
            {
                AllowRememberLogin = AccountOptions.AllowRememberLogin,
                EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin,
                ReturnUrl = returnUrl,
                Username = context?.LoginHint,
                ExternalProviders = providers.ToArray()
            };
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
        {
            var vm = await BuildLoginViewModelAsync(model.ReturnUrl);
            vm.Username = model.Username;
            vm.RememberLogin = model.RememberLogin;
            return vm;
        }

        private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        {
            var vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt };

            if (User?.Identity.IsAuthenticated != true)
            {
                // if the user is not authenticated, then just show logged out page
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            var context = await _interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            return vm;
        }

        private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interaction.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
                SignOutIframeUrl = logout?.SignOutIFrameUrl,
                LogoutId = logoutId
            };

            if (User?.Identity.IsAuthenticated == true)
            {
                var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider)
                {
                    var providerSupportsSignout = await HttpContext.GetSchemeSupportsSignOutAsync(idp);
                    if (providerSupportsSignout)
                    {
                        if (vm.LogoutId == null)
                        {
                            // if there's no current logout context, we need to create one
                            // this captures necessary info from the current logged in user
                            // before we signout and redirect away to the external IdP for signout
                            vm.LogoutId = await _interaction.CreateLogoutContextAsync();
                        }

                        vm.ExternalAuthenticationScheme = idp;
                    }
                }
            }

            return vm;
        }
    }

    public class Token
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("userName")]
        public string Username { get; set; }

        [JsonProperty("issued")]
        public DateTime IssuedAt { get; set; }

        [JsonProperty("expire_at")]
        public DateTime ExpiresAt { get; set; }

        [JsonProperty("accessFailedCount")]
        public int AccessFailedCount { get; set; }

        [JsonProperty("otpId")]
        public Guid? OtpId { get; set; }

        [JsonProperty("appVersion")]
        public string AppVersion { get; set; }

        [JsonProperty("iosVersion")]
        public string IosVersion { get; set; }

        [JsonProperty("isForgotPass")]
        public bool IsForgotPass { get; set; }

        [JsonProperty("userRoles")]
        public List<string> UserRoles { get; set; }
    }
}
