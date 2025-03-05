using EcommerceWebsite.DataContext.Repository.IRepository;
using EcommerceWebsite.EntityModels;
using EcommerceWebsite.EntityModels.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceWebsite.Areas.Customer
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var cartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product");
            ShoppingCartVM shoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCartList = cartList,
                CountItems = cartList.Sum(cart => cart.Count),
                TotalPrice = GetTotalPriceOnCart(cartList)
            };
            return View(shoppingCartVM);
        }
        public IActionResult Plus(int id)
        {
            ShoppingCart shoppingCartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == id);
            if (shoppingCartFromDb == null)
            {
                return NotFound();
            }
            else
            {
                shoppingCartFromDb.Count++;
                _unitOfWork.ShoppingCart.Update(shoppingCartFromDb);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            };
        }
        public IActionResult Minus(int id)
        {
            ShoppingCart shoppingCartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == id);
            if (shoppingCartFromDb == null)
            {
                return NotFound();
            }
            if (shoppingCartFromDb.Count > 1)
            {
                shoppingCartFromDb.Count--;
                _unitOfWork.ShoppingCart.Update(shoppingCartFromDb);                
            }
            else
            {
                _unitOfWork.ShoppingCart.Remove(shoppingCartFromDb);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult RemoveFromCart(int id)
        {
            ShoppingCart shoppingCartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == id);
            if (shoppingCartFromDb == null)
            {
                return NotFound();
            }
            else
            {
                _unitOfWork.ShoppingCart.Remove(shoppingCartFromDb);
                _unitOfWork.Save();
            }
            return RedirectToAction(nameof(Index));
        }
        private double GetTotalPriceOnCart(IEnumerable<ShoppingCart> ShoppingCartList)
        {
            double totalPrice = 0;
            foreach (var cart in ShoppingCartList)
            {
                cart.Price = cart.Product.Price * cart.Count;
                totalPrice += cart.Price;

            }
            return totalPrice;
        }
    }
}
