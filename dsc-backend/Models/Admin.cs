using System;
using System.Collections.Generic;

namespace dsc_backend.Models;

public partial class Admin
{
    public int AdminId { get; set; }

    public int? RoleId { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public decimal? Fund { get; set; }

    public virtual Role? Role { get; set; }

    public virtual ICollection<TransferHistory> TransferHistoryReceiverAdmins { get; set; } = new List<TransferHistory>();

    public virtual ICollection<TransferHistory> TransferHistorySenderAdmins { get; set; } = new List<TransferHistory>();
}
