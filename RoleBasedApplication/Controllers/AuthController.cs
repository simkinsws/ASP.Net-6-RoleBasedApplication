using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RoleBasedApplication.Entities;
using RoleBasedApplication.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RoleBasedApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private DataBaseContext _context;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthController(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IUserService userService,
            DataBaseContext dataBaseContext)
        {
            _configuration = configuration;
            _userService = userService;
            _context = dataBaseContext;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("GetRole"), Authorize]
        public ActionResult<string> GetRole()
        {
            var userRole = _userService.getRole();
            return Ok(userRole);
        }

        [HttpGet("GetUserName"), Authorize]
        public async Task<ActionResult<string>> GetUserName()
        {
            var userName = await _userService.getUserName();
            return Ok(userName);
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponseDto>> Register(RegisterDto request)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var user = new UserModel();
            var userName = _context.Users.FirstOrDefault(u => u.Username.Equals(request.Username));
            if(userName != null)
            {
                return BadRequest("User with user name {" + userName.Username + "} Already Exists");
            }
            user.Username = request.Username;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.Role = "User";
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(new RegisterResponseDto()
            {
                Username = user.Username,
                Password = request.Password,
                Role = user.Role
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginDto request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username.Equals(request.Username));
            if(user == null)
            {
                return BadRequest("User not found");
            }
            if(!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Wrong password");
            }

            string token = CreateToken(user);
            LoginResponseDto response = new LoginResponseDto()
            {
                Role = user.Role,
                UserName = user.Username,
                token = token
            };
            Response.Cookies.Append("Authorization", token, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddMinutes(30),
                IsEssential = true,
                SameSite = SameSiteMode.None,
                Secure = true,
            });
            return Ok(response);
        }


        [HttpPost("Logout"), Authorize]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("Authorization");

            return Ok(new
            {
                message = "you have been logged out succesfully"
            });
        }

        [HttpGet("isUserAuthenticated"), Authorize]
        public IActionResult isUserAuthenticated()
        {
            try
            {
                var jwt = Request.Cookies["Authorization"];


                var loggedInUserName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);

                var token = verify(jwt);

                var claims = token.Claims;

                var userNameToCheck = "";

                foreach(var name in claims)
                {
                    if(name.Value == loggedInUserName)
                    {
                        userNameToCheck = name.Value;
                    }
                }

                var user = _context.Users.Where(u => u.Username.Equals(userNameToCheck)).FirstOrDefault();

                return Ok(user);
            } catch(Exception e)
            {
                return Unauthorized();
            }
        }

        private string CreateToken(UserModel user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name, user.Username),
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private JwtSecurityToken verify(string jwt)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Token").Value);
            tokenHandler.ValidateToken(jwt, new TokenValidationParameters()
            {
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false
            }, out SecurityToken validatedToken);

            return (JwtSecurityToken)validatedToken;
        }
    }
}
