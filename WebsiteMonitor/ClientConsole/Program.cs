using ClientConsole;
class Program
{
    static async Task Main(string[] args)
    { 
        Startup startup = new Startup();
        await startup.Run();
    }
}