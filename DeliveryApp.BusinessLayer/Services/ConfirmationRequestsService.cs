using DeliveryApp.BusinessLayer.Models;
using EventStore.Client;
using System.Text.Json;

namespace DeliveryApp.BusinessLayer.Services
{
    public class ConfirmationRequestsService : IConfirmationRequestsService
    {
        public void SendRequest(PackageData packageData)
        {
            const string stream = "package-delivered-stream";
            const int defaultPort = 2113;

            var settings = EventStoreClientSettings.Create($"esdb://127.0.0.1:{defaultPort}?Tls=false");

            using (var client = new EventStoreClient(settings))
            {
                client.AppendToStreamAsync(
                    stream,
                    StreamState.Any,
                    new[] { GetEventDataFor(packageData) }).Wait();
            }
        }

        private static EventData GetEventDataFor(PackageData packageData)
        {
            return new EventData(
                Uuid.NewUuid(),
                "package-delivered",
                JsonSerializer.SerializeToUtf8Bytes(packageData));
        }
    }
}
