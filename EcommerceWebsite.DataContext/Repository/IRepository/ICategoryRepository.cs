using EcommerceWebsite.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceWebsite.DataContext.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {      
        void Update(Category category);
    }
}
