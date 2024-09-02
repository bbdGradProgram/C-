using ClientConsole.Services;
namespace ClientConsole
{
    public class MonitorLogs
    {
        public static async Task DisplayMonitorLogs(int websiteId)
        {
            try
            {
                if (websiteId <= 0)
                {
                    Console.WriteLine("Invalid website ID.");
                    return;
                }

                var monitorLogs = await MonitorLogServices.GetMonitorLogs(websiteId);

                if (monitorLogs == null || monitorLogs.Count == 0)
                {
                    return;
                }

                foreach (var log in monitorLogs)
                {
                    ConsoleColor statusColor = log.ResponseStatus == 200 ? ConsoleColor.Green : ConsoleColor.Red;

                    Console.ForegroundColor = statusColor;

                    Console.WriteLine($"Date Checked: {log.DateChecked}\tResponse Status: {log.ResponseStatus}");

                    Console.ResetColor();
                    Console.WriteLine("--------------------------------------");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching monitor logs: {ex.Message}");
            }
        }
    }
}

