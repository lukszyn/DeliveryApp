using DeliveryApp.DataLayer.Models;
using DeliveryApp.WebApi.Client.Models;
using GeoCoordinatePortable;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace DeliveryApp.Api.Client
{
    class Program
    {
        private User _loggedUser;

        static void Main(string[] args)
        {
            new Program().Run();
        }

        private void Run()
        {
            Console.WriteLine("Choose option:");
            Console.WriteLine("1. Sign in");
            Console.WriteLine("2. Exit");

            do
            {
                var userChoice = GetIntFromUser("Choose option number");

                switch (userChoice)
                {
                    case 1:
                        SignIn();
                        break;
                    case 2:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Unknown option");
                        break;
                }
            } while (_loggedUser == null);

            Console.WriteLine("Choose option:");
            Console.WriteLine("1. Show latest waybills");
            Console.WriteLine("2. Show route");
            Console.WriteLine("3. Deliver a package");
            Console.WriteLine("4. Exit");

            while (true)
            {
                var userChoice = GetIntFromUser("Choose option number");

                switch (userChoice)
                {
                    case 1:
                        GetLatestWaybills(_loggedUser.Id);
                        SwitchToManualDelivery(_loggedUser.Id);
                        UpdatePackagesStatus(Status.OnTheWay);
                        break;
                    case 2:
                        ShowRoute(_loggedUser.Id);
                        break;
                    case 3:
                        DeliverPackage();
                        break;
                    case 4:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Unknown option");
                        break;
                }
            }
        }

        private void DeliverPackage()
        {
            var packageId = GetIntFromUser("Enter the package id:");
            var package = GetPackage(packageId);

            using (var httpClient = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(package), Encoding.UTF8, "application/json");
                var response = httpClient.PutAsync(@$"http://localhost:10500/api/packages/deliver", content).Result;
                var responseText = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine($"Package delivered successfully.");
                    Console.ResetColor();
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Something went wrong. Package could not be delivered.");
                    Console.ResetColor();
                }
            }
        }

        private Package GetPackage(int id)
        {
            foreach (var package in _loggedUser.Packages)
            {
                if (package.Id == id)
                {
                    return package;
                }
            }

            return null;
        }

        private void ShowRoute(int id)
        {
            var packages = _loggedUser.Packages;
            var position = _loggedUser.Position;

            foreach (Package package in packages)
            {
                var distance = GetPackageDistance(position,
                    package.Sender.Position,
                    package.ReceiverPosition);
                var estimatedTime = EstimateDeliveryTime(_loggedUser.Vehicle.AverageSpeed, distance);

                PrintPackage(package, GetPackageDeliveryData(distance, estimatedTime));
            }
        }

        private double GetPackageDistance(Position current, Position next, Position final)
        {
            var start = new GeoCoordinate(current.Latitude, current.Longitude);
            var middle = new GeoCoordinate(next.Latitude, next.Longitude);
            var end = new GeoCoordinate(final.Latitude, final.Longitude);

            return (start.GetDistanceTo(middle) + middle.GetDistanceTo(end)) / 1000;
        }

        public double EstimateDeliveryTime(double avgSpeed, double distance)
        {
            return distance / avgSpeed;
        }

        private void SignIn()
        {
            var credentials = GetCredentials();

            CheckCredentials(credentials);

            if (_loggedUser == null)
            {
                Console.WriteLine("Invalid email or password.\n");
                return;
            }

            Console.WriteLine($"\nWelcome to your account, {credentials.Email}\n");
        }

        private Credentials GetCredentials()
        {
            var email = GetTextFromUser("Provide an email");
            var password = GetTextFromUser("Provide a password");

            return new Credentials()
            {
                Email = email,
                Password = password
            };
        }

        public string GetTextFromUser(string message)
        {
            Console.Write($"{message}: ");
            return Console.ReadLine();
        }

        public int GetIntFromUser(string message)
        {
            int number;
            while (!int.TryParse(GetTextFromUser(message), out number))
            {
                Console.WriteLine("Not na integer - try again.\n");
            }

            return number;
        }

        public void CheckCredentials(Credentials credentials)
        {
            using (var httpClient = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(credentials), Encoding.UTF8, "application/json");
                var response = httpClient.PostAsync(@$"http://localhost:10500/api/users/credentials", content).Result;
                var responseText = response.Content.ReadAsStringAsync().Result;
                var responseObject = JsonConvert.DeserializeObject<User>(responseText);

                if (response.IsSuccessStatusCode) 
                {
                    _loggedUser = responseObject;
                }
            }
        }

        private void GetLatestWaybills(int id)
        {
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync(@$"http://localhost:10500/api/users/{id}").Result;
                var responseText = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseObject = JsonConvert.DeserializeObject<User>(responseText);
                    _loggedUser.Packages = responseObject.Packages;

                    foreach (var package in _loggedUser.Packages)
                    {
                        PrintPackage(package, GetPackageData(package));
                    }
                }
                else
                {
                    Console.WriteLine($"Failed. Status code: {response.StatusCode}");
                }
            }
        }

        private void PrintPackage(Package package, string textToPrint)
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine($"\n\nPackage no. {package.Number}");
            Console.ResetColor();

            Console.WriteLine(textToPrint);
        }

        private string GetPackageData(Package package)
        {
            return $"\nPackage ID:\n\t{package.Id}" +
                $"\nReceiver:\n\t{package.Receiver}" +
                $"\nReceiver address:\n\t{package.ReceiverAddress.Street} {package.ReceiverAddress.Number}" +
                $"\n\t{package.ReceiverAddress.ZipCode} {package.ReceiverAddress.City}" +
                $"\nSender:\n\t{package.Sender.FirstName} {package.Sender.LastName}";
        }

        private string GetPackageDeliveryData(double distance, double time)
        {
            return $"\nDistance:\n\t{distance:N2} km" +
                $"\nEstimated Delivery Time:\n\t{time:N2} h";
        }

        private void SwitchToManualDelivery(int id)
        {
            using (var httpClient = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(true), Encoding.UTF8, "application/json");
                var response = httpClient.PutAsync(@$"http://localhost:10500/api/users/manual/?id={id}", content).Result;
                var responseText = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine($"Your packages are now delivered manually.");
                    Console.ResetColor();
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Something went wrong. Your packages are delivered automatically.");
                    Console.ResetColor();
                }
            }
        }

        private void UpdatePackagesStatus(Status status)
        {
            using (var httpClient = new HttpClient())
            {
                var packages = ChangePackagesStatus(_loggedUser.Packages, status);

                var content = new StringContent(JsonConvert.SerializeObject(packages), Encoding.UTF8, "application/json");
                var response = httpClient.PutAsync(@$"http://localhost:10500/api/packages/update", content).Result;
                var responseText = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine($"All your packages are on the way.");
                    Console.ResetColor();
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Something went wrong...");
                    Console.ResetColor();
                }
            }
        }

        private ICollection<Package> ChangePackagesStatus(ICollection<Package> packages, Status status)
        {
            foreach (var package in packages)
            {
                package.Status = status;
            }

            return packages;
        }
    }
}
