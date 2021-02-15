using DeliveryApp.BusinessLayer.Models;
using EventStore.Client;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConfirmationSender
{
    public class Program
    {
        static void Main(string[] args)
        {
            const string stream = "package-delivered-stream";
            const int defaultPort = 2113;

            var settings = EventStoreClientSettings.Create($"esdb://127.0.0.1:{defaultPort}?Tls=false");

            using (var client = new EventStoreClient(settings))
            {
                client.SubscribeToStreamAsync(
                    stream,
                    StreamPosition.End,
                    EventArrived);

                Console.WriteLine("Press any key to close...");
                Console.ReadLine();
            }
        }

        private async static Task EventArrived(
            StreamSubscription subscription,
            ResolvedEvent resolvedEvent,
            CancellationToken cancellationToken)
        {
            var jsonData = Encoding.UTF8.GetString(resolvedEvent.Event.Data.ToArray());

            var packageData = JsonConvert.DeserializeObject<PackageData>(jsonData);

            Console.WriteLine(jsonData);
        }
    }
}
