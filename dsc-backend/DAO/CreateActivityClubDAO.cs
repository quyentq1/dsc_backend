namespace dsc_backend.DAO
{
    public class CreateActivityClubDAO
    {
        public int activityclubId { get; set; }    
        public string? sport { get; set; }
        public string? eventType { get; set; }
        public string? datetime { get; set; }
        public string? location { get; set; }
        public int playerCount { get; set; }
        public bool IsFree { get; set; }
        public int? PerPerson { get; set; }
        public int? PerTeam { get; set; }
        public string? minSkill { get; set; }
        public string? maxSkill { get; set; }
        public string? name { get; set; }
        public string? description { get; set; }
        public int clubId { get; set; }
        public int UserId { get; set; }
        public int? amount { get; set;}

    }
}
