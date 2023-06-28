using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace IdentityServer
{
    public interface IConfig
    {
        IEnumerable<ApiScope> GetApiScopes();
        IEnumerable<ApiResource> GetApiResources();
        IEnumerable<IdentityResource> GetIdentityResources();
        IEnumerable<Client> GetClients();

    }
    public static class Config
    {
        //public Config(IConfiguration configuration)
        //{
        //    Configuration = configuration;
        //}

        public static IConfiguration Configuration { get; set; }

        //private string spaClientUrl1 = Configuration.GetSection("urlData").GetValue<string>("ClientUrl");
        //private static readonly string swaggerUrl = "https://localhost:44337";
        //private static readonly string spaClientUrl = "https://localhost:4200";

        //private static readonly string swaggerUrl = "https://demobookingapi.brggroup.vn";
        //private static readonly string spaClientUrl = "https://demobooking.brggroup.vn";
        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
             {
                 new ApiScope(name: "read",   displayName: "Read your data."),
                 new ApiScope(name: "write",  displayName: "Write your data."),
                 new ApiScope(name: "delete", displayName: "Delete your data."),
                 new ApiScope(name: "identityserverapi", displayName: "manage identityserver api endpoints."),
                 new ApiScope(name: "postman_api", displayName: "manage identityserver api endpoints."),
                 new ApiScope(name: "api1", displayName: "manage identityserver api endpoints.")
             };
        }
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("postman_api","api1")
            };
        }

        // scopes define the resources in your system
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),

            };
        }

        public static IEnumerable<Client> GetClients()
        {
            string swaggerUrl = Startup.StaticConfig.GetSection("urlData").GetValue<string>("SwaggerUrl");// "https://localhost:44337";
            string spaClientUrl = Startup.StaticConfig.GetSection("urlData").GetValue<string>("ClientUrl"); // "https://localhost:4200";
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Development")
            {
                swaggerUrl = Startup.StaticConfig.GetSection("urlData").GetValue<string>("SwaggerUrlPro"); // "https://demobookingapi.brggroup.vn";
                spaClientUrl = Startup.StaticConfig.GetSection("urlData").GetValue<string>("ClientUrlPro"); // "https://demobooking.brggroup.vn";
            }
            return new List<Client>
            {
                new Client
{
                    ClientId = "demo_api_swagger",
                    ClientName = "Swagger UI for demo_api",
                    ClientSecrets = {new Secret("secret".Sha256())}, // change me!
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,
                    RedirectUris = {$"{swaggerUrl}/oauth2-redirect.html" },
                    AllowedCorsOrigins = { $"{swaggerUrl}"},
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1"
                    }
                },
                new Client
                {
                    ClientId = "clientApp",
                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    RequireClientSecret = false,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowOfflineAccess = true,
                    // scopes that client has access to
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1"
                    },
                    RequirePkce = false,
                    //RedirectUris = { "https://localhost:44337/signin-oidc" },
                },

                // OpenID Connect implicit flow client (MVC)
                new Client
                {
                    ClientId = "postman-api",
                    ClientName = "Postman Test Client",
                    AllowedGrantTypes = GrantTypes.Code,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                    RedirectUris = { "https://www.getpostman.com/oauth2/callback" },
                    //NOTE: This link needs to match the link from the presentation layer - oidc-client
                    //      otherwise IdentityServer won't display the link to go back to the site
                    PostLogoutRedirectUris = { "https://www.getpostman.com" },
                    AllowedCorsOrigins = { "https://www.getpostman.com" },
                    EnableLocalLogin = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "postman_api"
                    },
                    ClientSecrets = new List<Secret>() { new Secret("SomeValue".Sha256()) }
                },
                new Client
                {
                    ClientId = "iosClient",
                    ClientName = "SPA ios",
                    AllowedGrantTypes = GrantTypes.Code,
                    AllowOfflineAccess = true,
                    RequireClientSecret = false,
                    RequirePkce = true,
                    RequireConsent = false,
                    RedirectUris = new List<string>{
                        $"{spaClientUrl}/auth-callback",
                    },
                    PostLogoutRedirectUris = new List<string>{
                        $"{spaClientUrl}/",
                    },
                    AllowedCorsOrigins = new List<string>{
                         $"{spaClientUrl}",
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "resourceApi"
                    }
                },
                new Client
                {
                    ClientId = "androidClient",
                    ClientName = "Android app",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireConsent = false,
                    AllowOfflineAccess = true,
                    ClientSecrets =
                    {
                        new Secret("my-secret".Sha256())
                    },
                    RefreshTokenUsage = TokenUsage.ReUse,
                    RedirectUris = new List<string>{
                        $"{spaClientUrl}/auth-callback",
                    },
                    PostLogoutRedirectUris = new List<string>{
                        $"{spaClientUrl}/",
                    },
                    AllowedCorsOrigins = new List<string>{
                         $"{spaClientUrl}",
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "resourceApi"
                    }
                },
                 new Client
                 {
                     ClientId = "spaCodeClient",
                     ClientName = "SPA Code Client",
                     AccessTokenType = AccessTokenType.Jwt,
                     // RequireConsent = false,
                     AccessTokenLifetime = 10800,// 3 hour
                     IdentityTokenLifetime = 10800,
                     RefreshTokenExpiration = TokenExpiration.Sliding,
                     RefreshTokenUsage = TokenUsage.OneTimeOnly,
                     SlidingRefreshTokenLifetime = 3600,
                     RequireClientSecret = false,
                     AllowedGrantTypes = GrantTypes.Code,
                     RequirePkce = true,

                     AllowAccessTokensViaBrowser = true,
                     RedirectUris = new List<string>
                    {
                        $"{spaClientUrl}/#/callback",
                        $"{spaClientUrl}/silent-renew.html",
                    },
                     PostLogoutRedirectUris = new List<string>
                    {
                        $"{spaClientUrl}/unauthorized",
                        $"{spaClientUrl}",
                    },
                     AllowedCorsOrigins = new List<string>
                    {
                        $"{spaClientUrl}",
                    },
                     AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1"
                    }
                 },
            };
        }

        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser>
            {
                new TestUser{SubjectId = "818727", Username = "alice", Password = "alice",
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Name, "Alice Smith"),
                        new Claim(JwtClaimTypes.GivenName, "Alice"),
                        new Claim(JwtClaimTypes.FamilyName, "Smith"),
                        new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                        new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServerConstants.ClaimValueTypes.Json)
                    }
                },
                new TestUser{SubjectId = "88421113", Username = "bob", Password = "bob",
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Name, "Bob Smith"),
                        new Claim(JwtClaimTypes.GivenName, "Bob"),
                        new Claim(JwtClaimTypes.FamilyName, "Smith"),
                        new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                        new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServerConstants.ClaimValueTypes.Json),
                        new Claim("location", "somewhere")
                    }
                }
            };
        }
    }
    //public class Config
    //{
    //    public static IEnumerable<IdentityResource> GetIdentityResources()
    //    {
    //        return new List<IdentityResource>
    //        {
    //            new IdentityResources.OpenId(),
    //            new IdentityResources.Email(),
    //            new IdentityResources.Profile(),
    //        };
    //    }

    //    public static IEnumerable<ApiResource> GetApiResources()
    //    {
    //        return new List<ApiResource>
    //        {
    //            new ApiResource("resourceapi", "Resource API")
    //            {
    //               // Scopes = {new Scope("api.read")}
    //            }
    //        };
    //    }
    //    public static IEnumerable<ApiResource> GetApis()
    //    {
    //        return new List<ApiResource>
    //        {
    //            new ApiResource("web_api", "My Web API")
    //        };
    //    }
    //    public static IEnumerable<Client> GetClients()
    //    {
    //        return new List<Client>
    //        {
    //            new Client
    //            {
    //                ClientId = "client",

    //                // no interactive user, use the clientid/secret for authentication
    //                AllowedGrantTypes = GrantTypes.ClientCredentials,

    //                // secret for authentication
    //                ClientSecrets =
    //                {
    //                    new Secret("secret".Sha256())
    //                },

    //                // scopes that client has access to
    //                AllowedScopes = { "web_api" }
    //            },
    //            // resource owner password grant client
    //            new Client
    //            {
    //                ClientId = "ro.client",
    //                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

    //                ClientSecrets =
    //                {
    //                    new Secret("secret".Sha256())
    //                },
    //                AllowedScopes = { "web_api" }
    //            },
    //            // OpenID Connect hybrid flow client (MVC)
    //            new Client
    //            {
    //                ClientId = "mvc",
    //                ClientName = "MVC Client",
    //                AllowedGrantTypes = GrantTypes.Hybrid,

    //                ClientSecrets =
    //                {
    //                    new Secret("secret".Sha256())
    //                },

    //                RedirectUris           = { "http://localhost:4200/signin-oidc" },
    //                PostLogoutRedirectUris = { "http://localhost:4200/signout-callback-oidc" },

    //                AllowedScopes =
    //                {
    //                    IdentityServerConstants.StandardScopes.OpenId,
    //                    IdentityServerConstants.StandardScopes.Profile,
    //                    "web_api"
    //                },

    //                AllowOfflineAccess = true
    //            },
    //            // JavaScript Client
    //            new Client
    //            {
    //                ClientId = "angular_spa",
    //                ClientName = "JavaScript Client",
    //                AllowedGrantTypes = GrantTypes.Code,
    //                RequirePkce = true,
    //                RequireClientSecret = false,

    //                RedirectUris =           { "http://localhost:4200/auth-callback" },
    //                PostLogoutRedirectUris = { "http://localhost:4200" },
    //                AllowedCorsOrigins =     { "http://localhost:4200" },

    //                AllowedScopes =
    //                {
    //                    IdentityServerConstants.StandardScopes.OpenId,
    //                    IdentityServerConstants.StandardScopes.Profile,
    //                    "web_api"
    //                }
    //            }
    //        };
    //    }
    //    //public static IEnumerable<Client> GetClients()
    //    //{
    //    //    return new[]
    //    //    {
    //    //        new Client {
    //    //            RequireConsent = false,
    //    //            ClientId = "angular_spa",
    //    //            ClientName = "Angular SPA",
    //    //            AllowedGrantTypes = GrantTypes.Implicit,
    //    //            AllowedScopes = { "openid", "profile", "email", "api.read" },
    //    //            RedirectUris = {"http://localhost:4200/auth-callback"},
    //    //            PostLogoutRedirectUris = {"http://localhost:4200/"},
    //    //            AllowedCorsOrigins = {"http://localhost:4200"},
    //    //            AllowAccessTokensViaBrowser = true,
    //    //            AccessTokenLifetime = 3600
    //    //        }
    //    //    };
    //    //}
    //}
}
