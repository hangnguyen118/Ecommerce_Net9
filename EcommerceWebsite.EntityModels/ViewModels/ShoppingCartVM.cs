using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceWebsite.EntityModels.ViewModels
{
    public class ShoppingCartVM
    {        
        public IEnumerable<ShoppingCart> ShoppingCartList { get; set; }
        public Order Order { get; set; }
        public int CountItems { get; set; }
    }
}
