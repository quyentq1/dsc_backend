﻿using System;
using System.Collections.Generic;

namespace dsc_backend.Models;

public partial class Team
{
    public int TeamId { get; set; }

    public int? TournamentId { get; set; }

    public int? UserTeamId { get; set; }

    public string? TeamName { get; set; }

    public virtual ICollection<Match> MatchTeam1s { get; set; } = new List<Match>();

    public virtual ICollection<Match> MatchTeam2s { get; set; } = new List<Match>();

    public virtual ICollection<TeamTournament> TeamTournaments { get; set; } = new List<TeamTournament>();

    public virtual Tournament? Tournament { get; set; }

    public virtual ICollection<UserTeam> UserTeams { get; set; } = new List<UserTeam>();
}
