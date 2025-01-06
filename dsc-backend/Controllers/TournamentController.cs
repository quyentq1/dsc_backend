using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using dsc_backend.DAO;
using dsc_backend.Helper;
using dsc_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;

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
                    a.Avatar,
                    a.TournamentType
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
                    LevelName = a.Level.LevelName,
                    a.Avatar,
                    NumberOfRegisteredTeams = _db.Teams.Count(t => t.TournamentId == a.TournamentId), // Đếm số team đã tham gia
                    a.TournamentType
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
                    TeamNames = a.Teams.Select(t => t.TeamName).ToList()
                })
                .ToListAsync();

            if (!tournaments.Any())
            {
                return NotFound();
            }

            return Ok(tournaments);
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


        [HttpPost("createTournament")]
        public async Task<IActionResult> CreateTournament([FromForm] CreateTournamentDAO tournaments, [FromForm] IFormFile file)
        {
            if (tournaments == null)
            {
                return BadRequest(new { Success = false, Message = "Dữ liệu giải đấu không hợp lệ" });
            }

            try
            {
                // Khởi tạo đối tượng Tournament
                var addTournament = new Tournament
                {
                    LevelId = tournaments.LevelId,
                    Name = tournaments.Name,
                    Description = tournaments.note,
                    StartDate = tournaments.StartDate,
                    EndDate = tournaments.EndDate,
                    Location = tournaments.location,
                    NumberOfTeams = tournaments.numberOfParticipants,
                    CreatedDate = tournaments.startTime,
                    MemberOfTeams = tournaments.teamSize,
                    LimitRegister = tournaments.RegistrationDeadline,
                    UserId = tournaments.UserId,
                    TournamentType = tournaments.TournamentType
                };

                // Xử lý upload ảnh nếu có file
                if (file != null && file.Length > 0)
                {
                    try
                    {
                        var uploadResult = await UploadToCloudinary(file);

                        if (uploadResult != null)
                        {
                            addTournament.Avatar = uploadResult.SecureUrl.ToString();
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

                // Lưu giải đấu vào database
                await _db.Tournaments.AddAsync(addTournament);
                await _db.SaveChangesAsync();

                return Ok(new
                {
                    Success = true,
                    TournamentId = addTournament.TournamentId,
                    Message = "Đã thêm kèo đấu thành công",
                    Data = addTournament
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
        public async Task<IActionResult> addMemberTeam([FromForm] AddMemberTeamDAO request, [FromForm] IFormFile file)
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
                if (file != null && file.Length > 0)
                {
                    try
                    {
                        var uploadResult = await UploadToCloudinary(file);

                        if (uploadResult != null)
                        {
                            team.Avatar = uploadResult.SecureUrl.ToString();
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
        public async Task<IActionResult> updateTounarment(int tournamentId, [FromForm] CreateTournamentDAO tournament, IFormFile file = null)
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
                existingTournament.Name = tournament.Name ?? existingTournament.Name;
                existingTournament.LevelId = tournament.LevelId;
                existingTournament.Description = tournament.note ?? existingTournament.Description;
                existingTournament.StartDate = tournament?.StartDate ?? existingTournament.StartDate;
                existingTournament.EndDate = tournament?.EndDate ?? existingTournament.EndDate;
                existingTournament.LimitRegister = tournament?.RegistrationDeadline ?? existingTournament.LimitRegister;
                existingTournament.Location = tournament.location ?? existingTournament.Location;
                existingTournament.NumberOfTeams = tournament?.numberOfParticipants ?? existingTournament.NumberOfTeams;
                existingTournament.CreatedDate = tournament?.startTime ?? existingTournament.CreatedDate;
                existingTournament.MemberOfTeams = tournament?.teamSize ?? existingTournament.MemberOfTeams;

                // Chỉ xử lý file nếu có file được gửi lên
                if (file != null && file.Length > 0)
                {
                    try
                    {
                        var uploadResult = await UploadToCloudinary(file);

                        if (uploadResult != null)
                        {
                            existingTournament.Avatar = uploadResult.SecureUrl.ToString();
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
                // Nếu không có file mới, giữ nguyên ảnh cũ

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
            // Kiểm tra vòng có TournamentId và RoundNumber
            var round = await _db.Rounds
                .Where(r => r.TournamentId == tournamentId && r.RoundNumber == roundNumber)
                .FirstOrDefaultAsync();

            if (round == null)
            {
                // Tạo mới vòng với TournamentId và RoundNumber
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
            if (request == null || request.Matches.Count == 0)
            {
                return BadRequest("No match data provided.");
            }

            try
            {
                foreach (var roundData in request.Matches)
                {
                    int roundNumber = int.Parse(roundData.Key);
                    List<MatchDAO> roundMatches = roundData.Value;

                    // Lấy hoặc tạo Round
                    var round = await _db.Rounds.FirstOrDefaultAsync(r =>
                        r.TournamentId == request.TournamentId &&
                        r.RoundNumber == roundNumber);

                    if (round == null)
                    {
                        round = new Round
                        {
                            TournamentId = request.TournamentId,
                            RoundNumber = roundNumber
                        };
                        _db.Rounds.Add(round);
                        await _db.SaveChangesAsync(); // Lưu để có RoundId
                    }

                    foreach (var matchDto in roundMatches)
                    {
                        // Lấy hoặc tạo Match
                        var match = await _db.Matches.FirstOrDefaultAsync(m =>
                            m.RoundId == round.RoundId &&
                            m.MatchNumber == matchDto.MatchNumber);

                        if (match == null)
                        {
                            match = new Match
                            {
                                RoundId = round.RoundId,
                                TournamentId = request.TournamentId, // Thêm TournamentId
                                MatchNumber = matchDto.MatchNumber,
                                Team1Id = matchDto.Team1Id,
                                Team2Id = matchDto.Team2Id,
                                Time = matchDto.Time,           // Thêm Time
                                Location = matchDto.Location    // Thêm Location
                            };
                            _db.Matches.Add(match);
                            await _db.SaveChangesAsync(); // Lưu để có MatchId
                        }
                        else
                        {
                            // Cập nhật Match nếu cần
                            match.Team1Id = matchDto.Team1Id;
                            match.Team2Id = matchDto.Team2Id;
                            match.TournamentId = request.TournamentId; // Cập nhật TournamentId
                            match.Time = matchDto.Time;         // Cập nhật Time
                            match.Location = matchDto.Location; // Cập nhật Location
                            _db.Matches.Update(match);
                            await _db.SaveChangesAsync();
                        }

                        // Lưu kết quả nếu có, với điều kiện là điểm số có giá trị
                        if (matchDto.Score1.HasValue || matchDto.Score2.HasValue)
                        {
                            var result = await _db.Results.FirstOrDefaultAsync(r => r.MatchId == match.MatchId);
                            if (result == null)
                            {
                                result = new Result
                                {
                                    MatchId = match.MatchId,
                                    ScoreTeam1 = matchDto.Score1,
                                    ScoreTeam2 = matchDto.Score2,
                                    PenaltyTeam1 = matchDto.Penalty1,
                                    PenaltyTeam2 =  matchDto.Penalty2
                                };
                                _db.Results.Add(result);
                            }
                            else
                            {
                                result.ScoreTeam1 = matchDto.Score1;
                                result.ScoreTeam2 = matchDto.Score2;
                                result.PenaltyTeam1 = matchDto.Penalty1;
                                result.PenaltyTeam2 = matchDto.Penalty2;
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
                    .Include(m => m.Round) // Load thông tin Round
                    .Include(m => m.Results) // Load thông tin Results
                    .Include(m => m.Team1) // Bao gồm thông tin đội 1
                    .Include(m => m.Team2) // Bao gồm thông tin đội 2
                    .Where(m => m.Round.TournamentId == tournamentId && m.TournamentId == tournamentId) // Thêm điều kiện TournamentId tại Match
                    .OrderBy(m => m.Round.RoundNumber) // Sắp xếp theo RoundNumber
                    .ThenBy(m => m.MatchNumber) // Sắp xếp thêm theo MatchNumber
                    .ToListAsync();

                var data = results.Select(m => new
                {
                    RoundNumber = m.Round.RoundNumber, // Thông tin Round
                    MatchNumber = m.MatchNumber,       // Số trận đấu
                    Team1Id = m.Team1Id,               // ID đội 1
                    Team2Id = m.Team2Id,               // ID đội 2
                    Team1Name = m.Team1?.TeamName, // Tên đội 1 (Nếu không có thì trả về TBD)
                    Team2Name = m.Team2?.TeamName, // Tên đội 2 (Nếu không có thì trả về TBD)
                    Score1 = m.Results.FirstOrDefault()?.ScoreTeam1, // Điểm đội 1
                    Score2 = m.Results.FirstOrDefault()?.ScoreTeam2, // Điểm đội 2
                    Penalty1 = m.Results.FirstOrDefault()?.PenaltyTeam1,
                    Penalty2 = m.Results.FirstOrDefault()?.PenaltyTeam2,
                    Time = m.Time,           // Thêm Time
                    Location = m.Location
                }).ToList();

                return Ok(new
                {
                    success = true,
                    message = "Tournament results fetched successfully.",
                    data = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Internal server error: {ex.Message}"
                });
            }
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

            if (!tournaments.Any())
            {
                return NotFound();
            }

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
            if (!members.Any() )
            {
                return NotFound(new { Message = "No members or team information found for the given IDs." });
            }

            return Ok(members);
        }
        
        [HttpGet("getAllTournamentNames")]
        public async Task<IActionResult> getAllTournamentNames()
        {
            try
            {
                // Query the database to get all activity names
                List<string> TournamentsNames = await _db.Tournaments
                    .Select(a => a.Name)
                    .ToListAsync();

                // Return the list of names as JSON
                return Ok(TournamentsNames);
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpPost("PaymentforTournament")]
        public async Task<IActionResult> PaymentforTournament()
        {
            try
            {
                // Lấy Admin hiện tại
                var existingFundAdmin = await _db.Admins.FirstOrDefaultAsync();

                if (existingFundAdmin == null)
                {
                    return NotFound("Admin not found.");
                }

                // Cộng 200,000 vào trường Fund của Admin
                existingFundAdmin.Fund += 200000;

                // Lưu lại thay đổi vào cơ sở dữ liệu
                _db.Admins.Update(existingFundAdmin);
                await _db.SaveChangesAsync();

                // Trả về danh sách tên giải đấu hoặc các dữ liệu bạn muốn
                return Ok(new { Fund = existingFundAdmin.Fund, Message = "Payment successful, Fund updated." });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }


    }
}
