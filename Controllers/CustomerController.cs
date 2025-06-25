
using Day6.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace Day6.Controllers
{
    public class CustomerController : Controller
    {
        string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(Customer customer)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("InsertCustomer", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", customer.Name);
                cmd.Parameters.AddWithValue("@Email", customer.Email);
                cmd.Parameters.AddWithValue("@Mobile", customer.Mobile);
                cmd.Parameters.AddWithValue("@Gender", customer.Gender);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            TempData["Success"] = "Added to Cart";
            return RedirectToAction("Login");
        }

        public ActionResult Login()
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult Login(string email)
        //{
        //    using (SqlConnection con = new SqlConnection(cs))
        //    {
        //        SqlCommand cmd = new SqlCommand("GetCustomerByEmail", con);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@Email", email);
        //        con.Open();
        //        SqlDataReader rdr = cmd.ExecuteReader();
        //        if (rdr.Read())
        //        {
        //            Session["CustomerId"] = rdr["Id"];
        //            Session["CustomerName"] = rdr["Name"];
        //            return RedirectToAction("Index", "Product");
        //        }
        //    }

        //    ViewBag.Error = "Invalid email.";
        //    return View();
        //}
        [HttpPost]
        public ActionResult Login(string email)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("GetCustomerByEmail", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email", email);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    var customer = new Customer
                    {
                        Id = (int)rdr["Id"],
                        Name = rdr["Name"].ToString(),
                        Email = rdr["Email"].ToString(),
                        Mobile = rdr["Mobile"].ToString(),
                        Gender = rdr["Gender"].ToString()
                    };

                    Session["customer"] = customer; //  this is what OrderController expects
                    Session["CustomerId"] = customer.Id;
                    Session["CustomerName"] = customer.Name;

                    return RedirectToAction("Index", "Product");
                }
            }

            ViewBag.Error = "Invalid email.";
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
