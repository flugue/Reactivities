using Microsoft.AspNetCore.Http;
using Reactivities.Application.Interfaces;
using System.Security.Claims;

namespace Reactivities.Infrastracture.Security
{
    public class UserAccessor : IUserAccessor
    {
        public UserAccessor(IHttpContextAccessor htttpContextAccessor)
        {
            HtttpContextAccessor = htttpContextAccessor;
        }

        public IHttpContextAccessor HtttpContextAccessor { get; }

        public string GetUsername()
        {
            return HtttpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        }
    }
}
