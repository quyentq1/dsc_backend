namespace dsc_backend.DAO
{
    public class AddMemberTeamDAO
    {
        public int UserId { get; set; }
        public int TournamentId { get; set; }
        public string TeamName { get; set; }
        public List<PlayerRequestDAO> Players { get; set; }
    }
}
