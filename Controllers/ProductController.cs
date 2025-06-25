using Day6.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Day6.Controllers
{
    public class ProductController : Controller
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public ActionResult Create()
        {
            ViewBag.CategoryList = GetCategoryList();
            return View();
        }
        public ActionResult Index()
        {
            var products = new List<Product>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("GetAllProducts", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    products.Add(new Product
                    {
                        Id = (int)rdr["ProductId"],
                        Name = rdr["Name"].ToString(),
                        Price = (decimal)rdr["Price"],
                        Stock = (int)rdr["Stock"],
                        Description = rdr["Description"].ToString(),
                        CategoryName = rdr["Name"].ToString()
                    });
                }
            }
            return View(products);
        }
        [HttpPost]
        public ActionResult Create(Product product)
        {
            if (ModelState.IsValid)
           {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("AddProduct", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Name", product.Name);
                    cmd.Parameters.AddWithValue("@Price", product.Price);
                    cmd.Parameters.AddWithValue("@Stock", product.Stock);
                    cmd.Parameters.AddWithValue("@Description", product.Description);
                    cmd.Parameters.AddWithValue("@CategoryId", product.CategoryId);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

                TempData["Success"] = "Product created successfully!";
                return RedirectToAction("Index");
            }

            ViewBag.CategoryList = GetCategoryList();
            return View(product);
        }
        public ActionResult Edit(int id)
        {
            Product product = null;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("GetProductById", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProductId", id);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    product = new Product
                    {
                        Id = Convert.ToInt32(dr["ProductId"]),
                        Name = dr["Name"].ToString(),
                        Price = Convert.ToDecimal(dr["Price"]),
                        Stock = Convert.ToInt32(dr["Stock"]),
                        Description = dr["Description"].ToString(),
                        CategoryId = Convert.ToInt32(dr["CategoryId"])
                    };
                }
                con.Close();
            }

            if (product == null)
                return HttpNotFound();

            ViewBag.Categories = GetCategoryList();
            return View(product);
        }

        [HttpPost]
        public ActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("UpdateProduct", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ProductId", product.Id);
                    cmd.Parameters.AddWithValue("@Name", product.Name);
                    cmd.Parameters.AddWithValue("@Price", product.Price);
                    cmd.Parameters.AddWithValue("@Stock", product.Stock);
                    cmd.Parameters.AddWithValue("@Description", product.Description);
                    cmd.Parameters.AddWithValue("@CategoryId", product.CategoryId);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

                TempData["Success"] = "Product updated successfully!";
                return RedirectToAction("Index");
            }

            ViewBag.Categories = GetCategoryList();
            return View(product);
        }
        public ActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("DeleteProduct", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProductId", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
            TempData["msg"] = "Product deleted!";
            return RedirectToAction("Index");
        }

        public ActionResult ShowByCategory()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("GetAllCategories", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                List<Category> catList = new List<Category>();
                while (rdr.Read())
                {
                    catList.Add(new Category
                    {
                        CategoryId = (int)rdr["CategoryId"],
                        Name = rdr["Name"].ToString()
                    });
                }
                ViewData["Categories"] = catList;
            }
            return View();
        }

        public PartialViewResult ProductList(int categoryId)
        {
            List<Product> list = new List<Product>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("GetProductsByCategoryId", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CategoryId", categoryId);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(new Product
                    {
                        Id = (int)rdr["ProductId"],
                        Name = rdr["Name"].ToString(),
                        Price = (decimal)rdr["Price"],
                        Stock = (int)rdr["Stock"]
                    });
                }
            }
            return PartialView("_ProductList", list);
        }

        private List<SelectListItem> GetCategoryList()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("GetAllCategories", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new SelectListItem
                    {
                        Value = reader["CategoryId"].ToString(),
                        Text = reader["Name"].ToString()
                    });
                }

                con.Close();
            }

            return list;
        }
        [HttpPost]
        public ActionResult AddToCart(int productId)
        {
            if (Session["CustomerId"] == null)
                return RedirectToAction("Login", "Customer");

            List<CartItem> cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();
            CartItem existingItem = cart.FirstOrDefault(x => x.ProductId == productId);

            if (existingItem != null)
                existingItem.Quantity++;
            else
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("GetProductById", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProductId", productId);
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        cart.Add(new CartItem
                        {
                            ProductId = productId,
                            ProductName = rdr["Name"].ToString(),
                            UnitPrice = (decimal)rdr["Price"],
                            Quantity = 1
                        });
                    }
                }
            }

            Session["Cart"] = cart;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UpdateCart(int productId, int quantity)
        {
            List<CartItem> cart = Session["Cart"] as List<CartItem>;
            var item = cart.FirstOrDefault(x => x.ProductId == productId);
            if (item != null && quantity > 0)
                item.Quantity = quantity;

            Session["Cart"] = cart;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult PlaceOrder()
        {
            if (Session["CustomerId"] == null || Session["Cart"] == null)
                return RedirectToAction("Login", "Customer");

            int customerId = (int)Session["CustomerId"];
            Cart cart = Session["Cart"] as Cart;

            if (cart == null || cart.Items == null || !cart.Items.Any())
                return RedirectToAction("Index", "Cart"); 

            decimal total = cart.CartTotal;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("InsertOrder", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CustomerId", customerId);
                cmd.Parameters.AddWithValue("@Total", total);
                SqlParameter outParam = new SqlParameter("@OrderId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outParam);

                con.Open();
                cmd.ExecuteNonQuery();
                int orderId = (int)outParam.Value;

                foreach (var item in cart.Items)
                {
                    SqlCommand itemCmd = new SqlCommand("InsertOrderItem", con);
                    itemCmd.CommandType = CommandType.StoredProcedure;
                    itemCmd.Parameters.AddWithValue("@OrderId", orderId);
                    itemCmd.Parameters.AddWithValue("@ProductId", item.ProductId);
                    itemCmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                    itemCmd.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
                    itemCmd.ExecuteNonQuery();
                }
            }

            Session["Cart"] = null;
            TempData["Success"] = "Order placed successfully!";
            return RedirectToAction("Index");
        }

    }
}