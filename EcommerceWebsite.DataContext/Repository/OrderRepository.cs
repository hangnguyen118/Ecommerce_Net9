using EcommerceWebsite.DataContext.Repository.IRepository;
using EcommerceWebsite.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceWebsite.DataContext.Repository
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        ApplicationDbContext _db;
        public OrderRepository(ApplicationDbContext db): base(db) 
        {
            _db =  db;
        }
        public void Update(Order obj)
        {
            _db.Orders.Update(obj);
        }
        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderFromDb = _db.Orders.FirstOrDefault(u => u.Id == id);
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if(string.IsNullOrEmpty(paymentStatus))
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
        }
        public void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId = null)
        {
            var orderFromDb = _db.Orders.FirstOrDefault(u => u.Id == id);
            if (!string.IsNullOrEmpty(sessionId))
            {
                orderFromDb.SessionId = sessionId;                
            }
            if(!string.IsNullOrEmpty(paymentIntentId))
            {
                orderFromDb.PaymentIntentId = paymentIntentId;
                orderFromDb.PaymentDate = DateTime.Now;
            }    
        }
    }
}
