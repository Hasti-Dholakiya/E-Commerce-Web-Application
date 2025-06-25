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
    public class OrderDAL
    {
        string conStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public int PlaceOrder(int customerId, List<CartItem> cart)
        {
            decimal total = cart.Sum(x => x.TotalPrice);
            int orderId = 0;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    // Insert Order
                    SqlCommand cmd = new SqlCommand("InsertOrder", con, tran);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CustomerId", customerId);
                    cmd.Parameters.AddWithValue("@Total", total);
                    SqlParameter outParam = new SqlParameter("@OrderId", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outParam);
                    cmd.ExecuteNonQuery();
                    orderId = (int)outParam.Value;

                    // Insert each item
                    foreach (var item in cart)
                    {
                        SqlCommand itemCmd = new SqlCommand("InsertOrderItem", con, tran);
                        itemCmd.CommandType = CommandType.StoredProcedure;
                        itemCmd.Parameters.AddWithValue("@OrderId", orderId);
                        itemCmd.Parameters.AddWithValue("@ProductId", item.ProductId);
                        itemCmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                        itemCmd.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
                        itemCmd.ExecuteNonQuery();
                    }

                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }

            return orderId;
        }

        public List<Order> GetOrders(int? customerId = null)
        {
            List<Order> list = new List<Order>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetOrdersByCustomer", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CustomerId", (object)customerId ?? DBNull.Value);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new Order
                    {
                        Id = (int)dr["Id"],
                        CustomerId = (int)dr["CustomerId"],
                        OrderDate = (DateTime)dr["OrderDate"],
                        Total = (decimal)dr["Total"],
                        Status = dr["Status"].ToString(),
                        CustomerName = dr["CustomerName"].ToString()
                    });
                }
            }
            return list;
        }

        public List<OrderItem> GetOrderItems(int orderId)
        {
            List<OrderItem> list = new List<OrderItem>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetOrderItems", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new OrderItem
                    {
                        Id = (int)dr["Id"],
                        OrderId = (int)dr["OrderId"],
                        ProductId = (int)dr["ProductId"],
                        Quantity = (int)dr["Quantity"],
                        UnitPrice = (decimal)dr["UnitPrice"],
                        ProductName = dr["ProductName"].ToString()
                    });
                }
            }
            return list;
        }
        public void UpdateOrderStatus(int orderId, string status)
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("UPDATE [Order] SET Status = @Status WHERE Id = @OrderId", con);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}