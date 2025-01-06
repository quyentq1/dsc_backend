namespace dsc_backend.DAO
{
    public class MatchDAO
    {
        public int? MatchNumber { get; set; }
        public int? Team1Id { get; set; }
        public int? Team2Id { get; set; }
        public int? Score1 { get; set; }
        public int? Score2 { get; set; }
        public int? Penalty1 { get; set; }
        public int? Penalty2 { get; set; }

        public string? Location { get; set; }

        public DateTime? Time { get; set; }
    }
}
