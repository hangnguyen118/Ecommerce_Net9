using EcommerceWebsite.DataContext.Repository.IRepository;
using EcommerceWebsite.EntityModels;
using EcommerceWebsite.EntityModels.ViewModels;
using EcommerceWebsite.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.BillingPortal;
using Stripe.Checkout;
using System.Security.Claims;
using Session = Stripe.Checkout.Session;
using SessionCreateOptions = Stripe.Checkout.SessionCreateOptions;
using SessionService = Stripe.Checkout.SessionService;

namespace EcommerceWebsite.Areas.Customer
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
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
                Order = new()
            };
            shoppingCartVM.Order.OrderTotal = GetTotalPriceOnCart(cartList);
            return View(shoppingCartVM);
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
        public IActionResult Checkout()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var cartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product");
            ShoppingCartVM shoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCartList = cartList,
                CountItems = cartList.Sum(cart => cart.Count),
                Order = new()
            };
            shoppingCartVM.Order.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            shoppingCartVM.Order.Name = shoppingCartVM.Order.ApplicationUser.Name;
            shoppingCartVM.Order.PhoneNumber = shoppingCartVM.Order.ApplicationUser.PhoneNumber;
            shoppingCartVM.Order.City = shoppingCartVM.Order.ApplicationUser.City;
            shoppingCartVM.Order.PostalCode = shoppingCartVM.Order.ApplicationUser.PostalCode;
            shoppingCartVM.Order.StreetAddress = "StreetAddress";
            shoppingCartVM.Order.State = "State";

            shoppingCartVM.Order.OrderTotal = GetTotalPriceOnCart(cartList);
            if(shoppingCartVM.Order.OrderTotal < SD.MinimumOrderAmountVND)
            {
                TempData["error"] = $"The total order value must be at least {SD.MinimumOrderAmountVND} VND to proceed with payment via Stripe.";
                return RedirectToAction(nameof(Index));
            }
            return View(shoppingCartVM);
        }
        [HttpPost]
        [ActionName("Checkout")]
        public IActionResult CheckoutPOST()
        {            
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"); ;
          
            ShoppingCartVM.Order.CreatedAt = DateTime.Now;
            ShoppingCartVM.Order.ApplicationUserId = userId;

            ShoppingCartVM.Order.OrderTotal = GetTotalPriceOnCart(ShoppingCartVM.ShoppingCartList);
            ShoppingCartVM.Order.PaymentStatus = SD.PaymentStatusPending;
            ShoppingCartVM.Order.OrderStatus = SD.StatusPending;
            
            _unitOfWork.Order.Add(ShoppingCartVM.Order);
            _unitOfWork.Save();
            foreach(var cart in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderId = ShoppingCartVM.Order.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }
            var domain = "https://localhost:7052";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"/Customer/Cart/OrderConfirmation?id={ShoppingCartVM.Order.Id}",
                CancelUrl = domain + "/Customer/Cart/Cancel",
            };
            foreach(var item in ShoppingCartVM.ShoppingCartList)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price),
                        Currency = "vnd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title
                        }
                    },
                    Quantity = item.Count
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);
            _unitOfWork.Order.UpdateStripePaymentID(ShoppingCartVM.Order.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
        public IActionResult OrderConfirmation(int id)
        {
            Order order = _unitOfWork.Order.Get(u => u.Id == id, includeProperties: "ApplicationUser");
            if(order.PaymentStatus!=SD.PaymentStatusDelayedPayment)
            {

            }

            var service = new SessionService();
            Session session = service.Get(order.SessionId);
            if(session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.Order.UpdateStripePaymentID(id, session.Id, session.PaymentIntentId);
                _unitOfWork.Order.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                _unitOfWork.Save();
            }
            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == order.ApplicationUserId).ToList();
            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();            
            return View(id);
        }
    }
}
