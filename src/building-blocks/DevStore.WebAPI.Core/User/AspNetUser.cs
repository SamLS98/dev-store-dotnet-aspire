using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace DevStore.WebAPI.Core.User
{
    public class AspNetUser(IHttpContextAccessor accessor) : IAspNetUser
    {
        public string Name => accessor.HttpContext.User.Identity.Name;

        public Guid GetUserId()
        {
            return IsAuthenticated() ? Guid.Parse(accessor.HttpContext.User.GetUserId()) : Guid.Empty;
        }

        public string GetUserEmail()
        {
            return IsAuthenticated() ? accessor.HttpContext.User.GetUserEmail() : "";
        }

        public string GetUserToken()
        {
            return IsAuthenticated() ? accessor.HttpContext.User.GetUserToken() : "";
        }

        public string GetUserRefreshToken()
        {
            return IsAuthenticated() ? accessor.HttpContext.User.GetUserRefreshToken() : "";
        }

        public bool IsAuthenticated()
        {
            return accessor.HttpContext.User.Identity.IsAuthenticated;
        }

        public bool IsInRole(string role)
        {
            return accessor.HttpContext.User.IsInRole(role);
        }

        public IEnumerable<Claim> GetClaims()
        {
            return accessor.HttpContext.User.Claims;
        }

        public HttpContext GetHttpContext()
        {
            return accessor.HttpContext;
        }
    }
}