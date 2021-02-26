using Unity.Microsoft.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Unity;

namespace DeliveryApp.WebApi
{
    public class AppHost
    {
        private IWebHost _webApiHost;
        private readonly IUnityContainer _container;

        public AppHost(IUnityContainer container)
        {
            _container = container;
        }

        public void Start()
        {
            _webApiHost = WebHost
                .CreateDefaultBuilder()
                .UseUnityServiceProvider(_container)
                .ConfigureServices(services =>
                {
                    services.AddMvc();
                })
                .Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                    });
                    app.UseCors();
                })
                .UseUrls("http://*:10500")
                .Build();

            _webApiHost.RunAsync();
        }

        public void Stop()
        {
            _webApiHost.StopAsync().Wait();
        }
    }
}
