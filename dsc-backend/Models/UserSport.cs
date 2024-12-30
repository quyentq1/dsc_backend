using System;
using System.Collections.Generic;

namespace dsc_backend.Models;

public partial class UserSport
{
    public int UserSportId { get; set; }

    public int UserId { get; set; }

    public int? SportId { get; set; }

    public int? LevelId { get; set; }

    public string? Achievement { get; set; }

    public string? Position { get; set; }

    public virtual Level? Level { get; set; }

    public virtual Sport? Sport { get; set; }

    public virtual User? User { get; set; }
}