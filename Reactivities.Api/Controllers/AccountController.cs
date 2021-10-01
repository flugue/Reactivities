using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reactivities.Api.DTOs;
using Reactivities.Api.Services;
using Reactivities.Domain;
using Reactivities.Persistence;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Reactivities.Api.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        public AccountController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            TokenService tokenService,
            DataContext context)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            TokenService = tokenService;
            Context = context;
        }

        public UserManager<AppUser> UserManager { get; }
        public SignInManager<AppUser> SignInManager { get; }
        public TokenService TokenService { get; }
        public DataContext Context { get; }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto dto)
        {
            var user = await UserManager.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.Email == dto.Email);

            if (user == null)
                return Unauthorized();

            var result = await SignInManager.CheckPasswordSignInAsync(user, dto.Password, false);

            if (result.Succeeded)
                return CreateUserObject(user);

            return Unauthorized();
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto dto)
        {
            if (await UserManager.Users.AnyAsync(x => x.Email == dto.Email))
                return BadRequest("Email Taken");
            if (await UserManager.Users.AnyAsync(x => x.UserName == dto.Username))
                return BadRequest("Username Taken");

            var user = new AppUser
            {
                DisplayName = dto.DisplayName,
                Email = dto.Email,
                UserName = dto.Username
            };

            var result = await UserManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
            {
                return new UserDto
                {
                    DisplayName = user.DisplayName,
                    Image = null,
                    Token = TokenService.CreateToken(user),
                    Username = dto.Username,
                };
            }

            return BadRequest("Problem Registering User");
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await UserManager.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));
            return CreateUserObject(user);
        }


        private UserDto CreateUserObject(AppUser user)
        {
            return new UserDto
            {
                DisplayName = user.DisplayName,
                Username = user.UserName,
                Image = user?.Photos?.FirstOrDefault(x => x.IsMain)?.Url,
                Token = TokenService.CreateToken(user)
            };
        }
    }
}
