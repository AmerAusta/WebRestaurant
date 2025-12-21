using DataAccessLayer.OrderItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.OrderItem
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int MenuItemId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal PriceTotal { get; set; }

        public OrderItemDTO ODTO
        {
            get { return new OrderItemDTO(OrderItemId, OrderId, MenuItemId, Quantity, UnitPrice, PriceTotal); }
        }

        public OrderItem(OrderItemDTO orderItemDTO)
        {
            OrderItemId=orderItemDTO.OrderItemId;
            OrderId=orderItemDTO.OrderId;
            MenuItemId=orderItemDTO.MenuItemId;
            Quantity=orderItemDTO.Quantity;
            UnitPrice=orderItemDTO.UnitPrice;
            PriceTotal=orderItemDTO.PriceTotal;
        }

        //public int AddNewOrderItem()
        //{
        //    OrderItemId = OrderItemData.AddNewOredrItem(ODTO);
        //    return OrderItemId;
        //}
    }
}
