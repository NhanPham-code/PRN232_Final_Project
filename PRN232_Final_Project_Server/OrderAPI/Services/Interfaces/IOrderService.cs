using OrderAPI.DTOs;
using OrderAPI.Models;

namespace OrderAPI.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<ReadOrderDTO>> GetAllOrderAsync();
        IQueryable<Order> GetAllOrderQueryable();
        Task<IEnumerable<ReadOrderDTO>> GetOrderByUserIdAsync(int id);
        IQueryable<Order> GetOrderByUserIdQueryable(int id);
        Task<int> GetOrderCountByUserIdAsync(int userId);
        Task<IEnumerable<ReadOrderDetailDTO>> GetOrderDetailsByOrderIdAsync(int orderId);
        Task<ReadOrderDetailDTO> GetOrderDetailByIdAsync(int id);
        Task<ReadOrderDTO> CreateOrderAsync(CreateOrderDTO orderDTO);
        Task<ReadOrderDetailDTO> AddOrderDetailAsync(int orderId, CreateOrderDetailDTO createOrderDetailDto);
        Task<ReadOrderDTO> UpdateOrderAsync(int id, UpdateOrderDTO orderDTO);
        //Task<ReadOrderDetailDTO> UpdateOrderDetailAsync(int id, UpdateOrderDetailDTO updateOrderDetailDTO);
        Task<bool> DeleteOrderAsync(int id);
        //Task<bool> DeleteOrderDetailAsync(int id);
    }
}
