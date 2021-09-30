using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reactivities.Application.Activities;
using Reactivities.Domain;
using Reactivities.Persistence;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Reactivities.Api.Controllers
{
    public class ActivitiesController : BaseApiController
    {
        private DataContext Context { get; }
        public UserManager<AppUser> UserManager { get; }
        public IMapper Mapper { get; }

        public ActivitiesController(DataContext context,
            UserManager<AppUser> userManager,
            IMapper mapper)
        {
            Context = context;
            UserManager = userManager;
            Mapper = mapper;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<ActivityDto>>> GetActivities()
        {
            return await Context.Activities.ProjectTo<ActivityDto>(Mapper.ConfigurationProvider).ToListAsync();
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ActivityDto>> GetActivity(Guid id)
        {
            var result = await Context.Activities.Include(a => a.Attendees).ProjectTo<ActivityDto>(Mapper.ConfigurationProvider).FirstOrDefaultAsync(x => x.Id == id);

            if (result == null)
                return NotFound();
            return result;
        }

        [HttpPost]
        public async Task<ActionResult> Create(Activity activity)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await UserManager.FindByEmailAsync(email);

            var attendee = new ActivityAttendee
            {
                AppUser = user,
                Activity = activity,
                IsHost = true
            };

            activity.Attendees.Add(attendee);
            await Context.Activities.AddAsync(activity);
            await Context.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Policy ="IsActivityHost")]
        [HttpPut]
        public async Task<ActionResult> Update(Activity activity)
        {

            var retrievedActivity = await Context.Activities.SingleOrDefaultAsync(a => a.Id == activity.Id);

            if (retrievedActivity == null)
                return NotFound();

            retrievedActivity.Title = activity.Title;
            retrievedActivity.Date = activity.Date;
            retrievedActivity.Description = activity.Description;
            retrievedActivity.Category = activity.Category;
            retrievedActivity.City = activity.City;
            retrievedActivity.Venue = activity.Venue;

            await Context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("{id}/attend")]
        public async Task<ActionResult> Attend(Guid id)
        {

            var retrievedActivity = await Context.Activities
                .Include(a => a.Attendees)
                .ThenInclude(x => x.AppUser)
                .SingleOrDefaultAsync(a => a.Id == id);

            if (retrievedActivity == null)
                return NotFound();

            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var tokenClaims = new JwtSecurityToken(token.Replace("Bearer ", string.Empty));
            var email = tokenClaims.Payload["email"].ToString();
            var user = await UserManager.FindByEmailAsync(email);

            var hostName = retrievedActivity.Attendees.FirstOrDefault(x => x.IsHost)?.AppUser?.UserName;
            var attendance = retrievedActivity.Attendees.FirstOrDefault(x => x.AppUser.UserName == user.UserName);

            if (attendance != null && hostName == user.UserName)
                retrievedActivity.IsCancelled = !retrievedActivity.IsCancelled;

            if (attendance != null && hostName != user.UserName)
                retrievedActivity.Attendees.Remove(attendance);

            if (attendance == null)
            {
                var attendee = new ActivityAttendee
                {
                    AppUser = user,
                    Activity = retrievedActivity,
                    IsHost = false
                };

                retrievedActivity.Attendees.Add(attendee);
                
            }

            var result = await Context.SaveChangesAsync()>0;

            if(result)
                return Ok();
            else
                return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Activity>> Delete(Guid id)
        {
            var activity = await Context.Activities.SingleOrDefaultAsync(a => a.Id == id);
            if (activity == null)
                return NotFound();

            Context.Activities.Remove(activity);
            await Context.SaveChangesAsync();

            return Ok();
        }

    }
}
