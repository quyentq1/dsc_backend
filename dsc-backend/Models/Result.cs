using System;
using System.Collections.Generic;

namespace dsc_backend.Models;

public partial class Result
{
    public int ResultId { get; set; }

    public int? MatchId { get; set; }

    public int? ScoreTeam1 { get; set; }

    public int? ScoreTeam2 { get; set; }

    public int? PenaltyTeam1 { get; set; }

    public int? PenaltyTeam2 { get; set; }

    public virtual Match? Match { get; set; }
}
