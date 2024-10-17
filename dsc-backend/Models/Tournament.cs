using System;
using System.Collections.Generic;

namespace dsc_backend.Models;

public partial class Tournament
{
    public int TournamentId { get; set; }

    public int? LevelId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Location { get; set; }

    public int? NumberOfTeams { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual Level? Level { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Round> Rounds { get; set; } = new List<Round>();

    public virtual ICollection<TeamTournament> TeamTournaments { get; set; } = new List<TeamTournament>();

    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();
}
