using DataAccessLayer.Globale;
using DataAccessLayer.MenuItems;
using DataAccessLayer.OrderItem;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.OrderItem
{
    public class OrderItemDTO
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int MenuItemId {  get; set; }
        public int Quantity {  get; set; }
        public decimal UnitPrice { get; set; }
        public decimal PriceTotal { get; set; }

        public OrderItemDTO(int orderItemId, int orderId, int menuItemId, int quantity, decimal unitPrice, decimal priceTotal)
        {
            OrderItemId = orderItemId;
            OrderId = orderId;
            MenuItemId = menuItemId;
            Quantity = quantity;
            UnitPrice = unitPrice;
            PriceTotal = priceTotal;
        }
    }

    public class AddOrderItemDTO
    {
        public int MenuItemId { get; set; }
        public int Quantity { get; set; }

        public AddOrderItemDTO(int menuItemId, int quantity)
        {
            MenuItemId = menuItemId;
            Quantity = quantity;
        }
    }

    public class GetOrderItemDTO
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal PriceTotal { get; set; }

        public GetOrderItemDTO(string name, int quantity, decimal unitPrice, decimal priceTotal)
        {
            Name = name;
            Quantity = quantity;
            UnitPrice = unitPrice;
            PriceTotal = priceTotal;
        }
    }

    public class OrderItemData
    {
        public static List<GetOrderItemDTO> GetOrderItems(int OrderId)
        {
            var list = new List<GetOrderItemDTO>();
            using (SqlConnection conn = new SqlConnection(ConnectionString._connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetOrderItemsByOrderID", conn))
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OrderId", OrderId);

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var DTO = new GetOrderItemDTO(
                                    reader.GetString(reader.GetOrdinal("Name")),
                                    reader.GetInt16(reader.GetOrdinal("Quantity")),
                                    reader.GetDecimal(reader.GetOrdinal("UnitPrice")),
                                    reader.GetDecimal(reader.GetOrdinal("PriceTotal"))

                                    );
                                list.Add(DTO);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Get OrderItems failed: {ex.Message}");
                        return null;
                    }
                }

            }
            return list;
        }

    }
}
