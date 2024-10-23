using System;
using System.Collections.Generic;

namespace dsc_backend.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int? UserId { get; set; }

    public int? ClubId { get; set; }

    public int? TournamentId { get; set; }

    public decimal? Price { get; set; }

    public DateTime? PaymentDate { get; set; }

    public virtual Club? Club { get; set; }

    public virtual Tournament? Tournament { get; set; }

    public virtual User? User { get; set; }
}
