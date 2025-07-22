using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace UserUI.Helpers
{
    public class VnPayLibrary
    {
        private SortedList<string, string> requestData = new SortedList<string, string>();
        private SortedList<string, string> responseData = new SortedList<string, string>();

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                requestData.Add(key, value);
            }
        }

        public string CreateRequestUrl(string baseUrl, string hashSecret)
        {
            var data = new StringBuilder();
            var query = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in requestData)
            {
                data.Append($"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}&");
            }

            var rawData = data.ToString().TrimEnd('&');
            string secureHash = HmacSHA512(hashSecret, rawData);
            string url = $"{baseUrl}?{rawData}&vnp_SecureHash={secureHash}";
            return url;
        }

        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                responseData.Add(key, value);
            }
        }

        public string GetResponseData(string key)
        {
            return responseData.TryGetValue(key, out string value) ? value : null;
        }

        public bool ValidateSignature(string hashSecret)
        {
            var rawData = responseData
                .Where(kv => kv.Key != "vnp_SecureHash" && kv.Key != "vnp_SecureHashType")
                .Select(kv => $"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}");
            var rawString = string.Join("&", rawData);
            var secureHash = HmacSHA512(hashSecret, rawString);
            return secureHash == GetResponseData("vnp_SecureHash");
        }

        public static string HmacSHA512(string key, string inputData)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hash = hmac.ComputeHash(inputBytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }

}
