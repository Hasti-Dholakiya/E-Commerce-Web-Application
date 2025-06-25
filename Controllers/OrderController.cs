using Day6.DAL;
using Day6.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Day6.Controllers
{
    [RoutePrefix("order")]
    public class OrderController : Controller
    {
        OrderDAL orderDAL = new OrderDAL();

        [Route("place")]
        public ActionResult PlaceOrder()

        {
            var customer = (Customer)Session["customer"];
            var cart = (List<CartItem>)Session["cart"];
            if (customer == null || cart == null || cart.Count == 0)
                return RedirectToAction("Index", "Home");

            int orderId = orderDAL.PlaceOrder(customer.Id, cart);
            Session["cart"] = null;

            return RedirectToAction("Details", "Order", new { id = orderId });
        }

        [Route("details/{id}")]
        public ActionResult Details(int id)
        {
            var orderItems = orderDAL.GetOrderItems(id);
            var order = orderDAL.GetOrders().FirstOrDefault(o => o.Id == id);
            order.OrderItems = orderItems;

            return View(order);
        }


        [HttpPost]
        [Route("complete/{id}")]
        public ActionResult Complete(int id)
        {
            var order = orderDAL.GetOrders().FirstOrDefault(o => o.Id == id);
            if (order == null)
                return HttpNotFound();

            order.Status = "Completed";
            orderDAL.UpdateOrderStatus(id, "Completed");

            order.OrderItems = orderDAL.GetOrderItems(id);
            GenerateOrderPdf(order);

            return RedirectToAction("List");
        }

        [Route("list")]
        public ActionResult List(int? customerId)
        {
            var orders = orderDAL.GetOrders(customerId);
            ViewBag.Customers = new SelectList(new CustomerDAL().GetAllCustomers(), "Id", "Name", customerId);
            return View(orders);
        }

        private void GenerateOrderPdf(Order order)
        {
            string path = Server.MapPath("~/Content/PDFs/");
            Directory.CreateDirectory(path);

            string filePath = Path.Combine(path, $"Order_{order.Id}.pdf");

            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                Document doc = new Document();
                PdfWriter.GetInstance(doc, fs);
                doc.Open();

                doc.Add(new Paragraph($"Order ID: {order.Id}"));
                doc.Add(new Paragraph($"Customer: {order.CustomerName}"));
                doc.Add(new Paragraph($"Date: {order.OrderDate}"));
                doc.Add(new Paragraph($"Total: {order.Total:C}"));
                doc.Add(new Paragraph($"Status: {order.Status}"));
                doc.Add(new Paragraph(""));

                doc.Add(new Paragraph("Items:"));
                foreach (var item in order.OrderItems)
                {
                    doc.Add(new Paragraph($"- {item.ProductName} x {item.Quantity} @ {item.UnitPrice:C}"));
                }

                doc.Close();
            }
        }
        [Route("download/{id}")]
        public ActionResult DownloadInvoice(int id)
        {
            string fileName = $"Order_{id}.pdf";
            string path = Server.MapPath("~/Content/PDFs/" + fileName);

            if (!System.IO.File.Exists(path))
                return HttpNotFound("Invoice not found. Please mark the order as completed first.");

            return File(path, "application/pdf", fileName);
        }

    }
}
