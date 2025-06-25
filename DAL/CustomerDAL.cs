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
    public class CustomerDAL
    {
        string conStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public void InsertCustomer(Customer c)
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("InsertCustomer", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", c.Name);
                cmd.Parameters.AddWithValue("@Email", c.Email);
                cmd.Parameters.AddWithValue("@Mobile", c.Mobile);
                cmd.Parameters.AddWithValue("@Gender", c.Gender);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<Customer> GetAllCustomers()
        {
            List<Customer> list = new List<Customer>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllCustomers", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new Customer
                    {
                        Id = (int)dr["Id"],
                        Name = dr["Name"].ToString(),
                        Email = dr["Email"].ToString(),
                        Mobile = dr["Mobile"].ToString(),
                        Gender = dr["Gender"].ToString()
                    });
                }
            }
            return list;
        }

        public Customer GetCustomerByEmail(string email)
        {
            Customer c = null;
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetCustomerByEmail", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email", email);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    c = new Customer
                    {
                        Id = (int)dr["Id"],
                        Name = dr["Name"].ToString(),
                        Email = dr["Email"].ToString(),
                        Mobile = dr["Mobile"].ToString(),
                        Gender = dr["Gender"].ToString()
                    };
                }
            }
            return c;
        }
    }
}