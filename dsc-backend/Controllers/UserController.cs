﻿using Azure.Core;
using dsc_backend.DAO;
using dsc_backend.Helper;
using dsc_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace dsc_backend.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly ILogger<AuthenController> _logger;
        private readonly DscContext _db;
        private readonly Md5Helper _hashpass;
        private readonly CloudinarySettings _cloudinarySettings;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public UserController(ILogger<AuthenController> logger, DscContext db, IOptions<CloudinarySettings> cloudinarySettings, IWebHostEnvironment webHostEnvironment, Md5Helper hashpass)
        {
            _logger = logger;
            _hashpass = hashpass;
            _db = db;
            _cloudinarySettings = cloudinarySettings.Value;
            _webHostEnvironment = webHostEnvironment;
        }

        /*
         Controller UseCase ViewProfile
         */
        [HttpPost("getinfor")]
        public async Task<IActionResult> GetInfor([FromBody] User user)
        {
            if (user.Email != null)
            {
                var existingUser = await _db.Users.FirstOrDefaultAsync(x => x.Email == user.Email);
                var InforUser = new
                {
                    Userid = existingUser.UserId,
                    FullName = existingUser.FullName,
                    Address = existingUser.Address,
                    Phone = existingUser.Phone,
                    Email = user.Email,
                    RoleId = existingUser.RoleId,
                    Age = existingUser.Age,
                    Height = existingUser.Height,
                    Weight = existingUser.Weight,
                    Avatar = existingUser.Avatar,
                    Status = existingUser.Status,
                };
                return Ok(InforUser);
            }
            else { return BadRequest(); }
        }
        [HttpPost("updateinfor")]
        public async Task<IActionResult> UpdateInfor([FromBody] User user)
        {

            if (string.IsNullOrEmpty(user.Email))
            {
                return BadRequest("Tài Khoản Email không được trống.");
            }

            var existingUser = await _db.Users.FirstOrDefaultAsync(x => x.Email == user.Email);

            if (existingUser == null)
            {
                return NotFound("Người Dùng Không Tồn Tại.");
            }
            existingUser.FullName = user.FullName ?? existingUser.FullName;
            existingUser.Address = user.Address ?? existingUser.Address;
            existingUser.Phone = user.Phone ?? existingUser.Phone;
            existingUser.Age = user.Age ?? existingUser.Age;
            existingUser.Weight = user.Weight ?? existingUser.Weight;
            existingUser.Avatar = user.Avatar ?? existingUser.Avatar;
            existingUser.Height = existingUser.Height ?? existingUser.Height;
            _db.Users.Update(existingUser);
            await _db.SaveChangesAsync();
            var updatedUserInfo = new
            {
                UserId = existingUser.UserId,
                FullName = existingUser.FullName,
                Address = existingUser.Address,
                Phone = existingUser.Phone,
                Email = existingUser.Email,
                Age = existingUser.Age,
                Weight = existingUser.Weight,
                Avatar = existingUser.Avatar,
                Height = existingUser.Height,
            };

            return Ok(updatedUserInfo);
        }
        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDAO request)
        {
            var hashPass = _hashpass.MD5Hash(request.Password);
            var existingUser = await _db.Users.FirstOrDefaultAsync(x => x.Email == request.Email && x.Password == hashPass);
            if (existingUser != null)
            {
                string hashedPasswordNew = _hashpass.MD5Hash(request.NewPassword);
                existingUser.Password = hashedPasswordNew;
                await _db.SaveChangesAsync();
                var InforChangePass = new
                {
                    Success = true,
                    Email = existingUser.Email,
                    Message = "Thay đổi mật khẩu thành công"
                };
                return Ok(InforChangePass);
            }
            else
            {
                var InforChangePass = new
                {
                    Success = false,
                    Email = existingUser.Email,
                    Message = "Tài Khoản Không Tồn Tại"
                };
                return BadRequest(InforChangePass);
            }
        }
        /*
        Controller UseCase Viewyoursportslist
        */
        [HttpGet("getSportName")]
        public async Task<IActionResult> getSportName()
        {
            var listSport = await _db.Sports.ToListAsync();
            if (listSport != null)
            {
                var ListViewSport = new
                {
                    Success = true,
                    listSport
                };
                return Ok(ListViewSport);
            }
            else
            {
                var ListViewSport = new
                {
                    Success = false,
                    Message = "Chưa có Sport..."
                };
                return BadRequest(ListViewSport);
            }
        }

        [HttpGet("getLevel")]
        public async Task<IActionResult> getLevel()
        {
            var listLevel = await _db.Levels.ToListAsync();
            if (listLevel != null)
            {
                var ListViewLevel = new
                {
                    Success = true,
                    listLevel
                };
                return Ok(ListViewLevel);
            }
            else
            {
                var ListViewLevel = new
                {
                    Success = false,
                    Message = "Chưa có Sport..."
                };
                return BadRequest(ListViewLevel);
            }
        }
        [HttpPost("getMySport")]
        public async Task<IActionResult> getMySport([FromBody] UserSport usersport)
        {
            var userSportsList = await _db.UserSports
                                       .Where(x => x.UserId == usersport.UserId)
                                       .ToListAsync();
            if (userSportsList != null)
            {
                var ListViewSport = new
                {
                    Success = true,
                    userSportsList
                };
                return Ok(ListViewSport);
            }
            else
            {
                var ListViewSport = new
                {
                    Success = false,
                    Message = "Chưa có Sport..."
                };
                return BadRequest(ListViewSport);
            }
        }
        [HttpPost("AddSportName")]
        public async Task<IActionResult> AddSportName([FromBody] UserSport usersport)
        {
            var existingUser = await _db.Users.FirstOrDefaultAsync(x => x.UserId == usersport.UserId);
            if (existingUser != null)
            {
                var UserSports = new UserSport
                {
                    UserId = usersport.UserId,
                    SportId = usersport.SportId,
                    LevelId = usersport.LevelId,
                    Achievement = usersport.Achievement,
                    Position = usersport.Position,
                };
                _db.UserSports.Add(UserSports);
                _db.SaveChanges();
                var ListViewSport = new
                {
                    Success = true,
                    UserId = UserSports.UserId,
                    SportId = UserSports.SportId,
                    LevelId = UserSports.LevelId,
                    Achievement = UserSports.Achievement,
                    Position = UserSports.Position,
                    Message = "Đã thêm môn thể thao thành công"

                };
                return Ok(ListViewSport);
            }
            else
            {
                var ListViewSport = new
                {
                    Success = false,
                    Message = "Chưa có Sport..."
                };
                return BadRequest(ListViewSport);
            }
        }

    }

}
