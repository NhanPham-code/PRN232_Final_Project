using DTOs.FeedbackDTO;
using DTOs.UserDTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Service.BaseService;
using Service.Interfaces;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Service.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly HttpClient _client;

        public FeedbackService(GatewayHttpClient gateway)
        {
            _client = gateway.Client;
            
        }

        private void AddBearerToken(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<List<ReadFeedbackDTO>> GetAllAsync(string token)
        {
            AddBearerToken(token);
            var result = await _client.GetFromJsonAsync<List<ReadFeedbackDTO>>("/feedbacks");
            return result ?? new List<ReadFeedbackDTO>();
        }

        public async Task<ReadFeedbackDTO?> GetByUserIdAsync(string token)
        {
            AddBearerToken(token);
            return await _client.GetFromJsonAsync<ReadFeedbackDTO>($"/feedbacks/info");
        }

        public async Task<bool> CreateAsync(CreateFeedbackDTO dto, string token)
        {
            AddBearerToken(token);
            var resp = await _client.PostAsJsonAsync($"/feedbacks/add", dto);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(UpdateFeedbackDTO dto, string token)
        {
            AddBearerToken(token);
            var resp = await _client.PutAsJsonAsync($"/feedbacks/up/{dto.FeedbackID}", dto);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int userId, string token)
        {
            AddBearerToken(token);
            var resp = await _client.DeleteAsync($"/feedbacks/del/{userId}");
            return resp.IsSuccessStatusCode;
        }

        public async Task<List<ReadFeedbackDTO>> GetTopFeedbackAsync(int top, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:7028/odata/ODataFeedbacks?$top=5&$orderby=FeedbackID%20desc");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);


            var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode) return new List<ReadFeedbackDTO>();

            var json = await response.Content.ReadAsStringAsync();

            // OData trả về trong property "value", cần lấy ra
            var jobject = JObject.Parse(json);
            var list = jobject["value"].ToObject<List<ReadFeedbackDTO>>();
            return list;
        }
        public async Task<List<ReadFeedbackDTO>> GetAllOdataAsync(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7028/odata/ODataFeedbacks");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Lỗi gọi API Feedback: " + response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();

            // OData trả về trong property "value", cần lấy ra
            var jobject = JObject.Parse(json);
            var list = jobject["value"].ToObject<List<ReadFeedbackDTO>>();
            return list;
        }
    }
}