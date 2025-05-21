﻿using System;
using System.Collections.Generic;

namespace api_LuanVan.Models;

public partial class Region
{
    public int RegionId { get; set; }

    public string RegionName { get; set; } = null!;

    public virtual ICollection<Menu> Menus { get; set; } = new List<Menu>();

    public virtual ICollection<Table> Tables { get; set; } = new List<Table>();
}
