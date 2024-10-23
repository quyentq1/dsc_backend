using System;
using System.Collections.Generic;

namespace dsc_backend.Models;

public partial class TransferHistory
{
    public int TransferId { get; set; }

    public int? SenderClubId { get; set; }

    public int? ReceiverClubId { get; set; }

    public int? SenderAdminId { get; set; }

    public int? ReceiverAdminId { get; set; }

    public decimal? TransferAmount { get; set; }

    public DateTime? TransferDate { get; set; }

    public virtual Admin? ReceiverAdmin { get; set; }

    public virtual Club? ReceiverClub { get; set; }

    public virtual Admin? SenderAdmin { get; set; }

    public virtual Club? SenderClub { get; set; }
}
