using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace BencomOpdracht
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://local.bencomopdracht.nl:5000")
                .UseStartup<Startup>();
            return host;
        }
    }
}
