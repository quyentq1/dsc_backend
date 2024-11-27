namespace dsc_backend.DAO
{
    public class SaveTournamentResultsRequest
    {
        public int TournamentId { get; set; }
        public Dictionary<string, List<MatchDAO>> Matches { get; set; }
    }
}
