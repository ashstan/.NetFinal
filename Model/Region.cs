﻿using System;
using System.Collections.Generic;

#nullable disable

namespace Week11_NorthwindConsole.Model
{
    public partial class Region
    {
        public Region()
        {
            Territories = new HashSet<Territory>();
        }

        public int RegionId { get; set; }
        public string RegionDescription { get; set; }

        public virtual ICollection<Territory> Territories { get; set; }
    }
}