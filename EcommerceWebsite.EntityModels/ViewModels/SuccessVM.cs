using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceWebsite.EntityModels.ViewModels
{
    public class SuccessVM
    {
        public string OrderId { get; set; }
        public double TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
