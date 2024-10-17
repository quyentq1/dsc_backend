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
                        Height= existingUser.Height,
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
                    return BadRequest("Email is required.");
                }

                var existingUser = await _db.Users.FirstOrDefaultAsync(x => x.Email == user.Email);

                if (existingUser == null)
                {
                    return NotFound("User not found.");
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
        }
}
