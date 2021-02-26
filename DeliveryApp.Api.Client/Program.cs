using DeliveryApp.DataLayer.Models;
using DeliveryApp.WebApi.Client.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace DeliveryApp.Api.Client
{
    class Program
    {
        private string _loggedUserEmail;

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
                var userChoice = GetIntFromUser("Choose option number: ");

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
            } while (_loggedUserEmail != null);

            Console.WriteLine("Choose option:");
            Console.WriteLine("1. Show latest waybills");
            Console.WriteLine("2. Exit");

            while (true)
            {
                var userChoice = GetIntFromUser("Choose option number: ");

                switch (userChoice)
                {
                    case 1:
                        ShowLatestWaybills();
                        break;
                    case 2:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Unknown option");
                        break;
                }
            }
        }

        private void SignIn()
        {
            var credentials = GetCredentials();

            if (!CheckCredentials(credentials))
            {
                Console.WriteLine("Invalid email or password.\n");
                return;
            }

            Console.WriteLine($"\nWelcome to your account, {credentials.Email}\n");
            _loggedUserEmail = credentials.Email;
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

        public bool CheckCredentials(Credentials credentials)
        {
            using (var httpClient = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(credentials), Encoding.UTF8, "application/json");
                var response = httpClient.PostAsync(@$"http://localhost:10500/api/users/credentials", content).Result;
                var responseText = response.Content.ReadAsStringAsync().Result;

                return response.IsSuccessStatusCode ? true : false;
            }
        }

        private void ShowLatestWaybills()
        {
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync(@$"http://localhost:10500/api/users").Result;
                var responseText = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    var reponseObject = JsonConvert.DeserializeObject<List<Dog>>(responseText);
                    Console.WriteLine($"Success. Response content: ");
                    foreach (var dog in reponseObject)
                    {
                        PrintDog(dog);
                    }
                }
                else
                {
                    Console.WriteLine($"Failed. Status code: {response.StatusCode}");
                }
            }
        }
    }
}
