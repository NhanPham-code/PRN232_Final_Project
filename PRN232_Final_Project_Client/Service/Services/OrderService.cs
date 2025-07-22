using DTOs.OrderDTO;
using Service.BaseService;
using Service.Interfaces;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;

        public OrderService(GatewayHttpClient gateway)
        {
            _httpClient = gateway.Client;
        }

        private void AddBearerToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = null; // clear cũ
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }


        // get orderDetails detail with token 
        public async Task<List<ReadOrderDetailDTO>> GetOrderDetailAsync(int orderId, string token, int skip = 0, int top = 8)
        {
            AddBearerToken(token);
            var url = $"/OrderHistory/details/{orderId}";
            var orderDetails = await _httpClient.GetFromJsonAsync<List<ReadOrderDetailDTO>>(url);
            return orderDetails ?? new List<ReadOrderDetailDTO>();
        }

        // Create a new order with token (bool type)
        public async Task<int?> CreateOrderAsync(CreateOrderDTO order, string token)
        {
            AddBearerToken(token);
            var response = await _httpClient.PostAsJsonAsync("/OrderHistory/add", order);
            if (!response.IsSuccessStatusCode) return null;
            var responseContent = await response.Content.ReadFromJsonAsync<ReadOrderDTO>();
            return responseContent?.OrderID;
        }

        // Create a new order detail with token (bool type)
        public async Task<bool> CreateOrderDetailAsync(CreateOrderDetailDTO orderDetail, int orderId, string token)
        {
            AddBearerToken(token);
            var response = await _httpClient.PostAsJsonAsync($"/OrderDetails/create", orderDetail);
            return response.IsSuccessStatusCode;
        }

        public async Task<string> GetOrdersAsync(string token, int skip = 0, int top = 8)
        {
            try
            {
                AddBearerToken(token);

                // Tạo query string với OData parameters
                var queryString = $"?$orderby=OrderId desc&$count=true&$top={top}&$skip={skip}&$expand=OrderDetails";
                var response = await _httpClient.GetAsync($"/OrderHistory/me{queryString}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new HttpRequestException($"API call failed with status {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error calling order history API: {ex.Message}", ex);
            }
        }

        public async Task<string> GetAdminOrdersAsync(string token, string keyword, int currentPage, int pageSize)
        {
            try
            {
                AddBearerToken(token);
                if (string.IsNullOrWhiteSpace(token)) return null;

                var skip = (currentPage - 1) * pageSize;
                var filter = string.IsNullOrWhiteSpace(keyword)
                    ? ""
                    : $"$filter=contains(CustomerName,'{keyword}')&";

                var url = $"/OrderAdmin/all?{filter}$orderby=OrderId desc&top={pageSize}&$skip={skip}&$count=true&expand=OrderDetails"; // Đường dẫn đi qua Ocelot

/*                var url = $"/OrderAdmin/all"; // Bỏ filter OData để test*/

                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"API Error: {response.StatusCode}");
                    return null;
                }
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine("API Response: " + content);
                return content;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateOrderAsync(string token, UpdateOrderDTO updateOrderDto)
        {
            AddBearerToken(token); // Nếu bạn đang dùng JWT

            var response = await _httpClient.PutAsJsonAsync($"/Orders/{updateOrderDto.OrderID}/update", updateOrderDto);

            return response.IsSuccessStatusCode;
        }


    }
}
