using Day6.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Day6.DAL
{
    public class ProductDAL
    {
        string conStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public Product GetProductById(int id)
        {
            Product p = null;
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetProductById", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    p = new Product
                    {
                        Id = (int)dr["Id"],
                        Name = dr["Name"].ToString(),
                        Price = (decimal)dr["Price"],
                        Stock = (int)dr["Stock"]
                    };
                }
            }
            return p;
        }
    }
}