using ClientConsole.Services;
 
namespace ClientConsole
{
    public class Startup
    {
       async public Task Run()
        {
 
            DisplayBanner();
            Console.WriteLine("Welcome to Website Monitor Inc.");
            Console.WriteLine("Hello! Would you like to login? (yes/no)");
 
            string userInput;
            do
            {
                Console.WriteLine("1. Yes");
                Console.WriteLine("2. No");
                Console.Write("> ");
 
                userInput = Console.ReadLine();
 
                if (userInput == "1")
                {
                    var DeviceCode = await AuthServices.GetUserCode();
                    if(DeviceCode == null){
                        Console.WriteLine("Error: Could not get device code");
                        break;
                    }
 
                    Console.WriteLine("\n\nGo to https://github.com/login/device and enter the code \n" + DeviceCode.user_code + "\n\n");
 
 
                    var AccessToken = await AuthServices.GetAccessToken(DeviceCode.device_code);
                    if (AccessToken != null)
                    {
                        Console.WriteLine("Access token received successfully");
                        return;
                    }
 
                    // Start polling for the access token based on the interval
                    int maxAttempts = 5;
                    int attempts = 0;
 
                    while (AccessToken == null && attempts < maxAttempts)
                    {
                        Console.WriteLine("Waiting for user to authorize...");
                        await Task.Delay(20 * 1000); // Convert seconds to milliseconds
 
                        AccessToken = await AuthServices.GetAccessToken(DeviceCode.device_code);
                        attempts++;
                        if (AccessToken != null)
                        {
                            AuthServices.AccessToken = AccessToken;
                        }
                    }
                    if (AccessToken == null)
                    {
                        Console.WriteLine("Error: Could not get access token after " + maxAttempts + " attempts");
                        break;
                    }
 
                    // Authenticated successful now we do the following
                     WebsiteMonitor websiteMonitor = new();
                     await websiteMonitor.RunWebsiteMonitor();
                }
                else if (userInput == "2")
                {
                    Console.WriteLine("You chose not to login. You cannot proceed without logging in.");
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter '1' for Yes or '2' for No.");
                }
            } while (userInput != "1" && userInput != "2");
        }
 
 
        private static void DisplayBanner()
        {
            string startBanner = @"
 
__          __  _         _ _         __  __             _ _               _____              
\ \        / / | |       (_) |       |  \/  |           (_) |             |_   _|             
  \ \  /\  / /__| |__  ___ _| |_ ___  | \  / | ___  _ __  _| |_ ___  _ __    | |  _ __   ___   
   \ \/  \/ / _ \ '_ \/ __| | __/ _ \ | |\/| |/ _ \| '_ \| | __/ _ \| '__|   | | | '_ \ / __|  
    \  /\  /  __/ |_) \__ \ | ||  __/ | |  | | (_) | | | | | || (_) | |     _| |_| | | | (__ _ 
     \/  \/ \___|_.__/|___/_|\__\___| |_|  |_|\___/|_| |_|_|\__\___/|_|    |_____|_| |_|\___(_)

";
            Console.WriteLine(startBanner);
        }
    }
}