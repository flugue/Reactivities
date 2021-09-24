using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reactivities.Domain;
using Reactivities.Persistence;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reactivities.Api.Controllers
{
    [AllowAnonymous]
    public class ActivitiesController : BaseApiController
    {
        private DataContext Context { get;}
        public ActivitiesController(DataContext context)
        {
            Context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Activity>>> GetActivities()
        {
            return await Context.Activities.ToListAsync();
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Activity>> GetActivity(Guid id)
        {
            var result =  await Context.Activities.FindAsync(id);

            if (result == null)
                return NotFound();
            return result;
        }

        [HttpPost]
        public async Task<ActionResult> Create(Activity activity)
        {
            await Context.Activities.AddAsync(activity);
            await Context.SaveChangesAsync();

            return Ok();
        }


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
