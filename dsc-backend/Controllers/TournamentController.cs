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
        [HttpGet("getTeamTournament/{tournamentId}")]
        public async Task<IActionResult> GetTeamTournament(int tournamentId)
        {
            var tournaments = await _db.Tournaments
                .Include(x => x.Teams)
                .Where(x => x.TournamentId == tournamentId)
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
                    LevelName = a.Level.LevelName,
                    TeamId = a.Teams.Select(t=>t.TeamId).ToList(),
                    TeamCount = a.Teams.Count(), // Đếm số lượng team
                    TeamNames = a.Teams.Select(t => t.TeamName).ToList() // Lấy danh sách tên của các team
                })
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
        private async Task<int> GetOrCreateRoundId(int tournamentId, int roundNumber)
        {
            // Thêm điều kiện TournamentId vào query
            var round = await _db.Rounds
                .Where(r => r.TournamentId == tournamentId && r.RoundNumber == roundNumber)
                .FirstOrDefaultAsync();

            if (round == null)
            {
                // Tạo mới round với TournamentId được chỉ định
                round = new Round
                {
                    TournamentId = tournamentId,
                    RoundNumber = roundNumber
                };

                _db.Rounds.Add(round);
                await _db.SaveChangesAsync();
            }

            return round.RoundId;
        }
        [HttpPost("saveTournamentResults")]
        public async Task<IActionResult> SaveTournamentResults([FromBody] SaveTournamentResultsRequest request)
        {
            if (request.Matches == null || request.Matches.Count == 0)
            {
                return BadRequest("No match data provided.");
            }

            try
            {
                foreach (var round in request.Matches)
                {
                    int roundNumber = int.Parse(round.Key);
                    List<MatchDAO> roundMatches = round.Value;

                    // Lấy hoặc tạo RoundId
                    int roundId = await GetOrCreateRoundId(request.TournamentId, roundNumber);

                    // Log để kiểm tra
                    _logger.LogInformation($"Created/Retrieved RoundId: {roundId} for TournamentId: {request.TournamentId}, RoundNumber: {roundNumber}");

                    foreach (var matchDto in roundMatches)
                    {
                        // Tìm hoặc tạo trận đấu
                        var match = await _db.Matches
                            .FirstOrDefaultAsync(m => m.RoundId == roundId && m.MatchNumber == matchDto.MatchNumber);

                        if (match == null)
                        {
                            match = new Match
                            {
                                RoundId = roundId,
                                MatchNumber = matchDto.MatchNumber
                            };
                            if (matchDto.Team1Id.HasValue) match.Team1Id = matchDto.Team1Id.Value;
                            if (matchDto.Team2Id.HasValue) match.Team2Id = matchDto.Team2Id.Value;
                            _db.Matches.Add(match);
                        }
                        else
                        {
                            // Cập nhật thông tin trận đấu nếu cần
                            if (matchDto.Team1Id.HasValue) match.Team1Id = matchDto.Team1Id.Value;
                            if (matchDto.Team2Id.HasValue) match.Team2Id = matchDto.Team2Id.Value;
                            _db.Matches.Update(match);
                        }

                        await _db.SaveChangesAsync(); // Lưu để có MatchId

                        // Chỉ xử lý kết quả nếu có ít nhất một điểm số
                        if (matchDto.Score1.HasValue || matchDto.Score2.HasValue)
                        {
                            var result = await _db.Results.FirstOrDefaultAsync(r => r.MatchId == match.MatchId);
                            if (result == null)
                            {
                                result = new Result
                                {
                                    MatchId = match.MatchId
                                };
                                if (matchDto.Score1.HasValue) result.ScoreTeam1 = matchDto.Score1.Value;
                                if (matchDto.Score2.HasValue) result.ScoreTeam2 = matchDto.Score2.Value;
                                _db.Results.Add(result);
                            }
                            else
                            {
                                // Cập nhật kết quả nếu đã tồn tại
                                if (matchDto.Score1.HasValue) result.ScoreTeam1 = matchDto.Score1.Value;
                                if (matchDto.Score2.HasValue) result.ScoreTeam2 = matchDto.Score2.Value;
                                _db.Results.Update(result);
                            }
                        }
                    }
                }

                await _db.SaveChangesAsync();
                return Ok(new { success = true, message = "Tournament results saved successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Internal server error: {ex.Message}" });
            }
        }
        [HttpGet("getTournamentResults/{tournamentId}")]
        public async Task<IActionResult> GetTournamentResults(int tournamentId)
        {
            try
            {
                var results = await _db.Matches
                    .Include(m => m.Round) // Thêm Include để đảm bảo load được Round
                    .Where(m => m.Round.TournamentId == tournamentId) // Lọc theo tournamentId
                    .Select(m => new {
                        RoundNumber = m.Round.RoundNumber,
                        MatchNumber = m.MatchNumber,
                        Team1Id = m.Team1Id,
                        Team2Id = m.Team2Id,
                        Score1 = m.Results.Select(a => a.ScoreTeam1),
                        Score2 = m.Results.Select(a => a.ScoreTeam2)
                    })
                    .ToListAsync();

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Internal server error: {ex.Message}" });
            }
        }


    }
}
