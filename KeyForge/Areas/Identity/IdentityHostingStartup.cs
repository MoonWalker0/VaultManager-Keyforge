using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(KeyForge.Areas.Identity.IdentityHostingStartup))]
namespace KeyForge.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}