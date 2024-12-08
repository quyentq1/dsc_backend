using System;
using System.Collections.Generic;

namespace dsc_backend.Models;

public partial class ResultOfActivitiesClub
{
    public int ResultId { get; set; }

    public int? ActivityClubId { get; set; }

    public int? Team1Score { get; set; }

    public int? Team2Score { get; set; }

    public virtual ActivitiesClub? ActivityClub { get; set; }
}
