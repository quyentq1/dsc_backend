using System;
using System.Collections.Generic;

namespace dsc_backend.Models;

public partial class Match
{
    public int MatchId { get; set; }

    public int? RoundId { get; set; }

    public int? Team1Id { get; set; }

    public int? Team2Id { get; set; }

    public int? MatchNumber { get; set; }

    public int? TournamentId { get; set; }

    public string? Location { get; set; }

    public DateTime? Time { get; set; }

    public virtual ICollection<Result> Results { get; set; } = new List<Result>();

    public virtual Round? Round { get; set; }

    public virtual Team? Team1 { get; set; }

    public virtual Team? Team2 { get; set; }

    public virtual Tournament? Tournament { get; set; }
}
