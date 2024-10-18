using dsc_backend.Helper;
using dsc_backend.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace dsc_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenController : Controller
    {
        private readonly ILogger<AuthenController> _logger;
        private readonly DscContext _db;
        private readonly Md5Helper _hashpass;
        private readonly CloudinarySettings _cloudinarySettings;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AuthenController(ILogger<AuthenController> logger, DscContext db, IOptions<CloudinarySettings> cloudinarySettings, IWebHostEnvironment webHostEnvironment, Md5Helper hashpass)
        {
            _logger = logger;
            _hashpass = hashpass;
            _db = db;
            _cloudinarySettings = cloudinarySettings.Value;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginEmail([FromBody] User user)
        {
            var existingUser = await _db.Users.FirstOrDefaultAsync(x => x.Email == user.Email);
            if (existingUser != null)
            {
                if (existingUser.Password != null)
                {
                    var hashPass = _hashpass.MD5Hash(user.Password);
                    var existingAcount = await _db.Users.FirstOrDefaultAsync(y => y.Email == user.Email && y.Password == hashPass);
                    if (existingAcount != null)
                    {
                        var InforLogin = new
                        {
                            Success = true,
                            FullName = existingAcount.FullName,
                            Email = existingAcount.Email,
                            Roleid = existingAcount.RoleId
                        };
                        return Ok(InforLogin);
                    }
                    else
                    {
                        var InforLogin = new
                        {
                            Success = false,
                            Message = "Wrong Email or Password !"
                        };
                        return BadRequest(InforLogin);
                    }
                }
                else
                {
                    var InforLogin = new
                    {
                        Success = false,
                        Message = "User login by account Google. Not yet register by email and password !"
                    };
                    return BadRequest(InforLogin);
                }
            }
            else
            {
                var InforLogin = new
                {
                    Success = false,
                    Message = "Not found User !"
                };
                return BadRequest(InforLogin);
            }
        }

        [HttpGet("login-google")]
        public IActionResult LoginGoogle()
        {
            var props = new AuthenticationProperties { RedirectUri = "Authen/signin-google" };
            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }
        [HttpGet("signin-google")]
        public async Task<IActionResult> GoogleLogin()
        {
            var response = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (response.Principal == null) return BadRequest();

            var name = response.Principal.FindFirstValue(ClaimTypes.Name);
            var givenName = response.Principal.FindFirstValue(ClaimTypes.GivenName);
            var email = response.Principal.FindFirstValue(ClaimTypes.Email);
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null)
            {
                var newUsers = new User
                {
                    FullName = name,
                    Email = email,
                    RoleId = 2
                };
                _db.Users.Add(newUsers);
                _db.SaveChanges();
            }
            return Ok(email);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            var existingUser = await _db.Users.FirstOrDefaultAsync(x => x.Email == user.Email);
            if (existingUser == null)
            {
                var hashPass = _hashpass.MD5Hash(user.Password);
                var newUser = new User
                {
                    Email = user.Email,
                    Password = hashPass,
                    RoleId = 2
                };
                _db.Users.Add(newUser);
                _db.SaveChanges();
                var registerSuccess = new
                {
                    Success = true,
                    Email = newUser.Email,
                    Roleid = newUser.RoleId
                };
                return Ok(registerSuccess);
            }
            var registerFailed = new
            {
                Success = false,
                Message = "Email user was exist on websites !"
            };
            return BadRequest(registerFailed);
        }

    }
}
