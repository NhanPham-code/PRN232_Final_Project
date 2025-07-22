using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using OrderAPI.Data;
using OrderAPI.DTOs;
using OrderAPI.Models;
using OrderAPI.Repositories.Interfaces;
using OrderAPI.Services.Interfaces;

namespace OrderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("Queryable")]
        [EnableQuery]
        public IActionResult GetOrdersQueryable()
        {
            var orders = _orderService.GetAllOrderQueryable();
            if (orders == null || !orders.Any())
            {
                return Ok(Enumerable.Empty<ReadOrderDTO>().AsQueryable());
            }
            return Ok(orders);
        }

        // GET: api/Orders/5
        /*        [HttpGet("{id}")]
                public async Task<ActionResult<ReadOrderDTO>> GetOrder(int id)
                {
                    var orders = await _orderService.GetOrderByUserIdAsync(id);

                    if (orders == null)
                    {
                        return NotFound();
                    }

                    return Ok(orders);
                }*/

        // Method to get orders by user ID with queryable
        // GET: api/Orders/5
        /*[HttpGet("{userId}")]
*//*        [EnableQuery(PageSize = 8)]
*//*        public IQueryable<ReadOrderDTO> GetOrdersByUserIdQueryable(int userId,
            [FromQuery] bool lastest = true)
        {
            var orders = _orderService.GetOrderByUserIdQueryable(userId);
            if (orders == null || !orders.Any())
            {
                return Enumerable.Empty<ReadOrderDTO>().AsQueryable();
            }

            // order by order id descending if lastest is true
            if (lastest)
            {
                orders = orders.OrderByDescending(o => o.OrderID);
            }
            return orders;
        }*/

        [HttpGet("{userId}/count")]
        public async Task<ActionResult<int>> GetOrderCount(int userId) // Chú ý: int userId
        {
            var count = await _orderService.GetOrderCountByUserIdAsync(userId);
            return Ok(count);
        }


        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, [FromBody] UpdateOrderDTO order)
        {
            if (id != order.OrderID)
            {
                return BadRequest();
            }

            var orders = await _orderService.UpdateOrderAsync(id, order);

            return Ok(orders);
        }

        // POST: api/Orders
        [HttpPost("{userId}")]
        public async Task<ActionResult<Order>> PostOrder(int userId, CreateOrderDTO order)
        {
            order.UserID = userId; // gán userId từ Ocelot route
            var createdOrder = await _orderService.CreateOrderAsync(order);
            return Ok(createdOrder);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _orderService.GetOrderByUserIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            await _orderService.DeleteOrderAsync(id);

            return NoContent();
        }

        // Method to get order details by order ID
        // GET: api/Orders/5/OrderDetails
        [HttpGet("{id}/OrderDetails")]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<ReadOrderDetailDTO>>> GetOrderDetailsByOrderId(int id)
        {
            var orderDetails = await _orderService.GetOrderDetailsByOrderIdAsync(id);
            if (orderDetails == null || !orderDetails.Any())
            {
                return NotFound("No order details found for this order.");
            }
            return Ok(orderDetails);
        }

        // Method to get order detail by ID
        // GET: api/Orders/OrderDetails/5
        [HttpGet("OrderDetails/{id}")]
        public async Task<ActionResult<ReadOrderDetailDTO>> GetOrderDetailById(int id)
        {
            var orderDetail = await _orderService.GetOrderDetailByIdAsync(id);
            if (orderDetail == null)
            {
                return NotFound();
            }
            return Ok(orderDetail);
        }

        // Method to add a new order detail
        // POST: api/Orders/5/OrderDetails
        [HttpPost("{id}/OrderDetails")]
        public async Task<ActionResult<ReadOrderDetailDTO>> PostOrderDetail(int id, CreateOrderDetailDTO orderDetail)
        {
            var createdOrderDetail = await _orderService.AddOrderDetailAsync(id, orderDetail);
            if (createdOrderDetail == null)
            {
                return BadRequest("Failed to create order detail.");
            }
            return CreatedAtAction(nameof(GetOrderDetailById), new { id = createdOrderDetail.OrderDetailID }, createdOrderDetail);
        }

        // Method to update an existing order detail
        // PUT: api/Orders/5/OrderDetails
        /*        [HttpPut("{id}/OrderDetails")]
                public async Task<IActionResult> UpdateOrderDetails(int id, [FromForm]UpdateOrderDetailDTO orderDetail)
                {
                    var updatedOrderDetail = await _orderService.UpdateOrderDetailAsync(id, orderDetail);
                    if (updatedOrderDetail == null)
                    {
                        return NotFound();
                    }

                    return Ok(updatedOrderDetail);
                }*/

        // Method to delete an order detail
        // DELETE: api/Orders/OrderDetails/5
        /*        [HttpDelete("OrderDetails/{id}")]
                public async Task<IActionResult> DeleteOrderDetail(int id)
                {
                    var orderDetail = await _orderService.GetOrderDetailByIdAsync(id);
                    if (orderDetail == null)
                    {
                        return NotFound();
                    }

                    await _orderService.DeleteOrderDetailAsync(id);

                    return NoContent();
                }*/
    }
}
