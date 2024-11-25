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
    public class TournamentController : Controller
    {
        private readonly ILogger<AuthenController> _logger;
        private readonly DscContext _db;
        private readonly Md5Helper _hashpass;
        private readonly CloudinarySettings _cloudinarySettings;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public TournamentController(ILogger<AuthenController> logger, DscContext db, IOptions<CloudinarySettings> cloudinarySettings, IWebHostEnvironment webHostEnvironment, Md5Helper hashpass)
        {
            _logger = logger;
            _hashpass = hashpass;
            _db = db;
            _cloudinarySettings = cloudinarySettings.Value;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet("GetAllTournament")]
        public async Task<IActionResult> GetAllTournament([FromQuery] int userId)
        {
            // Kiểm tra userId hợp lệ
            if (userId <= 0)
            {
                return BadRequest("Invalid userId.");
            }

            // Truy vấn danh sách Tournament
            var tournaments = await _db.Tournaments
    .Where(x => x.UserId != userId &&
                !_db.Teams.Any(team => team.TournamentId == x.TournamentId && team.UserId == userId))
    .Select(a => new
    {
        a.TournamentId,
        a.Name,
        a.NumberOfTeams,
        a.Location,
        a.StartDate,
        a.EndDate,
        LevelName = a.Level.LevelName,
    })
    .ToListAsync();


            // Kiểm tra danh sách rỗng
            if (!tournaments.Any())
            {
                return Ok("No tournaments found for the given user.");
            }

            return Ok(tournaments);
        }
        [HttpGet("GetAllTournamentJoin")]
        public async Task<IActionResult> GetAllTournamentJoin([FromQuery] int userId)
        {
            // Kiểm tra userId hợp lệ
            if (userId <= 0)
            {
                return BadRequest("Invalid userId.");
            }

            // Truy vấn danh sách Tournament
            var tournaments = await _db.Tournaments
    .Where(x => x.UserId != userId &&
                _db.Teams.Any(team => team.TournamentId == x.TournamentId && team.UserId == userId))
    .Select(a => new
    {
        a.TournamentId,
        a.Name,
        a.NumberOfTeams,
        a.Location,
        a.StartDate,
        a.EndDate,
        LevelName = a.Level.LevelName,
    })
    .ToListAsync();


            // Kiểm tra danh sách rỗng
            if (!tournaments.Any())
            {
                return Ok("No tournaments found for the given user.");
            }

            return Ok(tournaments);
        }
        [HttpGet("GetMyTournament")]
        public async Task<IActionResult> GetMyTournament([FromQuery] int userId)
        {
            // Kiểm tra userId hợp lệ
            if (userId <= 0)
            {
                return BadRequest("Invalid userId.");
            }

            // Truy vấn danh sách Tournament
            var tournaments = await _db.Tournaments
                .Where(x => x.UserId == userId)
                .Select(a => new
                {
                    a.TournamentId,
                    a.Name,
                    a.NumberOfTeams,
                    a.Location,
                    a.StartDate,
                    a.EndDate,
                })
                .ToListAsync();

            // Kiểm tra danh sách rỗng
            if (!tournaments.Any())
            {
                return Ok("No tournaments found for the given user.");
            }

            return Ok(tournaments);
        }
        [HttpGet("getTournamentDetails/{tournamentId}")]
        public async Task<IActionResult> getTournamentDetails(int tournamentId)
        {
            var tournaments = await _db.Tournaments
                .Select(a => new
                {
                    a.TournamentId,
                    a.Name,
                    a.StartDate,
                    a.Location,
                    a.NumberOfTeams,
                    a.Description,
                    a.EndDate,
                    a.MemberOfTeams,
                    a.LimitRegister,
                    a.CreatedDate,
                    a.LevelId,
                    LevelName = a.Level.LevelName
                    // Thêm các thuộc tính khác của Activity mà bạn muốn lấy

                })
                .Where(x => x.TournamentId == tournamentId)
                .ToListAsync();

            if (!tournaments.Any())
            {
                return NotFound();
            }

            return Ok(tournaments);
        }
        [HttpPost("createTournament")]
        public async Task<IActionResult> createTournament([FromBody] CreateTournamentDAO tournaments)
        {
            if (tournaments == null)
            {
                return BadRequest("Invalid tournament data");
            }

            try
            {
                var AddTournament = new Tournament
                {
                    LevelId = tournaments.LevelId,
                    Name = tournaments.Name,
                    Description = tournaments.note,
                    StartDate = tournaments.StartDate,
                    EndDate = tournaments.EndDate,
                    Location = tournaments.location,
                    NumberOfTeams = tournaments.teamSize,
                    CreatedDate = tournaments.startTime,
                    MemberOfTeams = tournaments.numberOfParticipants,
                    LimitRegister = tournaments.RegistrationDeadline,
                    UserId = tournaments.UserId,
                };

                await _db.Tournaments.AddAsync(AddTournament);
                await _db.SaveChangesAsync();

                return Ok(new
                {
                    Success = true,
                    Message = "Đã thêm kèo đấu thành công",
                    Data = AddTournament
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Lỗi khi tạo giải đấu",
                    Error = ex.Message
                });
            }
        }

        [HttpPost("deleteTournament/{tournamentId}")]
        public async Task<IActionResult> deleteTournament(int tournamentId)
        {
            var tournament = await _db.Tournaments.Where(x=>x.TournamentId == tournamentId).FirstOrDefaultAsync();
            if (tournament == null)
            {
                return NotFound(new { message = "Giải đấu không tồn tại." });
            }

            // Nếu tìm thấy, xóa giải đấu
            _db.Tournaments.Remove(tournament);

            // Lưu thay đổi vào cơ sở dữ liệu
            await _db.SaveChangesAsync();

            // Trả về thông báo thành công
            return Ok(new { message = "Giải đấu đã được xóa thành công.", success = true });

        }
        [HttpPost("addMemberTeam")]
        public async Task<IActionResult> addMemberTeam(AddMemberTeamDAO request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.TeamName) || request.Players == null || !request.Players.Any())
            {
                return BadRequest("Invalid request");
            }


            try
            {
                // Add team
                var team = new Team
                {
                    TournamentId = request.TournamentId,
                    UserId = request.UserId,
                    TeamName = request.TeamName
                };
                _db.Teams.Add(team);
                await _db.SaveChangesAsync();

                // Add players to TeamTournament
                var teamTournaments = request.Players.Select(player => new TeamTournament
                {
                    TeamId = team.TeamId,
                    NamePlayer = player.Name,
                    NumberPlayer = player.Number
                }).ToList();

                _db.TeamTournaments.AddRange(teamTournaments);
                await _db.SaveChangesAsync();
                return Ok(new { Success = true, Message = "Team added successfully", TeamId = team.TeamId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "An error occurred", Details = ex.Message });
            }
        }
            [HttpPost("updateTounarment/{tournamentId}")]
        public async Task<IActionResult> updateTounarment(int tournamentId, [FromBody] CreateTournamentDAO tournament)
        {
            try
            {
                if (tournament == null)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Dữ liệu không hợp lệ"
                    });
                }

                var existingTournament = await _db.Tournaments
                    .FirstOrDefaultAsync(x => x.TournamentId == tournamentId);

                if (existingTournament == null)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Không tìm thấy giải đấu"
                    });
                }

                // Cập nhật thông tin giải đấu
                existingTournament.Name = tournament.Name;
                existingTournament.LevelId = tournament.LevelId;
                existingTournament.Description = tournament.note;
                existingTournament.StartDate = tournament.StartDate;
                existingTournament.EndDate = tournament.EndDate;
                existingTournament.LimitRegister = tournament.RegistrationDeadline;
                existingTournament.Location = tournament.location;
                existingTournament.NumberOfTeams = tournament.teamSize;
                existingTournament.CreatedDate = tournament.startTime;
                existingTournament.MemberOfTeams = tournament.numberOfParticipants;

                await _db.SaveChangesAsync();

                return Ok(new
                {
                    Success = true,
                    Message = "Cập nhật giải đấu thành công",
                    Data = existingTournament
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi cập nhật giải đấu",
                    Error = ex.Message
                });
            }

        }
    } 
}
