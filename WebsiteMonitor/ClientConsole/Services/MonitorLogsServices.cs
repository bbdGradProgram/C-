using Shared.DTOs;
using System.Net.Http.Json;

namespace ClientConsole.Services
{
    public static class MonitorLogServices
    {
        static HttpClient httpClient = new HttpClient();

        public async static Task<List<MonitorLogGetDto>> GetMonitorLogs(int websiteId)
        {
            try
            {
                string apiUrl = $"{AuthServices.APIHost}/api/Websites/{websiteId}/monitorlogs";
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AuthServices.AccessToken);
                var monitorLogs = await httpClient.GetFromJsonAsync<List<MonitorLogGetDto>>(apiUrl);

                if (monitorLogs == null || monitorLogs.Count == 0)
                {
                    Console.WriteLine("No monitor logs found  .");
                }

                return monitorLogs;
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
    }
}
