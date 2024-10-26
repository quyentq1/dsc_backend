using dsc_backend.Helper;
using dsc_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
        public async Task<IActionResult> getAllActivity()
        {
            var activitys = await _db.Activities
                .Include(t => t.Level) // Thông tin về Level
                .Include(t => t.Comments) // Thông tin về Comments
                .Include(t => t.Notifications) // Thông tin về Notifications
                .Include(t => t.ResultOfActivities) // Thông tin về ResultOfActivities
                .Include(t => t.UserActivities) // Thông tin về UserActivities
                .Include(t => t.User) // Thông tin về User
                .ToListAsync();

            if (activitys == null)
            {
                return NotFound();
            }

            return Ok(activitys);
        }
        [HttpPost("createActivity")]
        public async Task<IActionResult> createActivity([FromBody] Activity activitys)
        {
            if (activitys == null)
            {
                var AddActivity = new Activity
                {
                    UserId = activitys.UserId,
                    LevelId = activitys.LevelId,
                    ActivityName = activitys.ActivityName,
                    StartDate = activitys.StartDate,
                    Location = activitys.Location,
                    NumberOfTeams = activitys.NumberOfTeams,
                    Expense = activitys.Expense,
                    Description = activitys.Description,
                };
                _db.Activities.Add(AddActivity);
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
        public async Task<IActionResult> uppdateActivity([FromBody] Activity activitys)
        {
            if (activitys == null)
            {
                var ActivityExist = await _db.Activities.FirstOrDefaultAsync(x => x.ActivityId == activitys.ActivityId);

                if (ActivityExist == null)
                {
                    return NotFound("kèo đấu không tồn tại.");
                }
                ActivityExist.ActivityName = activitys.ActivityName ?? ActivityExist.ActivityName;
                ActivityExist.LevelId = activitys.LevelId ?? ActivityExist.LevelId;
                ActivityExist.StartDate = activitys.StartDate ?? ActivityExist.StartDate;
                ActivityExist.Location = activitys.Location ?? ActivityExist.Location;
                ActivityExist.NumberOfTeams = activitys.NumberOfTeams ?? ActivityExist.NumberOfTeams;
                ActivityExist.Expense = activitys.Expense ?? ActivityExist.Expense;
                ActivityExist.Description = activitys.Description ?? ActivityExist.Description;
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
        public async Task<IActionResult> requestJoinActivity([FromBody] RequestJoinActivity requestJoinActivity)
        {
            if (requestJoinActivity != null)
            {
                var joinActivitynew = new RequestJoinActivity
                {

                    UserId = requestJoinActivity.UserId,
                    ClubId = requestJoinActivity.ClubId,
                    ActivitiesId = requestJoinActivity.ActivitiesId,
                    Status = "1",
                    CreateDate = DateTime.Now,
                };
                _db.RequestJoinActivities.Add(joinActivitynew);
                _db.SaveChanges();
                var ListViewRequest = new
                {
                    Success = true,
                    UserId = requestJoinActivity.UserId,
                    ClubId = requestJoinActivity.ClubId,
                    ActivitiesId = requestJoinActivity.ActivitiesId,
                    Status = "1",
                    Createdate = DateTime.Now,
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
        [HttpPost("getrequestJoinActivity")]
        public async Task<IActionResult> getrequestJoinActivity([FromBody] RequestJoinActivity requestJoinActivites)
        {
            if (requestJoinActivites != null)
            {
                var listjoinactivity = await _db.RequestJoinActivities.Where(x => x.ActivitiesId == requestJoinActivites.ActivitiesId)
                                       .OrderByDescending(x => x.Status)
                                       .ToListAsync();
                var ListViewRequest = new
                {
                    Success = true,
                    listjoinactivity,

                };
                return Ok(ListViewRequest);
            }
            else
            {
                var ListViewRequest = new
                {
                    Success = false,
                    Message = "Có lỗi trong việc hiển thị đơn tham gia kèo đấu"
                };
                return BadRequest(ListViewRequest);
            }
        }
        [HttpPost("acceptrequestJoinActivity")]
        public async Task<IActionResult> acceptrequestJoinClub([FromBody] RequestJoinActivity requestJoinActivites)
        {
            if (requestJoinActivites != null)
            {
                var requestjoin = await _db.RequestJoinActivities.Where(x => x.RequestJoinActivityId == requestJoinActivites.RequestJoinActivityId)
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
        [HttpPost("cancelrequestJoinActivity")]
        public async Task<IActionResult> cancelrequestJoinActivity([FromBody] RequestJoinActivity requestJoinActivites)
        {
            if (requestJoinActivites != null)
            {
                var requestjoin = await _db.RequestJoinActivities.Where(x => x.RequestJoinActivityId == requestJoinActivites.RequestJoinActivityId)
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
        [HttpGet("getActivityDetails")]
        public async Task<IActionResult> getActivityDetails([FromBody] Activity activities)
        {
            var activitys = await _db.Activities
                .Include(t => t.Level) // Thông tin về Level
                .Include(t => t.Comments) // Thông tin về Comments
                .Include(t => t.Notifications) // Thông tin về Notifications
                .Include(t => t.ResultOfActivities) // Thông tin về ResultOfActivities
                .Include(t => t.UserActivities) // Thông tin về UserActivities
            .Include(t => t.User) // Thông tin về User
                .FirstOrDefaultAsync(c => c.ActivityId == activities.ActivityId);

            if (activitys == null)
            {
                return NotFound();
            }

            return Ok(activitys);
        }
    }
}
