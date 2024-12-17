using dsc_backend.Helper;
using dsc_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;
using static dsc_backend.Controllers.ClubController;

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
        [HttpPost("updateinforcustomer")]
        public async Task<IActionResult> UpdateInforcustomer([FromBody] User user)
        {
            var existingUser = await _db.Users.FirstOrDefaultAsync(x => x.UserId == user.UserId);

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
            existingUser.Height = user.Height ?? existingUser.Height;
            existingUser.Status = user.Status ?? existingUser.Status;
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
        [HttpGet("searchCustomer")]
        public async Task<IActionResult> SearchCustomer(string q)
        {
            if (string.IsNullOrEmpty(q))
            {
                return BadRequest(new { Success = false, Message = "Search query is required." });
            }

            // Search customers by full name (case insensitive)
            var customers = await _db.Users
                .Where(c => EF.Functions.Like(c.FullName, $"%{q}%")) // Use EF.Functions.Like for case-insensitive search
                .ToListAsync();

            if (customers == null || customers.Count == 0)
            {
                return NotFound(new { Success = false, Message = "No customers found." });
            }

            return Ok(new { Success = true, Data = customers });
        }
        [HttpGet("searchClub")]
        public async Task<IActionResult> searchClub(string q)
        {
            if (string.IsNullOrEmpty(q))
            {
                return BadRequest(new { Success = false, Message = "Search query is required." });
            }

            // Search customers by full name (case insensitive)
            var club = await _db.Clubs
                .Where(c => EF.Functions.Like(c.ClubName, $"%{q}%")) // Use EF.Functions.Like for case-insensitive search
                .ToListAsync();

            if (club == null || club.Count == 0)
            {
                return NotFound(new { Success = false, Message = "No club found." });
            }

            return Ok(new { Success = true, Data = club });
        }
        [HttpGet("searchTournament")]
        public async Task<IActionResult> searchTournament(string q)
        {
            if (string.IsNullOrEmpty(q))
            {
                return BadRequest(new { Success = false, Message = "Search query is required." });
            }

            // Search customers by full name (case insensitive)
            var tournaments = await _db.Tournaments
                .Where(c => EF.Functions.Like(c.Name, $"%{q}%")) // Use EF.Functions.Like for case-insensitive search
                .ToListAsync();

            if (tournaments == null || tournaments.Count == 0)
            {
                return NotFound(new { Success = false, Message = "No tournaments found." });
            }

            return Ok(new { Success = true, Data = tournaments });
        }
        [HttpGet("getClubList")]
        public async Task<IActionResult> getClubList()
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
                    .OrderByDescending(a => a.CreateDate)
                    .Select(a => new
                    {
                        a.ClubId,
                        a.ClubName,
                        a.Rules,
                        LevelName = a.Level.LevelName,
                        a.Status,
                        UserCount = userCounts.ContainsKey(a.ClubId) ? userCounts[a.ClubId] : 0,
                        a.Avatar,
                        a.CreateDate
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
        [HttpGet("getClubById/{clubId}")]
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
        [HttpGet("GetAllTournament")]
        public async Task<IActionResult> GetAllTournament()
        {

            var tournaments = await _db.Tournaments
    .Select(a => new
    {
        a.TournamentId,
        a.Name,
        a.NumberOfTeams,
        a.Location,
        a.StartDate,
        a.EndDate,
        LevelName = a.Level.LevelName,
        a.Avatar
    })
    .ToListAsync();


            // Kiểm tra danh sách rỗng
            if (!tournaments.Any())
            {
                return Ok("No tournaments found for the given user.");
            }

            return Ok(tournaments);
        }
        [HttpGet("getListTeam/{tournamentId}")]
        public async Task<IActionResult> GetListTeam(int tournamentId)
        {
            var tournaments = await _db.Teams
                .Where(t => t.TournamentId == tournamentId) // Lọc các đội theo TournamentId
                .Select(t => new
                {
                    t.TeamId,
                    t.TournamentId,
                    t.TeamName,
                    t.Avatar,
                    MemberCount = t.TeamTournaments.Count() // Đếm số lượng thành viên liên kết với TeamId
                })
                .ToListAsync();
            return Ok(tournaments);
        }
        [HttpPost("getListMember/{teamId}")]
        public async Task<IActionResult> GetListMember(int teamId)
        {
            // Lấy danh sách thành viên theo TeamId
            var members = await _db.TeamTournaments
                .Where(tt => tt.TeamId == teamId) // Lọc theo TeamId
                .Select(tt => new
                {
                    tt.NamePlayer, // Tên cầu thủ
                    tt.NumberPlayer // Số áo cầu thủ
                })
                .ToListAsync();
            // Nếu không tìm thấy thành viên hoặc thông tin đội, trả về lỗi
            if (!members.Any())
            {
                return NotFound(new { Message = "No members or team information found for the given IDs." });
            }

            return Ok(members);
        }
        [HttpPost("deleteTournament/{tournamentId}")]
        public async Task<IActionResult> deleteTournament(int tournamentId)
        {
            // Tìm giải đấu với các đối tượng liên quan
            var tournament = await _db.Tournaments
                                       .Include(t => t.Teams)           // Bao gồm các đội (Teams) liên kết với Tournament
                                       .Include(t => t.Matches)         // Bao gồm các trận đấu (Matches)
                                       .ThenInclude(m => m.Results)    // Bao gồm kết quả của các trận đấu
                                       .Include(t => t.Rounds)          // Bao gồm các vòng đấu (Rounds)
                                       .FirstOrDefaultAsync(t => t.TournamentId == tournamentId);

            if (tournament == null)
            {
                return NotFound(new { message = "Giải đấu không tồn tại." });
            }

            // 1. Xóa các bản ghi trong bảng TeamTournament liên quan đến các đội trong giải đấu
            var teamIds = tournament.Teams.Select(team => team.TeamId).ToList(); // Lấy danh sách TeamId liên quan
            var teamTournaments = await _db.TeamTournaments
                                            .Where(tt => teamIds.Contains(tt.TeamId)) // Tìm tất cả bản ghi trong TeamTournament liên quan đến các đội này
                                            .ToListAsync();

            _db.TeamTournaments.RemoveRange(teamTournaments); // Xóa tất cả các bản ghi trong TeamTournament

            // 2. Xóa các kết quả (Results) liên quan đến các trận đấu (Matches)
            foreach (var match in tournament.Matches)
            {
                if (match.Results != null && match.Results.Any())
                {
                    _db.Results.RemoveRange(match.Results); // Xóa tất cả kết quả của trận đấu
                }
            }

            // 3. Xóa các trận đấu (Matches) liên quan
            _db.Matches.RemoveRange(tournament.Matches);

            // 4. Xóa các vòng đấu (Rounds) liên quan
            _db.Rounds.RemoveRange(tournament.Rounds);

            // 5. Xóa các đội (Teams) liên quan
            _db.Teams.RemoveRange(tournament.Teams);

            // 6. Cuối cùng xóa giải đấu
            _db.Tournaments.Remove(tournament);

            // Lưu thay đổi vào cơ sở dữ liệu
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TournamentExists(tournamentId))
                {
                    return NotFound(new { message = "Giải đấu không tồn tại." });
                }
                else
                {
                    throw;
                }
            }

            // Trả về thông báo thành công
            return Ok(new { message = "Giải đấu đã được xóa thành công.", success = true });
        }

        [HttpGet("getTournamentByMonth/{time?}")] // Tham số time là optional
        public async Task<IActionResult> GetTournamentsByMonth(string time = null)
        {
            try
            {
                IQueryable<Tournament> query = _db.Tournaments;

                if (!string.IsNullOrEmpty(time))
                {
                    // Parse yearMonth string (format: "2024-09")
                    if (!DateTime.TryParseExact(time + "-01", "yyyy-MM-dd",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                    {
                        return BadRequest("Invalid date format. Use yyyy-MM format");
                    }

                    // Get first and last day of the month
                    var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                    var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                    // Filter tournaments based on StartDate
                    query = query.Where(t => t.StartDate >= firstDayOfMonth && t.StartDate <= lastDayOfMonth);
                }

                // Query tournaments
                var tournaments = await query
                    .Select(t => new
                    {
                        t.TournamentId,
                        t.Name,
                        t.StartDate,
                        t.EndDate,
                        t.Status,
                        t.NumberOfTeams,
                        Level = t.Level.LevelName,
                        User = new
                        {
                            t.User.FullName,
                            t.User.Email
                        },
                        Teams = t.Teams.Select(team => new
                        {
                            team.TeamId,
                            team.TeamName,
                            team.Avatar,
                            MemberCount = team.TeamTournaments.Count()
                        }).ToList()
                    })
                    .AsNoTracking()
                    .ToListAsync();

                if (!tournaments.Any())
                {
                    return NotFound(new { Message = "No tournaments found." });
                }

                return Ok(tournaments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "Internal server error", Details = ex.Message });
            }
        }


        [HttpGet("getTotalTournaments")]
        public async Task<IActionResult> GetTotalTournaments()
        {
            try
            {
                // Đếm tổng số tournaments trong cơ sở dữ liệu
                var totalTournaments = await _db.Tournaments.CountAsync();

                // Trả về kết quả
                return Ok(new { TotalTournaments = totalTournaments });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "Internal server error", Details = ex.Message });
            }
        }
        [HttpGet("getTotalUsers")]
        public async Task<IActionResult> GetTotalUsers()
        {
            try
            {
                // Đếm tổng số người dùng trong bảng Users
                var totalUsers = await _db.Users.CountAsync();

                // Trả về kết quả
                return Ok(new { TotalUsers = totalUsers });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "Internal server error", Details = ex.Message });
            }
        }
        [HttpGet("getTotalUsersActive")]
        public async Task<IActionResult> GetTotalUsersActive()
        {
            try
            {
                // Đếm tổng số người dùng trong bảng Users
                var totalUsers = await _db.Users.Where(x=>x.Status == "Active").CountAsync();

                // Trả về kết quả
                return Ok(new { TotalUsers = totalUsers });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "Internal server error", Details = ex.Message });
            }
        }
        [HttpGet("getFundAdmin")]
        public async Task<IActionResult> GetFundAdmin()
        {
            try
            {
                // Truy vấn tổng quỹ (giả sử cột quỹ tên là "Fund")
                var totalFund = await _db.Admins.SumAsync(a => a.Fund);

                // Trả về kết quả
                return Ok(new { TotalFund = totalFund });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "Internal server error", Details = ex.Message });
            }
        }

        private bool TournamentExists(int id)
        {
            return _db.Tournaments.Any(e => e.TournamentId == id);
        }

    }

}
