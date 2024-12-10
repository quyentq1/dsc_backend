using dsc_backend.Helper;
using dsc_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace dsc_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : Controller
    {
        private readonly ILogger<AuthenController> _logger;
        private readonly DscContext _db;
        private readonly Md5Helper _hashpass;
        private readonly CloudinarySettings _cloudinarySettings;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AdminController(ILogger<AuthenController> logger, DscContext db, IOptions<CloudinarySettings> cloudinarySettings, IWebHostEnvironment webHostEnvironment, Md5Helper hashpass)
        {
            _logger = logger;
            _hashpass = hashpass;
            _db = db;
            _cloudinarySettings = cloudinarySettings.Value;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet("listCustomer")]
        public async Task<IActionResult> ListCustomer()
        {
            try
            {
                // Giả sử bạn đang sử dụng Entity Framework Core và có DbContext tên là `AppDbContext`
                var users = await _db.Users.ToListAsync(); // Lấy toàn bộ danh sách Users từ bảng User

                // Trả về danh sách dưới dạng JSON
                return Ok(users);
            }
            catch (Exception ex)
            {
                // Log lỗi nếu có và trả về lỗi cho client
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách người dùng.", error = ex.Message });
            }
        }
        [HttpGet("getCustomerById/{customerId}")]
        public async Task<IActionResult> GetUserById(int customerId)
        {
            try
            {
                // Tìm User theo UserId
                var user = await _db.Users.FindAsync(customerId);

                if (user == null)
                {
                    // Nếu không tìm thấy User, trả về lỗi 404
                    return NotFound(new { message = $"Không tìm thấy người dùng với ID: {customerId}" });
                }

                // Trả về thông tin User
                return Ok(user);
            }
            catch (Exception ex)
            {
                // Log lỗi nếu có và trả về lỗi cho client
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy thông tin người dùng.", error = ex.Message });
            }
        }


    }

}
