using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MoviesAPI.DTOs;
using MoviesAPI.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration configuration;
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public AccountsController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration,
            ApplicationDBContext context,
            IMapper mapper
            )
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.context = context;
            this.mapper = mapper;
        }

        [HttpPost("Create")]
        public async Task<ActionResult<UserToken>> CreateUser([FromBody] UserInfo model)
        {
            var user = new IdentityUser { Email = model.EmailAddress, UserName = model.EmailAddress };
            var result = await userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                return await BuildToken(model);
            }
            else
            {
                return BadRequest(result.Errors);
            }

        }
        [HttpPost("Login")]
        public async Task<ActionResult<UserToken>> Login([FromBody] UserInfo model)
        {
            var result = await signInManager.PasswordSignInAsync(model.EmailAddress, model.Password, false, false);
            if (result.Succeeded)
            {
                return await BuildToken(model);
            }
            else
            {
                return BadRequest("Invalid login attempt");
            }
        }

        [HttpPost("RenewToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<UserToken>> Renew()
        {
            UserInfo userInfo = new()
            {
                EmailAddress = HttpContext.User.Identity.Name
            };
            return await BuildToken(userInfo);
        }
        private async Task<UserToken> BuildToken(UserInfo userInfo)
        {
            var claims = new List<Claim>()
           {
               new (ClaimTypes.Name,userInfo.EmailAddress),
               new (ClaimTypes.Email,userInfo.EmailAddress),
               new ("mykey","whatever value I want")
            };
            var IdentityUser = await userManager.FindByEmailAsync(userInfo.EmailAddress);
            if (IdentityUser != null)
            {
                var claimsDB = await userManager.GetClaimsAsync(IdentityUser);
                claims.AddRange(claimsDB);
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddMinutes(20);

            JwtSecurityToken token = new(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiration,
                signingCredentials: creds
                );

            return new UserToken
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration,
            };
        }

        [HttpGet("users")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<List<UserDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var quaryable = context.Users.AsQueryable();
            quaryable = quaryable.OrderBy(x => x.Email);
            await HttpContext.InsertPaginationHeaderInResponse(quaryable, paginationDTO.RecordsPerPage);
            var users = await quaryable.Paginate(paginationDTO).ToListAsync();
            return mapper.Map<List<UserDTO>>(users);
        }

        [HttpGet("roles")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<List<string?>>> Get()
        {
            return await context.Roles.Select(x => x.Name).ToListAsync();
        }

        [HttpPost("assignRole")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> AssignRole(EditRoleDTO editRoleDTO)
        {
            var user = await userManager.FindByIdAsync(editRoleDTO.UserId);
            if (user == null)
            {
                return NotFound();
            }
            await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, value: editRoleDTO.RoleName));
            return NoContent();
        }

        [HttpPost("removeRole")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> RemoveRole(EditRoleDTO editRoleDTO)
        {
            var user = await userManager.FindByIdAsync(editRoleDTO.UserId);
            if (user == null)
            {
                return NotFound();
            }
            await userManager.RemoveClaimAsync(user, new Claim(ClaimTypes.Role, value: editRoleDTO.RoleName));
            return NoContent();
        }
    }
}
