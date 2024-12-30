using System;
using System.Collections.Generic;

namespace dsc_backend.Models;

public partial class TeamTournament
{
    public int TeamTournamentId { get; set; }

    public int? TeamId { get; set; }

    public string? NamePlayer { get; set; }

    public int NumberPlayer { get; set; }

    public virtual Team? Team { get; set; }
}