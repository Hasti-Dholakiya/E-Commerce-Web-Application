using Day6.DAL;
using Day6.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Day6.Controllers
{
    public class CartController : Controller
    {
        ProductDAL productDAL = new ProductDAL();

        public ActionResult Index()
        {
            List<CartItem> cart = Session["cart"] as List<CartItem> ?? new List<CartItem>();
            ViewBag.TotalQuantity = cart.Sum(x => x.Quantity);
            ViewBag.TotalAmount = cart.Sum(x => x.TotalPrice);
            return View(cart);
        }

        public ActionResult AddToCart(int id)
        {
            Product p = productDAL.GetProductById(id);
            if (p == null) return HttpNotFound();
            List<CartItem> cart = Session["cart"] as List<CartItem> ?? new List<CartItem>();
            CartItem existing = cart.FirstOrDefault(x => x.ProductId == id);

            if (existing != null)
                existing.Quantity++;
            else
                cart.Add(new CartItem
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    UnitPrice = (decimal)p.Price,
                    Quantity = 1
                });

            Session["cart"] = cart;
            TempData["msg"] = "Product added to cart.";
            return RedirectToAction("Index", "Product");
        }

        public ActionResult Remove(int id)
        {
            List<CartItem> cart = Session["cart"] as List<CartItem>;
            if (cart != null)
            {
                CartItem item = cart.FirstOrDefault(x => x.ProductId == id);
                if (item != null)
                    cart.Remove(item);
                Session["cart"] = cart;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UpdateQuantity(int id, int quantity)
        {
            List<CartItem> cart = Session["cart"] as List<CartItem>;
            if (cart != null)
            {
                CartItem item = cart.FirstOrDefault(x => x.ProductId == id);
                if (item != null)
                {
                    if (quantity > 0)
                        item.Quantity = quantity;
                    else
                        cart.Remove(item);
                }
                Session["cart"] = cart;
            }
            return RedirectToAction("Index");
        }

    }
}









