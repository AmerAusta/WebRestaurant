using DataAccessLayer.Order;
using DataAccessLayer.OrderItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Order
{
    public class Order
    {
        public enum enMode { AddNew = 0, Update = 1 };
        enMode Mode = enMode.AddNew;
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public OrderDTO.enOrderStatusLookup StatusId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ReservationId { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public  List<OrderItemDTO> OrderItem { get; set; }

        public OrderDTO ODTO {
            get { return new OrderDTO(OrderId, CustomerId, StatusId, TotalAmount, CreatedAt, ReservationId, ExpiryDate,OrderItem); }
        }

        public Order(OrderDTO orderDTO,enMode oMode=enMode.AddNew)
        {
            OrderId = orderDTO.OrderId;
            CustomerId = orderDTO.CustomerId;
            StatusId = orderDTO.StatusId;
            TotalAmount = orderDTO.TotalAmount;
            CreatedAt = orderDTO.CreatedAt;
            ReservationId = orderDTO.ReservationId;
            ExpiryDate = orderDTO.ExpiryDate;
            OrderItem = orderDTO.OrderItem ?? new List<OrderItemDTO>();
            Mode =oMode;
        }

        public static Order Find(int OrderID)
        {
            OrderDTO ODTO = OrderData.GetOrderByID(OrderID);
            if (ODTO != null)
            {
                return new Order(ODTO, enMode.Update);
            }
            else
            {
                return null;
            }
        }

        public static List<GetOrderDTO> GetOrdersWithItemsByCustomerID(int customerId)
        {
            List<OrderDTO> orders = OrderData.GetOrdersByCustomerID(customerId);

            if (orders == null || orders.Count == 0)
                return new List<GetOrderDTO>();

            List<GetOrderDTO> result = new();

            foreach (var order in orders)
            {
                List<GetOrderItemDTO> items =
                    OrderItemData.GetOrderItems(order.OrderId)
                    ?? new List<GetOrderItemDTO>();

                result.Add(new GetOrderDTO(order, items));
            }

            return result;
        }

        public static List<GetOrderDTO> GetAllOrders()
        {
            List<OrderDTO> orders = OrderData.GetAllOrders();

            if (orders == null || orders.Count == 0)
                return new List<GetOrderDTO>();

            List<GetOrderDTO> result = new();

            foreach (var order in orders)
            {
                List<GetOrderItemDTO> items =
                    OrderItemData.GetOrderItems(order.OrderId)
                    ?? new List<GetOrderItemDTO>();

                result.Add(new GetOrderDTO(order, items));
            }

            return result;
        }

        public void CalculateTotalsAndExpiry()
        {
            TotalAmount = OrderItem.Sum(x => x.PriceTotal);
            if (ReservationId == null)
            {
                int maxPrepTime = OrderItem
                    .Select(x => BusinessLayer.MenuItems.MenuItems.Find(x.MenuItemId)?.PerparationTime ?? 0)
                    .Max();

                ExpiryDate = CreatedAt.AddMinutes(maxPrepTime);
            }
            else
            {
               ExpiryDate=BusinessLayer.Reservations.Reservations.GetReservationByReservationID(ReservationId.Value).ReservationDate;

            }
        }

        private bool _AddNewOrderWithItem()
        {

            OrderId = OrderData.AddOrderWithItems(ODTO);
            return OrderId != -1;
        }

        private bool _UpdateOrder()
        {
            return OrderData.UpdateOrder(ODTO);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewOrderWithItem())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                        return false;
                case enMode.Update:
                    return _UpdateOrder();
            }
            return false;
        }

        public static bool CancelleOrder(int OrderID)
        {
            return OrderData.CancelledOrderByOrderID(OrderID);
        }
    }
}
