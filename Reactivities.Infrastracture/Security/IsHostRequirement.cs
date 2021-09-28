using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Reactivities.Persistence;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Reactivities.Infrastracture.Security
{
    public class IsHostRequirement: IAuthorizationRequirement
    {
    }

    public class IsHostRequirementHandler : AuthorizationHandler<IsHostRequirement>
    {
        public IsHostRequirementHandler(DataContext dbContext,
            IHttpContextAccessor httpContextAccessor)
        {
            DbContext = dbContext;
            HttpContextAccessor = httpContextAccessor;
        }

        public DataContext DbContext { get; }
        public IHttpContextAccessor HttpContextAccessor { get; }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHostRequirement requirement)
        {
            var token = HttpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var tokenClaims = new JwtSecurityToken(token.Replace("Bearer ", string.Empty));
            var userId = tokenClaims.Payload["id"].ToString();

            if (userId == null) return Task.CompletedTask;

            var activityId = Guid.Parse(HttpContextAccessor.HttpContext?.Request.RouteValues.SingleOrDefault(x => x.Key == "id").Value?.ToString());
            var attendee = DbContext.ActivityAttendees.FindAsync(activityId, userId).Result;

            if (attendee==null) return Task.CompletedTask;

            if (attendee.IsHost) context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
