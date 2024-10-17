using System;
using System.Collections.Generic;

namespace dsc_backend.Models;

public partial class ResultOfActivity
{
    public int ResultId { get; set; }

    public int? ActivityId { get; set; }

    public int? Team1Score { get; set; }

    public int? Team2Score { get; set; }

    public virtual Activity? Activity { get; set; }
}
