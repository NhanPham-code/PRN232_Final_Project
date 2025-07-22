using AutoMapper;
using OrderAPI.DTOs;
using OrderAPI.Models;
using OrderAPI.Repositories.Interfaces;
using OrderAPI.Services.Interfaces;

namespace OrderAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepo _orderRepo;
        private readonly IMapper _mapper;
        private readonly IOrderDetailRepo _orderDetailRepo;

        public OrderService(IOrderRepo orderRepo, IMapper mapper, IOrderDetailRepo orderDetailRepo)
        {
            _orderRepo = orderRepo;
            _mapper = mapper;
            _orderDetailRepo = orderDetailRepo;
        }

        // Method to get all orders
        public async Task<IEnumerable<ReadOrderDTO>> GetAllOrderAsync()
        {
            var orders = await _orderRepo.GetAllOrder();
            return _mapper.Map<IEnumerable<ReadOrderDTO>>(orders);
        }

        // Method to get all orders as IQueryable
        public IQueryable<Order> GetAllOrderQueryable()
        {
            var orders = _orderRepo.GetAllOrderQueryable();
            return orders;
        }

        // Method to get an orders by userID
        public async Task<IEnumerable<ReadOrderDTO>> GetOrderByUserIdAsync(int userId)
        {
            var orders = await _orderRepo.GetOrderByUserId(userId);
            return _mapper.Map<IEnumerable<ReadOrderDTO>>(orders);
        }

        // Method to get an orders by userID as IQueryable
        public IQueryable<Order> GetOrderByUserIdQueryable(int userId)
        {
            var orders = _orderRepo.GetOrderByUserIdQueryable(userId);
            /*return _mapper.ProjectTo<ReadOrderDTO>(orders);*/
            return orders;
        }

        // Method to get an orders by userID with CountAsync
        public async Task<int> GetOrderCountByUserIdAsync(int userId)
        {
            return await _orderRepo.GetOrderCountByUserIdAsync(userId);
        }

        // Method to get orders details by orders ID
        public async Task<IEnumerable<ReadOrderDetailDTO>> GetOrderDetailsByOrderIdAsync(int orderId)
        {
            var orderDetails = await _orderDetailRepo.GetOrderDetailsByOrderId(orderId);
            return _mapper.Map<IEnumerable<ReadOrderDetailDTO>>(orderDetails);
        }

        // Method to get orders detail by ID
        public async Task<ReadOrderDetailDTO> GetOrderDetailByIdAsync(int id)
        {
            var orderDetail = await _orderDetailRepo.GetOrderDetailById(id);
            if (orderDetail == null) return null;
            return _mapper.Map<ReadOrderDetailDTO>(orderDetail);
        }

        // Method to create a new orders
        public async Task<ReadOrderDTO> CreateOrderAsync(CreateOrderDTO createOrderDto)
        {
            var order = _mapper.Map<Order>(createOrderDto);
            // Set the now time for OrderDate
            order.OrderDate = DateTime.UtcNow; 
            var createdOrder = await _orderRepo.CreateOrder(order);
            // Create orders details if provided
            if (createOrderDto.OrderDetails != null && createOrderDto.OrderDetails.Any())
            {
                foreach (var detail in createOrderDto.OrderDetails)
                {
                    var orderDetail = _mapper.Map<OrderDetail>(detail);
                    orderDetail.OrderID = createdOrder.OrderID; // Set the OrderID for the detail
                    await _orderDetailRepo.CreateOrderDetail(orderDetail);
                }
            }

            return _mapper.Map<ReadOrderDTO>(createdOrder);
        }

        // Method to add orders detail to an existing orders
        public async Task<ReadOrderDetailDTO> AddOrderDetailAsync(int orderId, CreateOrderDetailDTO createOrderDetailDto)
        {
            var orderDetail = _mapper.Map<OrderDetail>(createOrderDetailDto);

            var createdOrderDetail = await _orderDetailRepo.CreateOrderDetail(orderDetail);
            if (createdOrderDetail == null) return null;

            return _mapper.Map<ReadOrderDetailDTO>(createdOrderDetail);
        }


        // Method to update an existing orders
        public async Task<ReadOrderDTO> UpdateOrderAsync(int id, UpdateOrderDTO updateOrderDTO)
        {
            var order = _mapper.Map<Order>(updateOrderDTO);
            order.OrderID = id; // Ensure the ID is set for the update
            var updatedOrder = await _orderRepo.UpdateOrder(id, order);
            if (updatedOrder == null) return null;
            return _mapper.Map<ReadOrderDTO>(updatedOrder);
        }

        // Method to update an orders detail
        /*        public async Task<ReadOrderDetailDTO> UpdateOrderDetailAsync(int id, UpdateOrderDetailDTO updateOrderDetailDTO)
                {
                    // Get the current quantity of orders detail 
                    var currentOrderDetail = await _orderDetailRepo.GetOrderDetailById(id);
                    var currentQuantity = currentOrderDetail?.Quantity ?? 0;

                    var orderDetail = _mapper.Map<OrderDetail>(updateOrderDetailDTO);
                    orderDetail.OrderDetailID = id; // Ensure the ID is set for the update
                    var updatedOrderDetail = await _orderDetailRepo.UpdateOrderDetail(id, orderDetail);
                    var updatedQuantity = updateOrderDetailDTO.Quantity;
                    if (updatedOrderDetail == null) return null;
                    // If orders detail is updated, update the orders's total amount
                    var order = await _orderRepo.GetOrderByUserId(updatedOrderDetail.OrderID);
                    if (order != null)
                    {
                        // Calculate the difference in quantity
                        var quantityDifference = updatedQuantity - currentQuantity;
                        // Update the total amount based on the difference
                        order.TotalAmount += quantityDifference * updatedOrderDetail.UnitPrice;
                    }
                    return _mapper.Map<ReadOrderDetailDTO>(updatedOrderDetail);
                }*/

        // Method to delete an orders
        public async Task<bool> DeleteOrderAsync(int id)
        {
            return await _orderRepo.DeleteOrders(id);
        }

        // Method to delete an orders detail
/*        public async Task<bool> DeleteOrderDetailAsync(int orderDetailId)
        {
            var orderDetail = await _orderDetailRepo.GetOrderDetailById(orderDetailId);
            if (orderDetail == null) return false;

            int orderId = orderDetail.OrderID;

            // Delete the order detail first
            bool isDeleted = await _orderDetailRepo.DeleteOrderDetail(orderDetailId);
            if (!isDeleted) return false;

            // Get all remaining order details of the order
            var remainingDetails = await _orderDetailRepo.GetOrderDetailsByOrderId(orderId);

            var order = await _orderRepo.GetOrderByUserId(orderId);
            if (order == null) return false;

            if (remainingDetails == null || !remainingDetails.Any())
            {
                // No more order details, delete the order
                await _orderRepo.DeleteOrders(orderId);
                return true;
            }

            // Recalculate total amount
            order.TotalAmount = remainingDetails.Sum(od => od.Quantity * od.UnitPrice);
            await _orderRepo.UpdateOrder(orderId, order);

            return true;
        }*/

        // Method to get all orders as IQueryable
        /*public Task<IQueryable<ReadOrderDTO>> GetAllOrdersQueryableAsync()
        {
            var orders = _orderRepo.GetAllOrder();
            return _mapper.ProjectTo<ReadOrderDTO>(orders);
        }*/
    }
}
