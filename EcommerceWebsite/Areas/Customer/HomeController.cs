using System.Diagnostics;
using System.Security.Claims;
using EcommerceWebsite.DataContext.Repository.IRepository;
using EcommerceWebsite.EntityModels;
using EcommerceWebsite.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceWebsite.Areas.Customer
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim != null)
            {
                HttpContext.Session.SetInt32(SD.SessionCart,
                    _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).Count());
            }

            List<Product> products = _unitOfWork.Product.GetAll().ToList();
            return View(products);
        }
        public IActionResult Details(int id)
        {
            Product productFromDb = _unitOfWork.Product.Get(u => u.Id == id, includeProperties: "Category");
            if (productFromDb == null)
            {
                return NotFound();
            }
            ShoppingCart shoppingCart = new ShoppingCart()
            {
                Product = productFromDb,                
                Count = 1
            };
            return View(shoppingCart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCart shoppingCartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == userId &&
                u.ProductId == shoppingCart.ProductId
            );
            if (shoppingCartFromDb != null)
            {
                shoppingCartFromDb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(shoppingCartFromDb);
                _unitOfWork.Save();
            }
            else
            {
                shoppingCart.ApplicationUserId = userId;
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
                var itemInCart = _unitOfWork.ShoppingCart.GetAll (u => u.ApplicationUserId == userId).Count();
                HttpContext.Session.SetInt32(SD.SessionCart, itemInCart);
            };            
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
