using DTOs.OrderDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IOrderService
    {
        Task<string> GetOrdersAsync(string token, int top, int skip);
        Task<List<ReadOrderDetailDTO>> GetOrderDetailAsync(int orderId, string token, int top, int skip);
        Task<int?> CreateOrderAsync(CreateOrderDTO order, string token);
        Task<bool> CreateOrderDetailAsync(CreateOrderDetailDTO orderDetail, int orderId, string token);
        Task<string> GetAdminOrdersAsync(string token, string keyword, int currentPage, int pageSize);
        Task<bool> UpdateOrderAsync(string token, UpdateOrderDTO updateOrderDto);
    }
}
