using System.Net.Http.Json;
using Shared.DTOs;

namespace ClientConsole.Services
{
    public static class WebsiteServices
    {
        private static readonly HttpClient httpClient = new HttpClient();
        

        public async static Task<List<WebsiteGetDto>> GetAllWebsites()
        {
            try
            {
                string apiUrl = $"{AuthServices.APIHost}/api/Websites";
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AuthServices.AccessToken);
                var websites = await httpClient.GetFromJsonAsync<List<WebsiteGetDto>>(apiUrl);
                if (websites == null)
                {
                    Console.WriteLine("No websites found.");
                    return null;
                }

                return websites;
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

        public async static Task<bool> WebsiteExists(string url)
        {
            try
            {
                string apiUrl = $"{AuthServices.APIHost}/api/Websites";
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AuthServices.AccessToken);
                var websites = await httpClient.GetFromJsonAsync<List<WebsiteGetDto>>(apiUrl);

                if (websites == null)
                {
                    Console.WriteLine("No websites found.");
                    return false;
                }

                return websites.Any(w => w.Url == url);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HttpRequestException occurred: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                throw;
            }
        }

        public async static Task<WebsiteGetDto> GetWebsite(int websiteId)
        {
            try
            {
                string apiUrl = $"{AuthServices.APIHost}/api/Websites/{websiteId}";
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AuthServices.AccessToken);
                var website = await httpClient.GetFromJsonAsync<WebsiteGetDto>(apiUrl);
                if (website == null)
                {
                    Console.WriteLine("No website found.");
                    return null;
                }

                return website;
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

        public async static Task<WebsiteGetDto> AddWebsite(string url, int userId)
        {
            try
            {
                WebsitePostDto website = new WebsitePostDto
                {
                    Url = url,
                    UserID = userId
                };

                string apiUrl = $"{AuthServices.APIHost}/api/Websites";
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AuthServices.AccessToken);
                HttpResponseMessage response = await httpClient.PostAsJsonAsync(apiUrl, website);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<WebsiteGetDto>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HttpRequestException occurred: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                throw;
            }
        }
    }
}
