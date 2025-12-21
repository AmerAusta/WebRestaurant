using DataAccessLayer.MenuItems;
using DataAccessLayer.Order;
using DataAccessLayer.OrderItem;
using DataAccessLayer.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RestaurantWebApi.Controllers.Order
{
    [Route("api/Restaurant")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        [HttpGet("GetOrdersByOrderID/{OrderId}", Name = "GetOrdersByOrderID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles ="Admin")]
        public ActionResult GetOrdersByOrderID([FromRoute] int OrderId)
        {


            if (OrderId <= 0)
                return BadRequest("Not Accepted CustomerID");

            BusinessLayer.Order.Order Order = BusinessLayer.Order.Order.Find(OrderId);
            if (Order == null)
                return NotFound($"Order With ID={OrderId} Not Have Any Order");

            return Ok(Order.ODTO);
        }

        [HttpPost("AddOrder", Name = "AddOrder")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        public ActionResult AddOrder([FromBody] AddOrderDTO AddOrderDTO)
        {
            if (AddOrderDTO == null || AddOrderDTO.CustomerId < 0
                || AddOrderDTO.OrderItems == null)
            {
                return BadRequest("Invalid Order Data");
            }
            if (AddOrderDTO.ReservationId <= 0 || AddOrderDTO.ReservationId == null)
            {
                AddOrderDTO.ReservationId = null;
            }

            List<OrderItemDTO> orderItemDTOs = new();
            foreach (var item in AddOrderDTO.OrderItems)
            {
                var menuItem = BusinessLayer.MenuItems.MenuItems.Find(item.MenuItemId);
                if (menuItem == null)
                    return BadRequest($"MenuItemId {item.MenuItemId} not found");

                orderItemDTOs.Add(new OrderItemDTO(
                    0,
                    0,
                    item.MenuItemId,
                    item.Quantity,
                    menuItem.Price,
                    item.Quantity * menuItem.Price
                ));
            }


            OrderDTO NewOrderDTO = new OrderDTO(
                0,
                AddOrderDTO.CustomerId,
                OrderDTO.enOrderStatusLookup.Pending,
                0,
                DateTime.Now,
                AddOrderDTO.ReservationId,
                null,
                orderItemDTOs
            );

            BusinessLayer.Order.Order Order = new BusinessLayer.Order.Order(NewOrderDTO);
            if (!Order.Save())
                return StatusCode(500, new { Message = "Error adding Order" });

            Order.CalculateTotalsAndExpiry();

            if (!Order.Save())
                return StatusCode(500, new { Message = "Error Update Order" });

            return StatusCode(StatusCodes.Status201Created, new { Message = "Succeed" });

        }

        [HttpGet("GetCustomerOrders/{customerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = "AdminOrSameUser")]
        public ActionResult GetCustomerOrders(int customerId)
        {
            if (customerId <= 0)
                return BadRequest("Invalid CustomerId");

            var orders =
                BusinessLayer.Order.Order.GetOrdersWithItemsByCustomerID(customerId);

            if (orders.Count == 0)
                return NotFound("No orders found for this customer");

            return Ok(orders);
        }

        [HttpGet("GetAllOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles= "Admin")]
        public ActionResult GetAllOrders()
        {

            var orders =
                BusinessLayer.Order.Order.GetAllOrders();

            if (orders.Count == 0)
                return NotFound("No orders found for this customer");

            return Ok(orders);
        }

    }
}
