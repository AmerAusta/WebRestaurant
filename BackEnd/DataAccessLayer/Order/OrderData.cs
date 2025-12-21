using DataAccessLayer.Globale;
using DataAccessLayer.OrderItem;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Order
{
    public class OrderDTO
    {
        public enum enOrderStatusLookup { Pending = 1, Cancelled = 2, Completed = 3}
        public int OrderId { get; set; }
        public int CustomerId {  get; set; }
        public enOrderStatusLookup StatusId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ReservationId {  get; set; }
        public DateTime? ExpiryDate { get; set; }
        public List< OrderItemDTO>? OrderItem {  get; set; }
        public OrderDTO(int orderId, int customerId, enOrderStatusLookup statusId, decimal totalAmount, 
            DateTime createdAt, int? reservationId, DateTime? expiryDate, List<OrderItemDTO> orderItem=null)
        {
            OrderId = orderId;
            CustomerId = customerId;
            StatusId = statusId;
            TotalAmount = totalAmount;
            CreatedAt = createdAt;
            ReservationId = reservationId;
            ExpiryDate = expiryDate;
            OrderItem = orderItem;
        }


    }
    
    public class AddOrderDTO
    {
        public int CustomerId { get; set; }
        public int? ReservationId { get; set; }
        public List<AddOrderItemDTO> OrderItems { get; set; }

        public AddOrderDTO( int customerId, List<AddOrderItemDTO> orderItems, int? reservationId)
        {
            CustomerId = customerId;
            OrderItems = orderItems;
            ReservationId = reservationId;
        }
    }

    public class GetOrderDTO
    {
        public OrderDTO Order { get; set; }
        public List<GetOrderItemDTO> Items { get; set; }

        public GetOrderDTO(OrderDTO order, List<GetOrderItemDTO> items)
        {
            Order = order;
            Items = items;
        }
    }


    public class OrderData
    {
        public static OrderDTO GetOrderByID(int OrderId)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString._connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetOrderByID", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@OrderId", OrderId);

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new OrderDTO(
                                    reader.GetInt32(reader.GetOrdinal("OrderId")),
                                    reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                    (OrderDTO.enOrderStatusLookup)reader.GetByte(reader.GetOrdinal("StatusId")),
                                    reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                                    reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                    reader.IsDBNull(reader.GetOrdinal("ReservationId")) ? null : 
                                    reader.GetInt32(reader.GetOrdinal("ReservationId")),
                                    reader.IsDBNull(reader.GetOrdinal("ExpiryDate")) ? null : 
                                    reader.GetDateTime(reader.GetOrdinal("ExpiryDate"))
                                    );
                            }
                            else
                                return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Get Order By OrderID failed: {ex.Message}");
                        return null;
                    }
                }
            }
        }

        public static List<OrderDTO> GetOrdersByCustomerID(int customerId)
        {
            List<OrderDTO> list = new();

            using (SqlConnection conn = new SqlConnection(ConnectionString._connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetOrderByCustomerID", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@CustomerId", customerId);

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new OrderDTO(
                                  reader.GetInt32(reader.GetOrdinal("OrderId")),
                                  reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                  (OrderDTO.enOrderStatusLookup)reader.GetByte(reader.GetOrdinal("StatusId")),
                                  reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                                  reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                  reader.IsDBNull(reader.GetOrdinal("ReservationId")) ? null :
                                  reader.GetInt32(reader.GetOrdinal("ReservationId")),
                                  reader.IsDBNull(reader.GetOrdinal("ExpiryDate")) ? null :
                                  reader.GetDateTime(reader.GetOrdinal("ExpiryDate"))
                                ));
                            }
                        }
                        return list;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Get Order By CustomerID failed: {ex.Message}");
                        return null;
                    }
                }
            }
        }

        public static List<OrderDTO> GetAllOrders()
        {
            List<OrderDTO> list = new();

            using (SqlConnection conn = new SqlConnection(ConnectionString._connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllOrder", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new OrderDTO(
                                  reader.GetInt32(reader.GetOrdinal("OrderId")),
                                  reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                  (OrderDTO.enOrderStatusLookup)reader.GetByte(reader.GetOrdinal("StatusId")),
                                  reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                                  reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                  reader.IsDBNull(reader.GetOrdinal("ReservationId")) ? null :
                                  reader.GetInt32(reader.GetOrdinal("ReservationId")),
                                  reader.IsDBNull(reader.GetOrdinal("ExpiryDate")) ? null :
                                  reader.GetDateTime(reader.GetOrdinal("ExpiryDate"))
                                ));
                            }
                        }
                        return list;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Get Order By CustomerID failed: {ex.Message}");
                        return null;
                    }
                }
            }
        }

        public static OrderDTO GetOrderByReservationID(int OrderId)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString._connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetOrderByReservationID", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ReservationId", OrderId);

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new OrderDTO(
                                    reader.GetInt32(reader.GetOrdinal("OrderId")),
                                    reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                    (OrderDTO.enOrderStatusLookup)reader.GetByte(reader.GetOrdinal("StatusId")),
                                    reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                                    reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                    reader.IsDBNull(reader.GetOrdinal("ReservationId")) ? null :
                                    reader.GetInt32(reader.GetOrdinal("ReservationId")),
                                    reader.IsDBNull(reader.GetOrdinal("ExpiryDate")) ? null :
                                    reader.GetDateTime(reader.GetOrdinal("ExpiryDate"))
                                    );
                            }
                            else
                                return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Get Order By ReservationID failed: {ex.Message}");
                        return null;
                    }
                }
            }
        }

        public static bool UpdateOrder(OrderDTO OrderDTO)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString._connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_UpdateOrder", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@TotalAmount", OrderDTO.TotalAmount);
                    cmd.Parameters.AddWithValue("@ExpiryDate", OrderDTO.ExpiryDate);
                    cmd.Parameters.AddWithValue("@OrderId", OrderDTO.OrderId);

                    try
                    {
                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Update Order failed: {ex.Message}");
                        return false;
                    }
                }
            }
        }

        public static bool CancelledOrderByOrderID(int OrderID)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString._connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_CancelledOrderByOrderID", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@OrderID", OrderID);

                    try
                    {
                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Cancelled Order failed: {ex.Message}");
                        return false;
                    }
                }
            }
        }

        public static bool CancelledOrderByReservationID(int ReservationID)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString._connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_CancelledOrderByReservationID", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ReservationId", ReservationID);

                    try
                    {
                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Cancelled Order failed: {ex.Message}");
                        return false;
                    }
                }
            }
        }

        public static int AddOrderWithItems(OrderDTO orderDTO)
        {
            int orderId = -1;

            using (SqlConnection conn = new SqlConnection(ConnectionString._connectionString))
            {
                conn.Open();

                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    //إضافة Order
                    SqlCommand cmdOrder = new SqlCommand(
                        "SP_AddNewOrder",
                        conn,
                        transaction
                    );

                    cmdOrder.CommandType = CommandType.StoredProcedure;

                    cmdOrder.Parameters.AddWithValue("@CustomerId", orderDTO.CustomerId);
                    cmdOrder.Parameters.AddWithValue("@StatusId", orderDTO.StatusId);
                    cmdOrder.Parameters.AddWithValue("@CreatedAt", orderDTO.CreatedAt);
                    cmdOrder.Parameters.AddWithValue("@ReservationId", (object?)orderDTO.ReservationId ?? DBNull.Value);

                    SqlParameter outputId = new SqlParameter("@OrderId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };

                    cmdOrder.Parameters.Add(outputId);
                    cmdOrder.ExecuteNonQuery();

                    orderId = (int)outputId.Value;


                    // إضافة OrderItems
                    foreach (var item in orderDTO.OrderItem)
                    {
                        SqlCommand cmdItem = new SqlCommand(
                            "SP_AddNewOrderItem",
                            conn,
                            transaction
                        );

                        cmdItem.CommandType = CommandType.StoredProcedure;

                        cmdItem.Parameters.AddWithValue("@OrderId", orderId);
                        cmdItem.Parameters.AddWithValue("@MenuItemId", item.MenuItemId);
                        cmdItem.Parameters.AddWithValue("@Quantity", item.Quantity);
                        cmdItem.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);

                        SqlParameter outputItemId = new SqlParameter("@OrderItemId", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmdItem.Parameters.Add(outputItemId);


                        cmdItem.ExecuteNonQuery();
                    }


                    transaction.Commit();
                    return orderId;
                }
                catch(Exception ex) 
                {
                    transaction.Rollback();
                    Console.WriteLine("AddOrderWithItems ERROR:");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    throw;
                }
            }
        }
    }
}
