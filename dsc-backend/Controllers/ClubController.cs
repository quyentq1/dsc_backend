using dsc_backend.Helper;
using dsc_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace dsc_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClubController : Controller
    {
        private readonly ILogger<AuthenController> _logger;
        private readonly DscContext _db;
        private readonly Md5Helper _hashpass;
        private readonly CloudinarySettings _cloudinarySettings;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ClubController(ILogger<AuthenController> logger, DscContext db, IOptions<CloudinarySettings> cloudinarySettings, IWebHostEnvironment webHostEnvironment, Md5Helper hashpass)
        {
            _logger = logger;
            _hashpass = hashpass;
            _db = db;
            _cloudinarySettings = cloudinarySettings.Value;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet("getAllClub")]
        public async Task<IActionResult> getAllClub()
        {
            var listClub = await _db.Clubs.ToListAsync();
            if (listClub != null)
            {
                var ListViewClub = new
                {
                    Success = true,
                    listClub
                };
                return Ok(ListViewClub);
            }
            else
            {
                var ListViewClub = new
                {
                    Success = false,
                    Message = "Chưa có Club..."
                };
                return BadRequest(ListViewClub);
            }
        }
        [HttpPost("requestJoinClub")]
        public async Task<IActionResult> requestJoinClub([FromBody] RequestJoinClub requestJoinClub)
        {
            if (requestJoinClub != null)
            {
                var joinClubnew = new RequestJoinClub
                {

                    UserId = requestJoinClub.UserId,
                    ClubId = requestJoinClub.ClubId,
                    Status = "1",
                    Createdate = DateTime.Now,
                };
                _db.RequestJoinClubs.Add(joinClubnew);
                _db.SaveChanges();
                var ListViewRequest = new
                {
                    Success = true,
                    UserId = requestJoinClub.UserId,
                    ClubId = requestJoinClub.ClubId,
                    Status = "1",
                    Createdate = DateTime.Now,
                    Message = "Đã gởi đơn xin gia nhập club thành công"

                };
                return Ok(ListViewRequest);
            }
            else
            {
                var ListViewRequest = new
                {
                    Success = false,
                    Message = "Có lỗi trong việc gởi đơn xin gia nhập club"
                };
                return BadRequest(ListViewRequest);
            }
        }

        [HttpPost("getrequestJoinClub")]
        public async Task<IActionResult> getrequestJoinClub([FromBody] RequestJoinClub requestJoinClub)
        {
            if (requestJoinClub != null)
            {
                var listjoinclub = await _db.RequestJoinClubs.Where(x => x.ClubId == requestJoinClub.ClubId)
                                       .OrderByDescending(x => x.Status)
                                       .ToListAsync();
                var ListViewRequest = new
                {
                    Success = true,
                    listjoinclub,

                };
                return Ok(ListViewRequest);
            }
            else
            {
                var ListViewRequest = new
                {
                    Success = false,
                    Message = "Có lỗi trong việc hiển thị đơn xin gia nhập club"
                };
                return BadRequest(ListViewRequest);
            }
        }
        [HttpPost("acceptrequestJoinClub")]
        public async Task<IActionResult> acceptrequestJoinClub([FromBody] RequestJoinClub requestJoinClub)
        {
            if (requestJoinClub != null)
            {
                var requestjoin = await _db.RequestJoinClubs.Where(x => x.RequestClubId == requestJoinClub.RequestClubId)
                                       .FirstOrDefaultAsync();
                if (requestjoin != null)
                {
                    // Cập nhật Status
                    requestjoin.Status = "2";

                    // Lưu thay đổi vào cơ sở dữ liệu
                    await _db.SaveChangesAsync();
                    var ListViewRequest = new
                    {
                        Success = true,
                        RequestJoin = requestjoin, // Có thể trả về requestjoin nếu cần
                        Message = "Đã xác nhận đơn xin gia nhập club thành công"
                    };
                    return Ok(ListViewRequest);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                var ListViewRequest = new
                {
                    Success = false,
                    Message = "Có lỗi trong việc xác nhận đơn xin gia nhập club"
                };
                return BadRequest(ListViewRequest);
            }
        }
        [HttpPost("cancelrequestJoinClub")]
        public async Task<IActionResult> cancelrequestJoinClub([FromBody] RequestJoinClub requestJoinClub)
        {
            if (requestJoinClub != null)
            {
                var requestjoin = await _db.RequestJoinClubs.Where(x => x.RequestClubId == requestJoinClub.RequestClubId)
                                       .FirstOrDefaultAsync();
                if (requestjoin != null)
                {
                    // Cập nhật Status
                    requestjoin.Status = "3";

                    // Lưu thay đổi vào cơ sở dữ liệu
                    await _db.SaveChangesAsync();
                    var ListViewRequest = new
                    {
                        Success = true,
                        RequestJoin = requestjoin, // Có thể trả về requestjoin nếu cần
                        Message = "Đã từ chối đơn xin gia nhập club thành công"
                    };
                    return Ok(ListViewRequest);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                var ListViewRequest = new
                {
                    Success = false,
                    Message = "Có lỗi trong việc từ chối đơn xin gia nhập club"
                };
                return BadRequest(ListViewRequest);
            }
        }
        [HttpPost("getClubDetails")]
        public async Task<IActionResult> getClubDetails([FromBody] Club clubs)
        {
            {
                var club = await _db.Clubs
                    .Include(c => c.Level)
                    .Include(c => c.Sport)
                    .Include(c => c.Payments)
                    .Include(c => c.RequestJoinClubs)
                    .Include(c => c.TransferHistoryReceiverClubs)
                    .Include(c => c.TransferHistorySenderClubs)
                    .Include(c => c.UserClubs)
                    .FirstOrDefaultAsync(c => c.ClubId == clubs.ClubId);

                if (club == null)
                {
                    return NotFound();
                }

                return Ok(club);
            }
        }


    }
}
