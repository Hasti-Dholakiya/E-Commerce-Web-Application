using Day6.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace Day6.Controllers
{
    public class CategoryController : Controller
    {
        string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public ActionResult Index()
        {
            List<Category> list = new List<Category>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("GetAllCategories", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(new Category
                    {
                        CategoryId = (int)rdr["CategoryId"],
                        Name = rdr["Name"].ToString()
                    });
                }
            }
            return View(list);
        }

        public ActionResult Create() => View();

        [HttpPost]
        public ActionResult Create(Category cat)
        {

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("InsertCategory", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", cat.Name);
                con.Open();
                cmd.ExecuteNonQuery();
            }
            TempData["msg"] = "Category added successfully!";
            return RedirectToAction("Index");
            //  }
            return View(cat);
        }

        public ActionResult Edit(int id)
        {
            Category cat = new Category();
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("GetCategoryById", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CategoryId", id);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    cat.CategoryId = (int)rdr["CategoryId"];
                    cat.Name = rdr["Name"].ToString();
                }
            }
            return View(cat);
        }

        [HttpPost]
        public ActionResult Edit(Category cat)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    SqlCommand cmd = new SqlCommand("UpdateCategory", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CategoryId", cat.CategoryId);
                    cmd.Parameters.AddWithValue("@Name", cat.Name);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                TempData["msg"] = "Category updated successfully!";
                return RedirectToAction("Index");
            }
            return View(cat);
        }

        public ActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("DeleteCategory", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CategoryId", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
            TempData["msg"] = "Category deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}