using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using dsc_backend.DAO;
using dsc_backend.Helper;
using dsc_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Diagnostics;

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
                    .Where(club => club.Status == "Active" && // Thêm điều kiện Status là Active
                                   !_db.UserClubs.Any(uc => uc.UserId == userId && uc.ClubId == club.ClubId))
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
        [HttpGet("getClubJoined")]
        public async Task<IActionResult> getClubJoined([FromQuery] int userId)
        {
            // Lấy danh sách Club mà user đã tham gia với role là "Player" và có Status là "Active"
            var listClub = await _db.UserClubs
                .Where(uc => uc.UserId == userId && uc.Role == "Player" && uc.Club.Status == "Active") // Thêm điều kiện Status
                .Select(uc => uc.Club) // Truy cập vào Club thông qua quan hệ navigation
                .OrderByDescending(c => c.CreateDate)
                .Select(c => new
                {
                    c.ClubId,
                    c.ClubName,
                    LevelName = c.Level.LevelName,
                    c.Status,
                    UserCount = _db.UserClubs.Count(uc => uc.ClubId == c.ClubId), // Đếm số thành viên trong Club
                    c.Avatar
                })
                .ToListAsync();

            // Kiểm tra và trả kết quả
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
                    Message = "Người dùng chưa tham gia Club nào với vai trò Player hoặc Club không còn hoạt động."
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
        [HttpPost("outClub")]
        public async Task<IActionResult> outClub([FromBody] RequestJoinClub requestJoinClub)
        {
            bool requestRemoved = false;
            bool userClubRemoved = false;

            // Kiểm tra và xóa yêu cầu tham gia nếu tồn tại
            var requestExist = await _db.RequestJoinClubs
                .Where(x => x.ClubId == requestJoinClub.ClubId && x.UserId == requestJoinClub.UserId)
                .FirstOrDefaultAsync();

            if (requestExist != null)
            {
                _db.RequestJoinClubs.Remove(requestExist);
                requestRemoved = true;
            }

            // Kiểm tra và xóa thành viên câu lạc bộ nếu tồn tại
            var userClub = await _db.UserClubs
                .Where(uc => uc.ClubId == requestJoinClub.ClubId && uc.UserId == requestJoinClub.UserId)
                .FirstOrDefaultAsync();

            if (userClub != null)
            {
                _db.UserClubs.Remove(userClub);
                userClubRemoved = true;
            }

            // Nếu có ít nhất một thao tác xóa được thực hiện
            if (requestRemoved || userClubRemoved)
            {
                await _db.SaveChangesAsync();

                var successMessage = "";
                if (requestRemoved && userClubRemoved)
                {
                    successMessage = "Đã xóa yêu cầu tham gia và rời khỏi câu lạc bộ thành công.";
                }
                else if (requestRemoved)
                {
                    successMessage = "Đã xóa yêu cầu tham gia thành công.";
                }
                else
                {
                    successMessage = "Đã rời khỏi câu lạc bộ thành công.";
                }

                var response = new
                {
                    Success = true,
                    Message = successMessage
                };
                return Ok(response);
            }
            else
            {
                var response = new
                {
                    Success = false,
                    Message = "Không tìm thấy yêu cầu tham gia hoặc thành viên trong câu lạc bộ."
                };
                return BadRequest(response);
            }
        }

        [HttpGet("getrequestJoinClub/{clubId}")]
        public async Task<IActionResult> getrequestJoinClub(int clubId)
        {
            if (requestJoinClub != null)
            {
                var listjoinclub = await _db.RequestJoinClubs
                    .Where(x => x.ClubId == clubId)
                    .Include(x => x.User)
                    .OrderByDescending(x => x.Status)
                    .Select(x => new
                    {
                        x.ClubId,
                        x.RequestClubId,
                        x.UserId,
                        x.Status,
                        UserFullName = x.User.FullName,
                        UserAvatar = x.User.Avatar

                    })
                    .ToListAsync();
                var ListViewRequest = new
                {
                    Success = true,
                    ListJoinClub = listjoinclub,

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
                    var clubId = requestjoin.ClubId;
                    var userId = requestjoin.UserId;
                    var club = await _db.UserClubs
                        .Where(a => a.ClubId == clubId && a.UserId == userId)
                        .FirstOrDefaultAsync();
                    if (club == null)
                    {
                        club = new UserClub
                        {
                            ClubId = clubId,
                            UserId = userId,
                            JoinDate = DateTime.Now,
                            Role = "Player"
                        };
                        await _db.UserClubs.AddAsync(club);
                    }
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
                    c.LevelId,
                    LevelName = c.Level.LevelName,
                    c.Status,
                    c.Avatar,
                    c.Rules,
                    c.SportId,
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
                    club.LevelId,
                    club.SportId,
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
                .Where(uc => uc.UserId == userId && uc.Role == "Leader")
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
                                   u.UserId,
                                   u.FullName,
                                   u.Avatar,
                                   uc.Role,
                                   uc.JoinDate
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
        private async Task<ImageUploadResult> UploadToCloudinary(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File không hợp lệ");

            var account = new Account(
                "di6k4wpxl",
                "791189184743261",
                "xQRBuHQLrCQokqwVU777RrIyLDQ");

            var cloudinary = new Cloudinary(account);

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream)
                    };

                    return await cloudinary.UploadAsync(uploadParams);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Lỗi khi upload ảnh lên Cloudinary", ex);
            }
        }

        [HttpPost("createClub")]
        public async Task<IActionResult> createClub([FromForm] CreateClubDAO clubs, [FromForm] IFormFile file)
        {
            if (clubs != null)
            {
                var level = await _db.Levels.Where(x => x.LevelId == clubs.SkillLevel).FirstOrDefaultAsync();
                var AddClub = new Club
                {
                    SportId = clubs.Sport,
                    LevelId = level?.LevelId,
                    ClubName = clubs.ClubName,
                    Status = "Active",
                    Rules = clubs.Description,
                    CreateDate = DateTime.Now,
                };
                if (file != null && file.Length > 0)
                {
                    try
                    {
                        var uploadResult = await UploadToCloudinary(file);

                        if (uploadResult != null)
                        {
                            AddClub.Avatar = uploadResult.SecureUrl.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, new
                        {
                            Success = false,
                            Message = "Lỗi khi upload ảnh",
                            Error = ex.Message
                        });
                    }
                }
                _db.Clubs.Add(AddClub);
                _db.SaveChanges();
                var clubId = AddClub.ClubId;
                var ClubUser = new UserClub
                {
                    UserId = clubs.userId,
                    ClubId = clubId,
                    JoinDate = DateTime.Now,
                    Role = "Leader"
                };
                _db.UserClubs.Add(ClubUser);
                _db.SaveChanges();
                var ListViewClub = new
                {
                    Success = true,
                    UserId = clubs.userId,
                    LevelId = AddClub.LevelId,
                    ClubName = AddClub.ClubName,
                    CreateDate = AddClub.CreateDate,
                    Status = AddClub.Status,
                    Rules = AddClub.Rules,
                    Fund = AddClub.Fund,
                    Avatar = AddClub.Avatar,
                    Message = "Đã thêm câu lạc bộ thành công"

                };
                return Ok(ListViewClub);
            }
            else
            {
                var ListViewClub = new
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi thêm kèo đấu..."
                };
                return BadRequest(ListViewClub);
            }

        }
        [HttpPost("updateClub/{clubId}")]
        public async Task<IActionResult> updateClub(int clubId, [FromForm] CreateClubDAO clubs, IFormFile file = null)
        {
            if (clubs != null && clubId > 0)
            {
                // Tìm câu lạc bộ cần cập nhật
                var existingClub = await _db.Clubs
                    .Where(x => x.ClubId == clubId)
                    .FirstOrDefaultAsync();

                if (existingClub == null)
                {
                    return NotFound(new { Success = false, Message = "Câu lạc bộ không tồn tại." });
                }

                // Cập nhật thông tin câu lạc bộ
                var level = await _db.Levels
                    .Where(x => x.LevelId == clubs.SkillLevel)
                    .FirstOrDefaultAsync();

                existingClub.SportId = clubs.Sport;
                existingClub.LevelId = clubs.SkillLevel;
                existingClub.ClubName = clubs.ClubName;
                existingClub.Rules = clubs.Description;

                // Kiểm tra nếu có file thì upload và cập nhật ảnh đại diện
                if (file != null && file.Length > 0)
                {
                    try
                    {
                        var uploadResult = await UploadToCloudinary(file);

                        if (uploadResult != null)
                        {
                            existingClub.Avatar = uploadResult.SecureUrl.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, new
                        {
                            Success = false,
                            Message = "Lỗi khi upload ảnh",
                            Error = ex.Message
                        });
                    }
                }

                // Lưu thay đổi
                _db.Clubs.Update(existingClub);
                await _db.SaveChangesAsync();

                // Trả về thông tin câu lạc bộ đã cập nhật
                var updatedClub = new
                {
                    Success = true,
                    UserId = clubs.userId,
                    LevelId = existingClub.LevelId,
                    ClubName = existingClub.ClubName,
                    Status = existingClub.Status,
                    Rules = existingClub.Rules,
                    Fund = existingClub.Fund,
                    Avatar = existingClub.Avatar,
                    Message = "Cập nhật câu lạc bộ thành công"
                };

                return Ok(updatedClub);
            }
            else
            {
                var errorResponse = new
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi cập nhật câu lạc bộ."
                };
                return BadRequest(errorResponse);
            }
        }
        [HttpPost("stopClub")]
        public async Task<IActionResult> stopClub([FromBody] StopClubRequest request)
        {
            if (request == null || request.ClubId <= 0)
            {
                return BadRequest(new { Success = false, Message = "Invalid Club ID" });
            }

            try
            {
                // Tìm câu lạc bộ theo ClubId
                var club = await _db.Clubs.FirstOrDefaultAsync(c => c.ClubId == request.ClubId);
                if (club == null)
                {
                    return NotFound(new { Success = false, Message = "Câu lạc bộ không tồn tại" });
                }

                // Cập nhật trạng thái câu lạc bộ thành "Inactive"
                club.Status = "Inactive"; // Hoặc trạng thái bạn muốn để dừng hoạt động

                // Lưu thay đổi vào database
                await _db.SaveChangesAsync();

                return Ok(new { Success = true, Message = "Câu lạc bộ đã dừng hoạt động thành công" });
            }
            catch (Exception ex)
            {
                // Log lỗi nếu có
                _logger.LogError(ex, "Lỗi khi dừng hoạt động câu lạc bộ");

                return StatusCode(500, new { Success = false, Message = "Đã xảy ra lỗi khi dừng hoạt động câu lạc bộ" });
            }
        }

        public class StopClubRequest
        {
            public int ClubId { get; set; }
        }
        [HttpPost("activateClub")]
        public async Task<IActionResult> activateClub([FromBody] StopClubRequest request)
        {
            if (request == null || request.ClubId <= 0)
            {
                return BadRequest(new { Success = false, Message = "Invalid Club ID" });
            }

            try
            {
                // Tìm câu lạc bộ theo ClubId
                var club = await _db.Clubs.FirstOrDefaultAsync(c => c.ClubId == request.ClubId);
                if (club == null)
                {
                    return NotFound(new { Success = false, Message = "Câu lạc bộ không tồn tại" });
                }

                // Cập nhật trạng thái câu lạc bộ thành "Inactive"
                club.Status = "Active"; // Hoặc trạng thái bạn muốn để dừng hoạt động

                // Lưu thay đổi vào database
                await _db.SaveChangesAsync();

                return Ok(new { Success = true, Message = "Câu lạc bộ đã dừng hoạt động thành công" });
            }
            catch (Exception ex)
            {
                // Log lỗi nếu có
                _logger.LogError(ex, "Lỗi khi dừng hoạt động câu lạc bộ");

                return StatusCode(500, new { Success = false, Message = "Đã xảy ra lỗi khi dừng hoạt động câu lạc bộ" });
            }
        }
        [HttpGet("getAllClubNames")]
        public async Task<IActionResult> getAllClubNames()
        {
            try
            {
                // Query the database to get all activity names
                List<string> ClubNames = await _db.Clubs
                    .Select(a => a.ClubName)
                    .ToListAsync();

                // Return the list of names as JSON
                return Ok(ClubNames);
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

    }
}
