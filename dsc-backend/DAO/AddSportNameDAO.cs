namespace dsc_backend.DAO
{
    public class AddSportNameDAO
    {
        public int UserId { get; set; }
        public int SportId { get; set; }
        public int LevelId { get; set; }
        public string? Achievement { get; set; }
        public string? Position { get; set; }
    }
}
