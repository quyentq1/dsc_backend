using dsc_backend.Helper;
using dsc_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

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
        public async Task<IActionResult> getAllTournament()
        {
            var tournaments = await _db.Tournaments
                .Include(t => t.Level) // Thông tin về Level
                .Include(t => t.Comments) // Thông tin về Comments
                .Include(t => t.Fees) // Thông tin về Fees
                .Include(t => t.Notifications) // Thông tin về Notifications
                .Include(t => t.Payments) // Thông tin về Payments
                .Include(t => t.Rounds) // Thông tin về Rounds
                .Include(t => t.TeamTournaments) // Thông tin về TeamTournaments
                .Include(t => t.Teams) // Thông tin về Teams
                .Include(t => t.User) // Thông tin về User
                .ToListAsync();

            if (tournaments == null)
            {
                return NotFound();
            }

            return Ok(tournaments);
        }
    }
}
