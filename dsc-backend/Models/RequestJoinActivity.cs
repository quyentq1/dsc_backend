using System;
using System.Collections.Generic;

namespace dsc_backend.Models;

public partial class RequestJoinActivity
{
    public int RequestJoinActivityId { get; set; }

    public int UserId { get; set; }

    public int ActivitiesId { get; set; }

    public string? Status { get; set; }

    public DateTime? CreateDate { get; set; }

    public virtual Activity? Activities { get; set; }

    public virtual User? User { get; set; }
}