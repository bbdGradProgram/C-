using ClientConsole.Services;
using Shared.DTOs;

namespace ClientConsole
{

    public class WebsiteMonitor
    {
        public async Task RunWebsiteMonitor()
        {
            try
            {
                while (true)
                {
                    ShowMenu().GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private async Task ShowMenu()
        {
            Console.WriteLine("1. Add a new website");
            Console.WriteLine("2. View websites");
            Console.WriteLine("0. Exit");
            Console.Write("> ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await AddWebsite();
                    break;
                case "2":
                    await DisplayWebsitesForUser();
                    break;
                case "0":
                    Console.WriteLine("Exiting Website Monitor.");
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please enter 1 or 0.");
                    break;
            }
        }

        private async Task AddWebsite()
        {
            Console.WriteLine("Please enter the URL of the website you would like to monitor:");
            string url = Console.ReadLine();

            if (!IsValidUrl(url))
            {
                Console.WriteLine("Invalid website URL format. Please enter a valid URL.");
                url = Console.ReadLine();
            }

            bool websiteExists = await WebsiteServices.WebsiteExists(url);

            if (websiteExists)
            {
                Console.WriteLine("Website already exists in the database.");
            }
            else
            {
                int userId = 1;
                WebsiteGetDto addedWebsite = await WebsiteServices.AddWebsite(url, userId);

                if (addedWebsite != null)
                {
                    Console.WriteLine($"Website {addedWebsite.Url} added successfully:");
                }
                else
                {
                    Console.WriteLine("Failed to add the website.");
                }
            }
        }
        private async Task DisplayWebsitesForUser()
        {
            var websites = await WebsiteServices.GetAllWebsites();
            if (websites == null || websites.Count == 0)
            {
                Console.WriteLine("No websites found.");
                return;
            }
            int websiteCount = 1;
            foreach (var website in websites)
            {
                Console.WriteLine($"- {websiteCount}. URL: {website.Url}");
                websiteCount++;
            }
            await PickWebsiteToViewMonitorLogs(1, websites);
        }

        private async Task PickWebsiteToViewMonitorLogs(int userId, List<WebsiteGetDto> userWebsites)
        {
            try
            {
                Console.WriteLine("Enter the number of the website you want to view monitor logs for:");

                int websiteIndex;
                string choice = Console.ReadLine();

                while (!int.TryParse(choice, out websiteIndex) || websiteIndex <= 0 || websiteIndex > userWebsites.Count)
                {
                    Console.WriteLine("Invalid input. Please enter a valid integer number.");
                    choice = Console.ReadLine();
                }

                var chosenWebsite = userWebsites[websiteIndex - 1];
                await MonitorLogs.DisplayMonitorLogs(chosenWebsite.WebsiteID);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }



        private static bool IsValidUrl(string url)
        {
            try
            {
                Uri uriResult;
                return Uri.TryCreate(url, UriKind.Absolute, out uriResult);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

}