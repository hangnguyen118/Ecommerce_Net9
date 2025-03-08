using EcommerceWebsite.DataContext.Repository.IRepository;
using EcommerceWebsite.EntityModels;
using EcommerceWebsite.EntityModels.ViewModels;
using EcommerceWebsite.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace EcommerceWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index(string? status)
        {
            IEnumerable<Order> orders = _unitOfWork.Order.GetAll(includeProperties: "ApplicationUser").ToList();
            switch (status)
            {
                case "pending":
                    orders = orders.Where(u => u.OrderStatus == SD.StatusPending);
                    break;
                case "inprocess":
                    orders = orders.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    orders = orders.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    orders = orders.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
                case "all":                    
                default:                    
                    break;

            }
            return View(orders.ToList());
        }
        public IActionResult Details(int id) {
            OrderVM orderVM = new()
            {
                Order = _unitOfWork.Order.Get(u => u.Id == id, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderId == id, includeProperties: "Product")
            };
            return View(orderVM);
        }
    }
}
