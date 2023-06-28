using AutoMapper;
using App.BookingOnline.Api.Validators;
using App.BookingOnline.Api.ViewModels;
using App.Core.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using App.BookingOnline.Service;
using App.BookingOnline.Service.DTO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using App.Core;
using System;
using App.BookingOnline.Data.Identity;
using System.Linq;

namespace App.BookingOnline.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BaseApiController : ControllerBase
    {

        protected ClaimsIdentity ClaimsIdentity => User.Identity as ClaimsIdentity;

        protected string UserName => ClaimsIdentity.FindFirst("name")?.Value;
        protected string CurOrgId => ClaimsIdentity.FindFirst("org")?.Value;
        protected string UserId => ClaimsIdentity.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
        protected List<string> Roles => ClaimsIdentity.FindAll("http://schemas.microsoft.com/ws/2008/06/identity/claims/role").ToList().Select(s => s.Value).ToList();

        protected RespondData Success(object Data = null)
        {
            return new RespondData { IsSuccess = true, Data = Data };
        }

        protected RespondData Failure(string Code = "", string message = "", object Data = null)
        {
            return new RespondData { IsSuccess = false, Message = message, Data = Data };
        }

    }


}