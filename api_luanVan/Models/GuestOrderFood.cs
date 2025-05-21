using System;
using System.Collections.Generic;

namespace api_LuanVan.Models;

public partial class GuestOrderFood
{
    public int GuestOrderFoodId { get; set; }

    public int GuestId { get; set; }

    public DateTime? OrderTime { get; set; }

    public virtual GuestUser Guest { get; set; } = null!;

    public virtual ICollection<GuestOrderFoodDetail> GuestOrderFoodDetails { get; set; } = new List<GuestOrderFoodDetail>();
}
