using Microsoft.AspNetCore.Http;
using Reactivities.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

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
            var token = HtttpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var tokenClaims = new JwtSecurityToken(token.Replace("Bearer ", string.Empty));
            return tokenClaims.Payload["name"].ToString();
        }
    }
}
