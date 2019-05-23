using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Events;

namespace SpectrumCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();
            CreateWebHostBuilder(args).Build().Run();
        }

        private static string DomainToUse = "mydomain.com";

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .UseKestrel(kestrelOptions => kestrelOptions.ConfigureHttpsDefaults(httpsOptions => httpsOptions.ServerCertificate = new X509Certificate2(File.ReadAllBytes("pathToCertificate"), "certificatePassword")))
            .UseUrls("http://" + DomainToUse, "https://" + DomainToUse)
            .UseSerilog();
    }
}
