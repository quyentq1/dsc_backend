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
                    LevelName = a.Level.LevelName
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
                    LevelName = a.Level.LevelName
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
            var activities = await _db.UserActivities
                .Where(ua => ua.UserId == userId)
                .Include(ua => ua.Activity)                // Bao gồm bảng Activity
                    .ThenInclude(a => a.Level)             // Bao gồm bảng Level từ Activity
                .Select(ua => new
                {
                    ua.Activity.ActivityId,               // Lấy ActivityId từ bảng Activity
                    ua.Activity.ActivityName,             // Lấy tên của Activity
                    ua.Activity.StartDate,                // Lấy StartDate từ Activity
                    ua.Activity.Location,                 // Lấy Location từ Activity
                    ua.Activity.NumberOfTeams,            // Lấy NumberOfTeams từ Activity
                    LevelName = ua.Activity.Level.LevelName // Lấy tên của Level từ Activity
                })
                .ToListAsync();

            if (!activities.Any())
            {
                return NotFound("Không tìm thấy hoạt động nào cho người dùng này.");
            }

            return Ok(activities);
        }

        [HttpPost("createActivity")]
        public async Task<IActionResult> createActivity([FromBody] CreateActivityDAO activitys)
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
        public async Task<IActionResult> uppdateActivity([FromBody] CreateActivityDAO activitys)
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
        [HttpPost("requestJoinActivity")]
        public async Task<IActionResult> requestJoinActivity([FromBody] CreateActivityDAO requestJoinActivity)
        {
            var requestExist = await _db.RequestJoinActivities.Where(x => x.ActivitiesId == requestJoinActivity.activityId && x.UserId == requestJoinActivity.userId).FirstOrDefaultAsync();
            if(requestExist != null)
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
                        UserFullName = x.User.FullName // Lấy FullName từ bảng User
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
                            JoinDate= DateTime.UtcNow,
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
                    LevelName = a.Level.LevelName
                })
                .Where(x => x.ActivityId == activityId)
                .ToListAsync();

            if (!activitys.Any())
            {
                return NotFound();
            }

            return Ok(activitys);
        }
        [HttpGet("getMemberActivity/{activityId}")]
        public async Task<IActionResult> getMemberActivity(int activityId)
        {
            // Lấy danh sách UserActivities theo ActivityId
            var userActivities = await _db.UserActivities
                .Where(ua => ua.ActivityId == activityId)
                .Include(ua => ua.User) // Kết nối với bảng User
                .Include(ua => ua.Activity) // Kết nối với bảng Activity
                .ToListAsync();

            if (!userActivities.Any())
            {
                return NotFound("Không tìm thấy thông tin hoạt động hoặc thành viên.");
            }

            var firstActivity = userActivities.First().Activity;
            if (firstActivity == null)
            {
                return NotFound("Không tìm thấy thông tin hoạt động.");
            }

            var activity = new
            {
                ActivityId = firstActivity.ActivityId,
                ActivityName = firstActivity.ActivityName,
                StartDate = firstActivity.StartDate,
                Location = firstActivity.Location,
                NumberOfTeams = firstActivity.NumberOfTeams,
                Description = firstActivity.Description,
                Expense = firstActivity.Expense,
                LevelName = firstActivity.Level?.LevelName // Kiểm tra null cho Level
            };
            var userIds = userActivities.Select(ua => ua.UserId).ToList();
            var memberInfo = await _db.UserSports
                .Where(us => userIds.Contains(us.UserId))
                .Include(us => us.Level) // Kết nối với Level
                .ToListAsync(); // Chuyển sang danh sách

            // Tạo danh sách memberInfo với RoleActivity
            var resultMemberInfo = memberInfo.Select(us => new
            {
                UserId = us.UserId,
                FullName = us.User.FullName,
                RoleActivity = userActivities.FirstOrDefault(ua => ua.UserId == us.UserId)?.RoleInActivity,
                LevelName = us.Level.LevelName
            }).ToList();

            var result = new
            {
                Activity = activity,
                MemberInfo = resultMemberInfo
            };

            return Ok(result);
        }
    }
}
