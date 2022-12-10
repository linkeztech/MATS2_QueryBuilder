using MATS2_QueryBuilder;
using Newtonsoft.Json.Linq;

//IHost host = Host.CreateDefaultBuilder(args)
//    .ConfigureServices(services =>
//    {
//        services.AddHostedService<Worker>();
//    })
//    .Build();

class Program
{
    public static void Main(string[] args)
    {
        string text = System.IO.File.ReadAllText(@"Project/Settings/setup.json");
        JObject jsonConf = JObject.Parse(text);
        bool isWindowsServer = false;
        if (jsonConf.ContainsKey("is_windows_server"))
        {
            isWindowsServer = (bool)jsonConf["is_windows_server"];
        }
        if (isWindowsServer)
        {
            CreateWindowsHostBuilder(args).Build().Run();
        }
        else
        {
            CreateLinuxHostBuilder(args).Build().Run();
        }
    }

    public static IHostBuilder CreateLinuxHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args).UseSystemd()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<Worker>();
            });

    public static IHostBuilder CreateWindowsHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args).UseWindowsService()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<Worker>();
            });
}