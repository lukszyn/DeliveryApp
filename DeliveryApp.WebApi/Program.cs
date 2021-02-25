using Unity.Microsoft.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace DeliveryApp.WebApi
{
    public class Program
    {
        static void Main(string[] args)
        {
            var container = new UnityDiContainerProvider().GetContainer();

            WebHost
                .CreateDefaultBuilder()
                .UseUnityServiceProvider(container)
                .ConfigureServices(services =>
                {
                    services.AddMvc();
                })
                .Configure(app => {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                    });
                    app.UseCors();
                })
                .UseUrls("http://*:10500")
                .Build()
                .Run();
        }
    }
}