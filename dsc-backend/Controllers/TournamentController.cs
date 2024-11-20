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
        [HttpGet("getAllTournament")]
        public async Task<IActionResult> GetAllTournament([FromQuery] int userId)
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
                return NotFound("No tournaments found for the given user.");
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
                    // Thêm các thuộc tính khác của Activity mà bạn muốn lấy
                    LevelName = a.Level.LevelName
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
    } 
}
