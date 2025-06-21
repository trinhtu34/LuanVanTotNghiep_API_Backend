using System;
using System.Collections.Generic;

namespace api_LuanVan.Models;

public partial class Cart
{
    public long CartId { get; set; }

    public string UserId { get; set; } = null!;

    public DateTime? OrderTime { get; set; }

    public virtual ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();

    public virtual ICollection<PaymentResult> PaymentResults { get; set; } = new List<PaymentResult>();

    public virtual User User { get; set; } = null!;
}
