using CricArena.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CricArena.Core.Entities
{
    public class Payment
    {
        public Guid Id { get; set; }
        public Guid MatchId { get; set; }
        public Guid PlayerId { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public DateTime? PaidOn { get; set; }

        // Navigation Properties (for EF Core)
        public Match Match { get; set; } = null!;
        public Player Player { get; set; } = null!;
    }
}
