using DeliveryApp.BusinessLayer.Models;
using EventStore.Client;
using Newtonsoft.Json;
using System;
using System.Net.Mail;
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

            SendEmail(packageData);
        }

        private static void SendEmail(PackageData packageData)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("127.0.0.1");

                mail.From = new MailAddress("notifications@mail.com");
                mail.To.Add(packageData.Email);
                mail.Subject = "Delivery Confirmation";

                mail.Body = $"Your package has been delivered\n\n" +
                    $"{packageData.Id}\n" +
                    $"{packageData.Receiver}\n" +
                    $"{packageData.Street} {packageData.Number}\n" +
                    $"{packageData.ZipCode} {packageData.City}\n" +
                    $"Sender email: {packageData.Email}";

                SmtpServer.Port = 2500;

                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Sending a delivery confirmation email failed.");
            }
        }
    }
}
