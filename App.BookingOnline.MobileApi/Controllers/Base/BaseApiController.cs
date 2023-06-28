
using App.Core.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;
using System.Collections.Generic;

namespace App.BookingOnline.AppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BaseApiController : ControllerBase
    {
        protected ClaimsIdentity ClaimsIdentity => User.Identity as ClaimsIdentity;
        protected string UserName => ClaimsIdentity.FindFirst("name")?.Value;
        protected string UserId => ClaimsIdentity.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
        protected string CurOrgId => ClaimsIdentity.FindFirst("org")?.Value;
        protected List<string> Roles => ClaimsIdentity.FindAll("http://schemas.microsoft.com/ws/2008/06/identity/claims/role").ToList().Select(s => s.Value).ToList();

        protected RespondData Success(object Data = null)
        {
            return new RespondData { IsSuccess = true, Data = Data };
        }

        protected RespondData Failure(string message = "", object Data = null)
        {
            return new RespondData { IsSuccess = false, Message = message, Data = Data };
        }
    }
}