using System;
using System.Collections.Generic;

namespace dsc_backend.Models;

public partial class UserActivity
{
    public int UserActivityId { get; set; }

    public int? UserId { get; set; }

    public int? ActivityId { get; set; }

    public DateTime? JoinDate { get; set; }

    public string? RoleInActivity { get; set; }

    public virtual Activity? Activity { get; set; }

    public virtual User? User { get; set; }
}
