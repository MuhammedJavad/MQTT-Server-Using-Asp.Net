using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet.AspNetCore;

namespace WebClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseIISIntegration()
                .UseKestrel(o => {
                o.ListenAnyIP(1884, l => l.UseMqtt()); // mqtt pipeline
                o.ListenAnyIP(5000); // default http pipeline
            })
            .UseStartup<Startup>();
    }
}
