using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reactivities.Api.Services;
using Reactivities.Application.Profiles;
using Reactivities.Domain;
using Reactivities.Persistence;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Reactivities.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfilesController : ControllerBase
    {

        public ProfilesController(UserManager<AppUser> userManager,
                                    SignInManager<AppUser> signInManager,
                                    TokenService tokenService,
                                    DataContext context, IMapper mapper)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            TokenService = tokenService;
            Context = context;
            Mapper = mapper;
        }

        public UserManager<AppUser> UserManager { get; }
        public SignInManager<AppUser> SignInManager { get; }
        public TokenService TokenService { get; }
        public DataContext Context { get; }
        public IMapper Mapper { get; }

        [HttpGet("{username}")]
        public async Task<ActionResult<Application.Profiles.Profile>> GetDetails(string userName)
        {
            var user = await Context.Users.ProjectTo<Application.Profiles.Profile>(Mapper.ConfigurationProvider).SingleOrDefaultAsync(u => u.Username == userName);
            if (user == null)
                return NotFound();

            return Ok(user);
        }
    }
}
