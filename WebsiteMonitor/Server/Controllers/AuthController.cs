using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Server.Services; 

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthController(IConfiguration configuration, IUserService userService, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _userService = userService;
            _httpClientFactory = httpClientFactory;
        }
        [HttpGet("callback")]
        public async Task<IActionResult> GitHubCallback(string code)
        {
            var accessToken = await ExchangeCodeForToken(code);

            if (accessToken == null)
            {
                return BadRequest("Error retrieving access token.");
            }

            return Ok(accessToken);
        }   
        private async Task<DeviceTokenResponse?> ExchangeCodeForToken(string code)
        {
            var client = _httpClientFactory.CreateClient();
            // Set the Accept header to application/json
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var requestContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = "603609b84c91f62b1b27",
                ["client_secret"] = "b367d352894064e7af1b98de3a25208dd94ddaf6",
                ["code"] = code
            });

            var response = await client.PostAsync("https://github.com/login/oauth/access_token", requestContent);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var payload = JsonSerializer.Deserialize<DeviceTokenResponse>(responseContent);
                return payload!;    
            }
            return null;
        }

        private class DeviceTokenResponse
        {
            public string access_token{ get; set; }
            public string token_type { get; set; }
            public string scope { get; set; }
        }

    }
}
