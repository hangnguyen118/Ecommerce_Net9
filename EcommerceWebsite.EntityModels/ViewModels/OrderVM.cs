﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceWebsite.EntityModels.ViewModels
{
    public class OrderVM
    {
        public Order Order { get; set; }
        public IEnumerable<OrderDetail> OrderDetail { get; set; }
    }
}
