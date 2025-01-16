using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using dsc_backend.DAO;
using dsc_backend.Helper;
using dsc_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq;
using static System.Reflection.Metadata.BlobBuilder;

namespace dsc_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ActivityController : Controller
    {
        private readonly ILogger<AuthenController> _logger;
        private readonly DscContext _db;
        private readonly Md5Helper _hashpass;
        private readonly CloudinarySettings _cloudinarySettings;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ActivityController(ILogger<AuthenController> logger, DscContext db, IOptions<CloudinarySettings> cloudinarySettings, IWebHostEnvironment webHostEnvironment, Md5Helper hashpass)
        {
            _logger = logger;
            _hashpass = hashpass;
            _db = db;
            _cloudinarySettings = cloudinarySettings.Value;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet("getAllActivity")]
        public async Task<IActionResult> GetAllActivity([FromQuery] int userId)
        {
            var activities = await _db.Activities
                .Where(a => a.UserId != userId &&
                            !_db.UserActivities.Any(ua => ua.UserId == userId && ua.ActivityId == a.ActivityId)) // Kiểm tra UserActivity
                .OrderByDescending(a => a.StartDate)
                .Select(a => new
                {
                    a.ActivityId,
                    a.ActivityName,
                    a.StartDate,
                    a.Location,
                    a.NumberOfTeams,
                    LevelName = a.Level.LevelName,
                    a.Avatar
                })
                .ToListAsync();

            if (!activities.Any())
            {
                return NotFound();
            }

            return Ok(activities);
        }
        [HttpGet("getAllActivityClub")]
        public async Task<IActionResult> getAllActivityClub([FromQuery] int clubId)
        {
            var activities = await _db.ActivitiesClubs
                .Where(a => a.ClubId == clubId)
                .OrderByDescending(a => a.StartDate)
                .Select(a => new
                {
                    a.ActivityClubId,
                    a.ActivityName,
                    a.StartDate,
                    a.Location,
                    a.NumberOfTeams,
                    a.ClubId,
                    LevelName = a.Level.LevelName,
                    NumberOfParticipants = _db.UserActivityClubs.Count(u => u.ActivityId == a.ActivityClubId && u.Role == "Player"),
                    a.Avatar
                })
                .ToListAsync();

            if (!activities.Any())
            {
                return NotFound();
            }

            return Ok(activities);
        }


        [HttpGet("getActivityJoined")]
        public async Task<IActionResult> getActivityJoined([FromQuery] int userId)
        {
            var activities = await _db.Activities
                .Include(a => a.UserActivities)  // Kết hợp bảng UserActivity
                .Where(x => x.UserActivities.Any(ua => ua.UserId == userId && ua.RoleInActivity == "Player"))  // Kiểm tra UserId và RoleInActivity
                .OrderByDescending(a => a.StartDate)
                .Select(a => new
                {
                    a.ActivityId,
                    a.ActivityName,
                    a.StartDate,
                    a.Location,
                    a.NumberOfTeams,
                    LevelName = a.Level.LevelName,
                    a.Avatar
                })
                .ToListAsync();

            if (!activities.Any())
            {
                return NotFound();
            }

            return Ok(activities);
        }

        [HttpGet("getMyActivity")]
        public async Task<IActionResult> GetMyActivity([FromQuery] int userId)
        {
            try
            {
                var activities = await _db.UserActivities
                    .Where(ua => ua.UserId == userId && ua.RoleInActivity == "Admin") // Thêm điều kiện RoleInActivity
                    .Include(ua => ua.Activity)
                        .ThenInclude(a => a.Level)
                    .Select(ua => new
                    {
                        ua.Activity.ActivityId,
                        ua.Activity.ActivityName,
                        ua.Activity.StartDate,
                        ua.Activity.Location,
                        ua.Activity.NumberOfTeams,
                        LevelName = ua.Activity.Level.LevelName,
                        Avatar = ua.Activity.Avatar
                    })
                    .ToListAsync();

                if (!activities.Any())
                {
                    return NotFound("Không tìm thấy hoạt động nào mà bạn là Admin.");
                }

                return Ok(activities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
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
        [HttpPost("createActivityClub")]
        public async Task<IActionResult> createActivity([FromForm] CreateActivityClubDAO activitys, [FromForm] IFormFile file)
        {
            if (activitys != null)
            {
                var level = await _db.Levels.Where(x => x.LevelName == activitys.minSkill).FirstOrDefaultAsync();
                var AddActivity = new ActivitiesClub
                {
                    ClubId = activitys.clubId,
                    LevelId = level?.LevelId,
                    Description = activitys.description,
                    ActivityName = activitys.name,
                    StartDate = DateTime.Parse(activitys?.datetime),
                    Location = activitys?.location,
                    NumberOfTeams = activitys?.playerCount,
                    Expense = activitys?.amount,
                };
                if (file != null && file.Length > 0)
                {
                    try
                    {
                        var uploadResult = await UploadToCloudinary(file);

                        if (uploadResult != null)
                        {
                            AddActivity.Avatar = uploadResult.SecureUrl.ToString();
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
                _db.ActivitiesClubs.AddAsync(AddActivity);
                _db.SaveChanges();
                var activityId = AddActivity.ActivityClubId;
                var ActivityUser = new UserActivityClub
                {
                    ClubId = activitys.clubId,
                    UserId = activitys.UserId,
                    ActivityId = activityId,
                    JoinDate = DateTime.Now,
                    Status = "Active",
                    Role = "Admin"
                };
                _db.UserActivityClubs.Add(ActivityUser);
                _db.SaveChanges();
                var ListViewActivity = new
                {
                    Success = true,
                    ClubId = AddActivity.ClubId,
                    LevelId = AddActivity.LevelId,
                    ActivityName = AddActivity.ActivityName,
                    StartDate = AddActivity.StartDate,
                    Location = AddActivity.Location,
                    NumberOfTeams = AddActivity.NumberOfTeams,
                    Expense = AddActivity.Expense,
                    Description = AddActivity.Description,
                    Message = "Đã thêm kèo đấu thành công"

                };
                return Ok(ListViewActivity);
            }
            else
            {
                var ListViewActivity = new
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi thêm kèo đấu..."
                };
                return BadRequest(ListViewActivity);
            }

        }
        [HttpPost("createActivity")]
        public async Task<IActionResult> createActivityClub([FromForm] CreateActivityDAO activitys, [FromForm] IFormFile file)
        {
            if (activitys != null)
            {
                var level = await _db.Levels.Where(x => x.LevelName == activitys.minSkill).FirstOrDefaultAsync();
                var AddActivity = new Activity
                {
                    UserId = activitys.userId,
                    LevelId = level?.LevelId,
                    Description = activitys.description,
                    ActivityName = activitys.name,
                    StartDate = DateTime.Parse(activitys?.datetime),
                    Location = activitys?.location,
                    NumberOfTeams = activitys?.playerCount,
                    Expense = activitys?.amount,
                };
                if (file != null && file.Length > 0)
                {
                    try
                    {
                        var uploadResult = await UploadToCloudinary(file);

                        if (uploadResult != null)
                        {
                            AddActivity.Avatar = uploadResult.SecureUrl.ToString();
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
                _db.Activities.Add(AddActivity);
                _db.SaveChanges();
                var activityId = AddActivity.ActivityId;
                var ActivityUser = new UserActivity
                {
                    UserId = activitys.userId,
                    ActivityId = activityId,
                    JoinDate = DateTime.Now,
                    RoleInActivity = "Admin"
                };
                _db.UserActivities.Add(ActivityUser);
                _db.SaveChanges();
                var ListViewActivity = new
                {
                    Success = true,
                    UserId = AddActivity.UserId,
                    LevelId = AddActivity.LevelId,
                    ActivityName = AddActivity.ActivityName,
                    StartDate = AddActivity.StartDate,
                    Location = AddActivity.Location,
                    NumberOfTeams = AddActivity.NumberOfTeams,
                    Expense = AddActivity.Expense,
                    Description = AddActivity.Description,
                    Message = "Đã thêm kèo đấu thành công"

                };
                return Ok(ListViewActivity);
            }
            else
            {
                var ListViewActivity = new
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi thêm kèo đấu..."
                };
                return BadRequest(ListViewActivity);
            }

        }
        [HttpPost("uppdateActivity")]
        public async Task<IActionResult> uppdateActivity([FromForm] CreateActivityDAO activitys, IFormFile file = null)
        {
            if (activitys != null)
            {
                var ActivityExist = await _db.Activities.FirstOrDefaultAsync(x => x.ActivityId == activitys.activityId && x.UserId == activitys.userId);
                var level = await _db.Levels.Where(x => x.LevelName == activitys.minSkill).FirstOrDefaultAsync();
                if (ActivityExist == null)
                {
                    return NotFound("kèo đấu không tồn tại.");
                }
                var StartDate = DateTime.Parse(activitys?.datetime);
                ActivityExist.ActivityName = activitys.name ?? ActivityExist.ActivityName;
                ActivityExist.LevelId = level?.LevelId ?? ActivityExist.LevelId;
                ActivityExist.StartDate = (DateTime?)StartDate ?? ActivityExist.StartDate;
                ActivityExist.Location = activitys.location ?? ActivityExist.Location;
                ActivityExist.NumberOfTeams = (int?)activitys.playerCount ?? ActivityExist.NumberOfTeams;
                ActivityExist.Expense = activitys.amount ?? ActivityExist.Expense;
                ActivityExist.Description = activitys.description ?? ActivityExist.Description;
                if (file != null && file.Length > 0)
                {
                    try
                    {
                        var uploadResult = await UploadToCloudinary(file);

                        if (uploadResult != null)
                        {
                            ActivityExist.Avatar = uploadResult.SecureUrl.ToString();
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
                _db.Activities.Update(ActivityExist);
                await _db.SaveChangesAsync();
                var updatedactivity = new
                {
                    Success = true,
                    UserId = ActivityExist.UserId,
                    LevelId = ActivityExist.LevelId,
                    ActivityName = ActivityExist.ActivityName,
                    StartDate = ActivityExist.StartDate,
                    Location = ActivityExist.Location,
                    NumberOfTeams = ActivityExist.NumberOfTeams,
                    Expense = ActivityExist.Expense,
                    Description = ActivityExist.Description,
                    Message = "Đã thêm kèo đấu thành công"
                };

                return Ok(updatedactivity);
            }
            else
            {
                var ListViewActivity = new
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi cập nhật kèo đấu..."
                };
                return BadRequest(ListViewActivity);
            }

        }
        [HttpPost("uppdateActivityClub")]
        public async Task<IActionResult> uppdateActivityClub([FromForm] CreateActivityDAO activitys, IFormFile file = null)
        {
            if (activitys != null)
            {
                var ActivityExist = await _db.ActivitiesClubs.FirstOrDefaultAsync(x => x.ActivityClubId == activitys.activityId);
                var level = await _db.Levels.Where(x => x.LevelName == activitys.minSkill).FirstOrDefaultAsync();
                if (ActivityExist == null)
                {
                    return NotFound("kèo đấu không tồn tại.");
                }
                var StartDate = DateTime.Parse(activitys?.datetime);
                ActivityExist.ActivityName = activitys.name ?? ActivityExist.ActivityName;
                ActivityExist.LevelId = level?.LevelId ?? ActivityExist.LevelId;
                ActivityExist.StartDate = (DateTime?)StartDate ?? ActivityExist.StartDate;
                ActivityExist.Location = activitys.location ?? ActivityExist.Location;
                ActivityExist.NumberOfTeams = (int?)activitys.playerCount ?? ActivityExist.NumberOfTeams;
                ActivityExist.Expense = activitys.amount ?? ActivityExist.Expense;
                ActivityExist.Description = activitys.description ?? ActivityExist.Description;
                if (file != null && file.Length > 0)
                {
                    try
                    {
                        var uploadResult = await UploadToCloudinary(file);

                        if (uploadResult != null)
                        {
                            ActivityExist.Avatar = uploadResult.SecureUrl.ToString();
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
                _db.ActivitiesClubs.Update(ActivityExist);
                await _db.SaveChangesAsync();
                var updatedactivity = new
                {
                    Success = true,
                    ActivityClubId = ActivityExist.ActivityClubId,
                    ClubId = ActivityExist.ClubId,
                    LevelId = ActivityExist.LevelId,
                    ActivityName = ActivityExist.ActivityName,
                    StartDate = ActivityExist.StartDate,
                    Location = ActivityExist.Location,
                    NumberOfTeams = ActivityExist.NumberOfTeams,
                    Expense = ActivityExist.Expense,
                    Description = ActivityExist.Description,
                    Message = "Đã thêm kèo đấu thành công"
                };

                return Ok(updatedactivity);
            }
            else
            {
                var ListViewActivity = new
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi cập nhật kèo đấu..."
                };
                return BadRequest(ListViewActivity);
            }

        }
        [HttpPost("requestJoinActivity")]
        public async Task<IActionResult> requestJoinActivity([FromBody] CreateActivityDAO requestJoinActivity)
        {
            var requestExist = await _db.RequestJoinActivities.Where(x => x.ActivitiesId == requestJoinActivity.activityId && x.UserId == requestJoinActivity.userId).FirstOrDefaultAsync();
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
                if (requestJoinActivity != null)
                {
                    var joinActivitynew = new RequestJoinActivity
                    {

                        UserId = requestJoinActivity.userId,
                        ActivitiesId = requestJoinActivity.activityId,
                        Status = "1",
                        CreateDate = DateTime.Now,
                    };
                    _db.RequestJoinActivities.Add(joinActivitynew);
                    _db.SaveChanges();
                    var ListViewRequest = new
                    {
                        Success = true,
                        Message = "Đã gởi đơn xin tham gia kèo đấu thành công"

                    };
                    return Ok(ListViewRequest);
                }
                else
                {
                    var ListViewRequest = new
                    {
                        Success = false,
                        Message = "Có lỗi trong việc gởi đơn tham gia kèo đấu"
                    };
                    return BadRequest(ListViewRequest);
                }
            }
        }
        [HttpPost("joinActivityClub")]
        public async Task<IActionResult> joinActivityClub([FromBody] CreateActivityClubDAO requestJoinActivity)
        {
            {
                if (requestJoinActivity != null)
                {
                    var joinActivitynew = new UserActivityClub
                    {

                        UserId = requestJoinActivity.UserId,
                        ActivityId = requestJoinActivity.activityclubId,
                        Status = "Active",
                        JoinDate = DateTime.Now,
                        ClubId = requestJoinActivity.clubId,
                        Role = "Player"
                    };
                    _db.UserActivityClubs.Add(joinActivitynew);
                    _db.SaveChanges();
                    var ListViewRequest = new
                    {
                        Success = true,
                        Message = "Đã gởi đơn xin tham gia kèo đấu thành công"

                    };
                    return Ok(ListViewRequest);
                }
                else
                {
                    var ListViewRequest = new
                    {
                        Success = false,
                        Message = "Có lỗi trong việc gởi đơn tham gia kèo đấu"
                    };
                    return BadRequest(ListViewRequest);
                }
            }
        }
        [HttpPost("getInforJoinned")]
        public async Task<IActionResult> getInforJoinned([FromBody] CreateActivityClubDAO requestJoinActivity)
        {
            if (requestJoinActivity == null)
            {
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ." });
            }

            // Kiểm tra xem dữ liệu đã tồn tại hay chưa
            var requestExist = await _db.UserActivityClubs
                .Where(x => x.ActivityId == requestJoinActivity.activityclubId
                         && x.UserId == requestJoinActivity.UserId
                         && x.ClubId == requestJoinActivity.clubId)
                .FirstOrDefaultAsync();

            // Nếu đã tồn tại thì trả về thông báo success: true
            if (requestExist != null)
            {
                return Ok(new { success = true, message = "Người dùng đã tham gia hoạt động này trước đó." });
            }

            // Nếu chưa tồn tại thì trả về success: false
            return Ok(new { success = false, message = "Người dùng chưa tham gia hoạt động này." });
        }


        [HttpGet("getrequestJoinActivity/{activityId}")]
        public async Task<IActionResult> GetRequestJoinActivity(int activityId)
        {
            if (activityId > 0) // Kiểm tra activityId có giá trị hợp lệ
            {
                var listJoinActivity = await _db.RequestJoinActivities
                    .Where(x => x.ActivitiesId == activityId)
                    .Include(x => x.User) // Bao gồm thông tin từ bảng User
                    .OrderByDescending(x => x.Status)
                    .Select(x => new
                    {
                        x.RequestJoinActivityId, // Giả định bạn có trường RequestId
                        x.ActivitiesId,
                        x.CreateDate,
                        x.Status,
                        x.UserId,
                        UserFullName = x.User.FullName, // Lấy FullName từ bảng User
                        Avatar = x.User.Avatar
                    })
                    .ToListAsync();

                var listViewRequest = new
                {
                    Success = true,
                    ListJoinActivity = listJoinActivity,
                };

                return Ok(listViewRequest);
            }
            else
            {
                var listViewRequest = new
                {
                    Success = false,
                    Message = "Có lỗi trong việc hiển thị đơn tham gia kèo đấu"
                };
                return BadRequest(listViewRequest);
            }
        }

        [HttpPost("acceptrequestJoinActivity")]
        public async Task<IActionResult> acceptrequestJoinActivity([FromBody] RequestJoinActivity requestJoinActivites)
        {
            if (requestJoinActivites != null)
            {
                var requestjoin = await _db.RequestJoinActivities
                    .Where(x => x.RequestJoinActivityId == requestJoinActivites.RequestJoinActivityId &&
                                x.UserId == requestJoinActivites.UserId) // Kiểm tra UserId
                    .FirstOrDefaultAsync();

                if (requestjoin != null)
                {
                    // Cập nhật Status
                    requestjoin.Status = "2";

                    // Lưu thay đổi vào cơ sở dữ liệu
                    await _db.SaveChangesAsync();

                    // Tạo một đối tượng Activity mới hoặc cập nhật nếu đã tồn tại
                    var activityId = requestjoin.ActivitiesId; // Lấy ActivityId từ requestjoin
                    var userId = requestjoin.UserId; // Lấy UserId từ requestjoin

                    // Kiểm tra xem UserActivities đã tồn tại chưa
                    var activity = await _db.UserActivities
                        .Where(a => a.ActivityId == activityId && a.UserId == userId)
                        .FirstOrDefaultAsync();

                    if (activity == null)
                    {
                        activity = new UserActivity
                        {
                            ActivityId = activityId,
                            UserId = userId,
                            JoinDate = DateTime.UtcNow,
                            RoleInActivity = "Player"

                        };

                        await _db.UserActivities.AddAsync(activity);
                    }
                    // Lưu thay đổi vào cơ sở dữ liệu
                    await _db.SaveChangesAsync();

                    var ListViewRequest = new
                    {
                        Success = true,
                        RequestJoin = requestjoin, // Có thể trả về requestjoin nếu cần
                        Message = "Đã xác nhận đơn tham gia kèo đấu thành công"
                    };

                    return Ok(ListViewRequest);
                }
                else
                {
                    return BadRequest(new { Success = false, Message = "Không tìm thấy yêu cầu tham gia hoặc UserId không khớp." });
                }
            }
            else
            {
                return BadRequest(new { Success = false, Message = "Có lỗi trong việc xác nhận đơn tham gia kèo đấu." });
            }
        }

        [HttpPost("cancelrequestJoinActivity")]
        public async Task<IActionResult> cancelrequestJoinActivity([FromBody] RequestJoinActivity requestJoinActivites)
        {
            if (requestJoinActivites != null)
            {
                var requestjoin = await _db.RequestJoinActivities.Where(x => x.RequestJoinActivityId == requestJoinActivites.RequestJoinActivityId && x.UserId == requestJoinActivites.UserId)
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
                        Message = "Đã xác nhận đơn tham gia kèo đấu thành công"
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
                    Message = "Có lỗi trong việc xác nhận đơn tham gia kèo đấu"
                };
                return BadRequest(ListViewRequest);
            }
        }
        [HttpGet("getActivityDetails/{activityId}")]
        public async Task<IActionResult> getActivityDetails(int activityId)
        {
            var activitys = await _db.Activities
                .Select(a => new
                {
                    a.ActivityId,
                    a.ActivityName,
                    a.StartDate,
                    a.Location,
                    a.NumberOfTeams,
                    a.Description,
                    a.Expense,
                    // Thêm các thuộc tính khác của Activity mà bạn muốn lấy
                    LevelName = a.Level.LevelName,
                    a.Avatar
                })
                .Where(x => x.ActivityId == activityId)
                .ToListAsync();

            if (!activitys.Any())
            {
                return NotFound();
            }

            return Ok(activitys);
        }
        [HttpGet("getActivityDetailsClub/{activityclubId}")]
        public async Task<IActionResult> getActivityDetailsClub(int activityclubId)
        {
            var activitys = await _db.ActivitiesClubs
                .Select(a => new
                {
                    a.ActivityClubId,
                    a.ActivityName,
                    a.ClubId,
                    a.StartDate,
                    a.Location,
                    a.NumberOfTeams,
                    a.Description,
                    a.Expense,

                    NumberOfParticipants = _db.UserActivityClubs.Count(u => u.ActivityId == a.ActivityClubId && u.Role == "Player"),
                    // Thêm các thuộc tính khác của Activity mà bạn muốn lấy
                    LevelName = a.Level.LevelName,
                    a.Avatar
                })
                .Where(x => x.ActivityClubId == activityclubId)
                .ToListAsync();

            if (!activitys.Any())
            {
                return NotFound();
            }

            return Ok(activitys);
        }
        [HttpGet("getMemberActivity/{activityId}")]
        public async Task<IActionResult> GetMemberActivity(int activityId)
        {
            try
            {
                var userActivities = await _db.UserActivities
                    .Where(ua => ua.ActivityId == activityId)
                    .Include(ua => ua.User)
                    .Include(ua => ua.Activity)
                        .ThenInclude(a => a.Level) // Include thêm bảng Level
                    .ToListAsync();

                if (!userActivities.Any())
                {
                    return NotFound("Không tìm thấy thông tin hoạt động hoặc thành viên.");
                }

                var activity = userActivities.First().Activity;
                if (activity == null)
                {
                    return NotFound("Không tìm thấy thông tin hoạt động.");
                }

                var activityInfo = new
                {
                    ActivityId = activity.ActivityId,
                    ActivityName = activity.ActivityName,
                    StartDate = activity.StartDate,
                    Location = activity.Location,
                    NumberOfTeams = activity.NumberOfTeams,
                    Description = activity.Description,
                    Expense = activity.Expense,
                    LevelName = activity.Level?.LevelName,
                    Avatar = activity.Avatar
                };

                var memberInfo = userActivities.Select(ua => new
                {
                    UserId = ua.UserId,
                    AvatarUser = ua.User?.Avatar,
                    FullName = ua.User?.FullName,
                    RoleActivity = ua.RoleInActivity,
                }).ToList();
                var playerCount = userActivities.Count(ua => ua.RoleInActivity == "Player");
                var result = new
                {
                    Activity = activityInfo,
                    MemberInfo = memberInfo,
                    playerCount = playerCount
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("getMemberActivityClub/{activityclubId}")]
        public async Task<IActionResult> GetMemberActivityClub(int activityclubId)
        {
            try
            {
                var userActivities = await _db.UserActivityClubs
                    .Where(ua => ua.ActivityId == activityclubId)
                    .Include(ua => ua.User)
                    .Include(ua => ua.Activity)
                        .ThenInclude(a => a.Level) // Include thêm bảng Level
                    .ToListAsync();

                if (!userActivities.Any())
                {
                    return NotFound("Không tìm thấy thông tin hoạt động hoặc thành viên.");
                }

                var activity = userActivities.First().Activity;
                if (activity == null)
                {
                    return NotFound("Không tìm thấy thông tin hoạt động.");
                }

                var activityInfo = new
                {
                    ActivityId = activity.ActivityClubId,
                    ActivityName = activity.ActivityName,
                    StartDate = activity.StartDate,
                    Location = activity.Location,
                    NumberOfTeams = activity.NumberOfTeams,
                    Description = activity.Description,
                    Expense = activity.Expense,
                    LevelName = activity.Level?.LevelName,
                    Avatar = activity.Avatar,
                    ClubId = activity.ClubId
                };

                var memberInfo = userActivities.Select(ua => new
                {
                    UserId = ua.UserId,
                    AvatarUser = ua.User?.Avatar,
                    FullName = ua.User?.FullName,
                    RoleActivity = ua.Role
                }).ToList();

                var playerCount = userActivities.Count(ua => ua.Role == "Player");

                var result = new
                {
                    Activity = activityInfo,
                    MemberInfo = memberInfo,
                    PlayerCount = playerCount
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("getNameActivity")]
        public async Task<IActionResult> getNameActivity()
        {
            try
            {
                // Query the database to get all activity names
                List<string> activityNames = await _db.Activities
                    .Select(a => a.ActivityName)
                    .ToListAsync();

                // Return the list of names as JSON
                return Ok(activityNames);
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return StatusCode(500, "Internal server error: " + ex.Message);
            }

        }
        [HttpPost("CreateResults")]
        public async Task<IActionResult> CreateResults([FromBody] ResultOfActivity result)
        {
            if (result == null)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            var newResult = new ResultOfActivity
            {
                ActivityId = result.ActivityId,
                Team1Score = result.Team1Score,
                Team2Score = result.Team2Score
            };

            _db.ResultOfActivities.Add(newResult); // Thêm vào DbContext
            await _db.SaveChangesAsync(); // Lưu thay đổi vào DB

            return Ok(new
            {
                Message = "Thêm kết quả hoạt động thành công.",
                Data = newResult
            });
        }
        [HttpGet("GetResults/{activityId}")]
        public async Task<IActionResult> GetResults(int activityId)
        {
            var result = await _db.ResultOfActivities
                                       .FirstOrDefaultAsync(r => r.ActivityId == activityId);

            if (result == null)
            {
                // Trả về một đối tượng mặc định nếu chưa có kết quả
                return Ok(new
                {
                    ResultId = 0,
                    ActivityId = activityId,
                    Team1Score = 0,
                    Team2Score = 0
                });
            }

            return Ok(result);
        }
        [HttpGet("GetComments/{activityId}")]
        public async Task<IActionResult> GetComments(int activityId)
        {
            var comments = await _db.Comments
                                    .Where(c => c.ActivityId == activityId)
                                    .Include(c => c.User) // Join với bảng User để lấy FullName
                                    .Select(c => new
                                    {
                                        CommentID = c.CommentId,
                                        FullName = c.User.FullName, // Lấy FullName từ bảng User
                                        CommentText = c.Comment1 // Lấy Text bình luận
                                    })
                                    .ToListAsync();

            if (comments == null || !comments.Any())
            {
                return Ok(new { message = "Không có bình luận nào cho hoạt động này." });
            }

            return Ok(comments); // Trả về danh sách các bình luận với FullName, CommentID và CommentText
        }

        [HttpPut("UpdateResults")]
        public async Task<IActionResult> UpdateResult([FromBody] ResultOfActivity updatedResult)
        {
            if (updatedResult == null)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            if (updatedResult.ResultId == 0)
            {
                // Tạo mới kết quả nếu ResultId là 0
                var newResult = new ResultOfActivity
                {
                    ActivityId = updatedResult.ActivityId,
                    Team1Score = updatedResult.Team1Score,
                    Team2Score = updatedResult.Team2Score
                };

                _db.ResultOfActivities.Add(newResult);
                await _db.SaveChangesAsync();

                return Ok(new { Message = "Tạo kết quả thành công!", Data = newResult });
            }

            // Cập nhật kết quả hiện có
            var existingResult = await _db.ResultOfActivities.FindAsync(updatedResult.ResultId);
            if (existingResult == null)
            {
                return NotFound("Kết quả không tồn tại.");
            }

            existingResult.Team1Score = updatedResult.Team1Score;
            existingResult.Team2Score = updatedResult.Team2Score;

            _db.ResultOfActivities.Update(existingResult);
            await _db.SaveChangesAsync();

            return Ok(new { Message = "Cập nhật kết quả thành công!", Data = existingResult });
        }
        [HttpPut("AddComment")]
        public async Task<IActionResult> AddComment([FromBody] Comment comment)
        {
            if (comment == null)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            // Lưu bình luận vào cơ sở dữ liệu
            _db.Comments.Add(comment);
            await _db.SaveChangesAsync();

            // Lấy thông tin người dùng từ bảng User
            var user = await _db.Users
                                 .Where(u => u.UserId == comment.UserId)
                                 .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }

            // Trả về thông tin bình luận cùng với thông tin người dùng
            return Ok(new
            {
                commentID = comment.CommentId,
                fullName = user.FullName,      // Lấy FullName từ bảng User
                commentText = comment.Comment1
            });
        }


        [HttpDelete("DeleteComment/{commentId}")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var comment = await _db.Comments.FindAsync(commentId);
            if (comment == null)
            {
                return NotFound(new { message = "Bình luận không tồn tại" });
            }

            _db.Comments.Remove(comment);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Bình luận đã được xóa thành công!" });
        }


        // Comment in Activity Club

        [HttpGet("GetResultsClub/{activityClubId}")]
        public async Task<IActionResult> GetResultsClub(int activityClubId)
        {
            var result = await _db.ResultOfActivitiesClubs
                                       .FirstOrDefaultAsync(r => r.ActivityClubId == activityClubId);

            if (result == null)
            {
                // Trả về một đối tượng mặc định nếu chưa có kết quả
                return Ok(new
                {
                    ResultId = 0,
                    ActivityClubId = activityClubId,
                    Team1Score = 0,
                    Team2Score = 0
                });
            }

            return Ok(result);
        }
        [HttpGet("GetCommentsClub/{activityClubId}")]
        public async Task<IActionResult> GetCommentsClub(int activityClubId)
        {
            var comments = await _db.CommentClubs
                                    .Where(c => c.ActivityClubId == activityClubId)
                                    .Include(c => c.User) // Join với bảng User để lấy FullName
                                    .Select(c => new
                                    {
                                        CommentID = c.CommentClubId,
                                        FullName = c.User.FullName, // Lấy FullName từ bảng User
                                        CommentText = c.Comment // Lấy Text bình luận
                                    })
                                    .ToListAsync();

            if (comments == null || !comments.Any())
            {
                return Ok(new { message = "Không có bình luận nào cho hoạt động này." });
            }

            return Ok(comments); // Trả về danh sách các bình luận với FullName, CommentID và CommentText
        }
        [HttpPut("AddCommentClub")]
        public async Task<IActionResult> AddCommentClub([FromBody] CommentClub comment)
        {
            if (comment == null)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            // Lưu bình luận vào cơ sở dữ liệu
            _db.CommentClubs.Add(comment);
            await _db.SaveChangesAsync();

            // Lấy thông tin người dùng từ bảng User
            var user = await _db.Users
                                 .Where(u => u.UserId == comment.UserId)
                                 .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }

            // Trả về thông tin bình luận cùng với thông tin người dùng
            return Ok(new
            {
                commentID = comment.CommentClubId,
                fullName = user.FullName,      // Lấy FullName từ bảng User
                commentText = comment.Comment
            });
        }


        [HttpDelete("DeleteCommentClub/{commentClubId}")]
        public async Task<IActionResult> DeleteCommentClub(int commentClubId)
        {
            var comment = await _db.CommentClubs.FindAsync(commentClubId);
            if (comment == null)
            {
                return NotFound(new { message = "Bình luận không tồn tại" });
            }

            _db.CommentClubs.Remove(comment);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Bình luận đã được xóa thành công!" });
        }
        [HttpPut("UpdateResultsClub")]
        public async Task<IActionResult> UpdateResultClub([FromBody] ResultOfActivitiesClub updatedResult)
        {
            if (updatedResult == null)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            if (updatedResult.ResultId == 0)
            {
                // Tạo mới kết quả nếu ResultId là 0
                var newResult = new ResultOfActivitiesClub
                {
                    ActivityClubId = updatedResult.ActivityClubId,
                    Team1Score = updatedResult.Team1Score,
                    Team2Score = updatedResult.Team2Score
                };

                _db.ResultOfActivitiesClubs.Add(newResult);
                await _db.SaveChangesAsync();

                return Ok(new { Message = "Tạo kết quả thành công!", Data = newResult });
            }

            // Cập nhật kết quả hiện có
            var existingResult = await _db.ResultOfActivitiesClubs.FindAsync(updatedResult.ResultId);
            if (existingResult == null)
            {
                return NotFound("Kết quả không tồn tại.");
            }

            existingResult.Team1Score = updatedResult.Team1Score;
            existingResult.Team2Score = updatedResult.Team2Score;

            _db.ResultOfActivitiesClubs.Update(existingResult);
            await _db.SaveChangesAsync();

            return Ok(new { Message = "Cập nhật kết quả thành công!", Data = existingResult });
        }

    }
}