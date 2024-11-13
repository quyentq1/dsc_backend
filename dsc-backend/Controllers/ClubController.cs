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
        public async Task<IActionResult> getAllClub([FromQuery] int userId)
        {
            var userCounts = await _db.UserClubs
                .GroupBy(uc => uc.ClubId)
                .Select(g => new
                {
                    ClubId = g.Key,
                    UserCount = g.Count()
                })
                .ToDictionaryAsync(x => x.ClubId, x => x.UserCount);
            var listClub = await _db.Clubs
                .Where(club => !_db.UserClubs
                    .Any(uc => uc.UserId == userId && uc.ClubId == club.ClubId))
                .OrderByDescending(a => a.CreateDate)
                .Select(a => new
                {
                    a.ClubId,
                    a.ClubName,
                    LevelName = a.Level.LevelName,
                    a.Status,
                    UserCount = userCounts.ContainsKey(a.ClubId) ? userCounts[a.ClubId] : 0,
                    a.Avatar
                })
                .ToListAsync();

            if (listClub.Any())
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
            var requestExist = await _db.RequestJoinClubs.Where(x => x.ClubId == requestJoinClub.ClubId && x.UserId == requestJoinClub.UserId).FirstOrDefaultAsync();
            if (requestExist != null)
            {
                var ListViewRequest = new
                {
                    Success = true,
                    Message = "Đã gởi đơn xin tham gia trước đó rồi. Vui lòng chờ duyệt !"

                };
                return Ok(ListViewRequest);
            }
            else
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
        [HttpGet("getDetailClub/{clubId}")]
        public async Task<IActionResult> GetClubDetails(int clubId)
        {
            // Lấy thông tin Club và tên Leader
            var club = await _db.Clubs
                .Where(c => c.ClubId == clubId)
                .Select(c => new
                {
                    c.ClubId,
                    c.ClubName,
                    LevelName = c.Level.LevelName,
                    c.Status,
                    c.Avatar,
                    c.Rules,
                    UserCount = _db.UserClubs.Count(uc => uc.ClubId == c.ClubId),
                    LeaderFullName = _db.UserClubs
                        .Where(uc => uc.ClubId == c.ClubId && uc.Role == "Leader")
                        .Join(_db.Users, uc => uc.UserId, u => u.UserId, (uc, u) => u.FullName)
                        .FirstOrDefault(),
                    AvatarLeader = _db.UserClubs
                        .Where(uc => uc.ClubId == c.ClubId && uc.Role == "Leader")
                        .Join(_db.Users, uc => uc.UserId, u => u.UserId, (uc, u) => u.Avatar)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            // Kiểm tra xem Club có tồn tại hay không
            if (club == null)
            {
                var errorResponse = new
                {
                    Success = false,
                    Message = "Club không tồn tại."
                };
                return BadRequest(errorResponse);
            }

            // Trả về thông tin chi tiết của Club
            var response = new
            {
                Success = true,
                ClubDetails = new
                {
                    club.ClubId,
                    club.ClubName,
                    club.Rules,
                    club.LevelName,
                    club.Status,
                    club.Avatar,
                    club.UserCount,

                    LeaderFullName = club.LeaderFullName,
                    AvatarLeader = club.AvatarLeader,
                    // Tên của leader
                }
            };

            return Ok(response);
        }
        [HttpGet("getMyClub")]
        public async Task<IActionResult> getMyClub([FromQuery] int userId)
        {
            var userCounts = await _db.UserClubs
                .GroupBy(uc => uc.ClubId)
                .Select(g => new
                {
                    ClubId = g.Key,
                    UserCount = g.Count()
                })
                .ToDictionaryAsync(x => x.ClubId, x => x.UserCount);
            var listClub = await _db.UserClubs
                .Where(ua => ua.UserId == userId)
                .Include(ua => ua.Club)                
                    .ThenInclude(a => a.Level)
                                    .OrderByDescending(ua => ua.Club.CreateDate)
                .Select(ua => new
                {
                    ua.Club.ClubId,               
                    ua.Club.ClubName,             
                    ua.Club.Avatar,             
                    ua.Club.CreateDate,
                    ua.Club.Status,
                    LevelName = ua.Club.Level.LevelName,
                    UserCount = userCounts.ContainsKey(ua.ClubId) ? userCounts[ua.ClubId] : 0,
                })
                .ToListAsync();

            if (listClub.Any())
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
        [HttpGet("getMemberClub/{clubId}")]
        public async Task<IActionResult> getMemberClub(int clubId)
        {
            // Lấy thông tin câu lạc bộ và danh sách thành viên từ UserClub
            var club = await _db.Clubs
                 .Where(c => c.ClubId == clubId)
                 .Select(c => new
                 {
                     c.ClubId,
                     c.ClubName,
                     LevelName = c.Level.LevelName,
                     c.Status,
                     c.Avatar,
                     c.Rules,
                     UserCount = _db.UserClubs.Count(uc => uc.ClubId == c.ClubId), // Số lượng người dùng tham gia câu lạc bộ
                     Users = _db.UserClubs
                         .Where(uc => uc.ClubId == c.ClubId)  // Lọc danh sách UserClub theo ClubId
                         .Join(_db.Users,
                               uc => uc.UserId,  // Điều kiện join UserId từ UserClub
                               u => u.UserId,    // với User.UserId
                               (uc, u) => new    // Lấy thông tin FullName và Avatar của người dùng
                               {
                                   u.FullName,
                                   u.Avatar
                               })
                         .ToList()  // Chuyển thành danh sách các người dùng
                 })
                 .FirstOrDefaultAsync();

            if (club == null)
            {
                return NotFound();  // Trả về lỗi nếu không tìm thấy câu lạc bộ
            }

            return Ok(club);  // Trả về thông tin câu lạc bộ và danh sách thành viên
        }


    }
}
