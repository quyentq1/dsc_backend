using dsc_backend.Helper;
using dsc_backend.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text.Encodings.Web;
using dsc_backend.DAO;
using System;

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
            // Kiểm tra nếu là Admin
            var existingAdmin = await _db.Admins
                .FirstOrDefaultAsync(y => y.Email == user.Email && y.Password == user.Password);

            if (existingAdmin != null)
            {
                // Đăng nhập thành công cho Admin
                return Ok(new
                {
                    Success = true,
                    Message = "Welcome Admin",
                    UserId = existingAdmin.AdminId,
                    Email = existingAdmin.Email,
                    RoleId = "Admin",
                    Fund = existingAdmin.Fund,
                });
            }

            // Kiểm tra nếu là User
            var existingUser = await _db.Users.FirstOrDefaultAsync(x => x.Email == user.Email);

            if (existingUser == null)
            {
                // Người dùng không tồn tại
                return BadRequest(new
                {
                    Success = false,
                    Message = "Sai Tài Khoản Hoặc Mật Khẩu!"
                });
            }

            // Mã hóa mật khẩu người dùng nhập vào
            var hashPass = _hashpass.MD5Hash(user.Password);

            // Kiểm tra mật khẩu
            var existingAccount = await _db.Users
                .FirstOrDefaultAsync(y => y.Email == user.Email && y.Password == hashPass);

            if (existingAccount == null)
            {
                // Mật khẩu sai
                return BadRequest(new
                {
                    Success = false,
                    Message = "Sai Tài Khoản Hoặc Mật Khẩu!"
                });
            }

            // Kiểm tra trạng thái người dùng
            if (existingAccount.Status != "Active")
            {
                // Trạng thái người dùng không phải "Active"
                return BadRequest(new
                {
                    Success = false,
                    Message = "Tài khoản của bạn đã bị vô hiệu hóa bởi Quản Trị Viên!"
                });
            }

            // Đăng nhập thành công cho User
            return Ok(new
            {
                Success = true,
                UserId = existingAccount.UserId,
                FullName = existingAccount.FullName,
                Email = existingAccount.Email,
                RoleId = existingAccount.RoleId,
                Avatar = existingAccount.Avatar,
                Status = existingAccount.Status
            });
        }

        [HttpPost("GoogleLogin")]
        public async Task<IActionResult> GoogleLogin(GoogleLoginDAO google)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == google.email);
                var Password = "Passmail";
                var hashPass = _hashpass.MD5Hash(Password);
                if (user == null)
                {
                    var newUsers = new User
                    {
                        FullName = google.name,
                        Email = google.email,
                        RoleId = 2,
                        Status = "Active",
                        Password = hashPass
                    };

                    _db.Users.Add(newUsers);
                    await _db.SaveChangesAsync();

                    // Query lại để lấy UserId của user vừa tạo
                    var newUser = await _db.Users.FirstOrDefaultAsync(x => x.Email == google.email);

                    var newUserResponse = new
                    {
                        userId = newUser.UserId,
                        email = newUser.Email,
                        fullName = newUser.FullName,
                        roleId = newUser.RoleId,
                        status = newUser.Status,
                        message = "Đăng ký tài khoản thành công",
                        success = true
                    };

                    return Ok(newUserResponse);
                }

                // Response cho user đã tồn tại
                var existingUserResponse = new
                {
                    userId = user.UserId,
                    email = user.Email,
                    fullName = user.FullName,
                    roleId = user.RoleId,
                    status = user.Status,
                    message = "Đăng nhập thành công",
                    success = true
                };

                return Ok(existingUserResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi server: {ex.Message}" });
            }
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
                    FullName = user.FullName,
                    RoleId = 2,
                    Status = "Active"
                };
                _db.Users.Add(newUser);
                _db.SaveChanges();
                var registerSuccess = new
                {
                    Success = true,
                    UserID = newUser.UserId,
                    FullName = user.FullName,
                    Email = newUser.Email,
                    Roleid = newUser.RoleId,
                    Status = newUser.Status
                };
                return Ok(registerSuccess);
            }
            var registerFailed = new
            {
                Success = false,
                Message = "Tài Khoản Email đã tồn tại trên website !"
            };
            return BadRequest(registerFailed);
        }

    }
}
