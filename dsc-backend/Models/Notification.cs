using System;
using System.Collections.Generic;

namespace dsc_backend.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int? ActivityId { get; set; }

    public int? TournamentId { get; set; }

    public string? Content { get; set; }

    public int? UserId { get; set; }

    public virtual Activity? Activity { get; set; }

    public virtual Tournament? Tournament { get; set; }

    public virtual User? User { get; set; }
}
