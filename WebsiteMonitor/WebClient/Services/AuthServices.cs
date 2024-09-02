using Shared.DTOs;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web;
 
namespace WebClient.Services
{
    public static class AuthServices
    {
        static readonly HttpClient httpClient = new();
        public static string? AccessToken;

        public static string APIHost = "http://wmapi-env.eba-uneaiddn.eu-west-1.elasticbeanstalk.com";

        public async static Task<DeviceCodeResponse?> GetUserCode()
        {
            try
            {
            string host = "https://github.com/login/device/code";
 
            var requestBody = new
            {
                client_id = "603609b84c91f62b1b27",
                scope = "read:user, user:email",
            };
 
            var DeviceCode = await httpClient.PostAsJsonAsync(host, requestBody);
 
            if (DeviceCode.IsSuccessStatusCode)
            {
                var responseContent = await DeviceCode.Content.ReadAsStringAsync();
                // Convert query string to a dictionary
                var queryParams = HttpUtility.ParseQueryString(responseContent);
                var dictionary = new Dictionary<string, string>();
                foreach (string key in queryParams)
                {
                    dictionary[key] = queryParams[key]!;
                }
 
                // Convert the dictionary to JSON
                string json = JsonSerializer.Serialize(dictionary);
 
                // Deserialize JSON to the DeviceCodeResponse object
                var payload = JsonSerializer.Deserialize<DeviceCodeResponse>(json);
                Console.WriteLine("use code got successfully.");
                return  payload;
            }
            else
            {
                Console.WriteLine($"Error: {DeviceCode.StatusCode}");
                return null;
            }
    }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HttpRequestException occurred: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                throw;
            }
        }
 
        public static async Task<string?> GetAccessToken(string DeviceCode)
        {
        try
            {
            string host = "https://github.com/login/oauth/access_token";
 
            var requestBody = new
            {
                client_id = "603609b84c91f62b1b27",
                scope = "read:user, user:email",
                device_code = DeviceCode,
                grant_type= "urn:ietf:params:oauth:grant-type:device_code",
            };
 
            var AccessTokenResponse = await httpClient.PostAsJsonAsync(host, requestBody);
 
            if (AccessTokenResponse.IsSuccessStatusCode)
            {
                var responseContent = await AccessTokenResponse.Content.ReadAsStringAsync();
                // Convert query string to a dictionary
                var queryParams = HttpUtility.ParseQueryString(responseContent);
                var dictionary = new Dictionary<string, string>();
                foreach (string key in queryParams)
                {
                    dictionary[key] = queryParams[key]!;
                }
 
                // Convert the dictionary to JSON
                string json = JsonSerializer.Serialize(dictionary);
                var payload = JsonSerializer.Deserialize<DeviceTokenResponse>(json);
                return  payload!.access_token;
            }
            else
            {
                Console.WriteLine($"Enter code on https://github.com/login/device");
                return null;
            }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HttpRequestException occurred: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                throw;
            }
        }
 
        public class DeviceTokenResponse
        {
            public string access_token{ get; set; }
            public string token_type { get; set; }
            public string scope { get; set; }
        }
        public class DeviceCodeResponse
        {
            public string device_code{ get; set; }
            public string user_code { get; set; }
            public string verification_uri { get; set; }
            public string expires_in { get; set; }
            public string interval { get; set; }
        }
    }
}