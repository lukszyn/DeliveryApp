using Microsoft.AspNetCore.Hosting;
using Topshelf;
using Unity;
using System;

namespace DeliveryApp.WebApi
{
    public class Program
    {
        static void Main()
        {
            var container = new UnityDiContainerProvider().GetContainer();
            var appHost = container.Resolve<AppHost>();

            var rc = HostFactory.Run(x =>
            {
                x.Service<AppHost>(s =>
                {
                    s.ConstructUsing(sf => appHost);
                    s.WhenStarted(ah => ah.Start());
                    s.WhenStopped(ah => ah.Stop());
                });

                x.RunAsLocalSystem();
                x.SetDescription("DeliveryApp WebAPI Service");
                x.SetDisplayName("DeliveryAppAPI.Service");
                x.SetServiceName("DeliveryAppAPI.Service");
            });

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
            Environment.ExitCode = exitCode;
        }
    }
}