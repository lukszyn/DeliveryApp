using System;
using DeliveryApp.BusinessLayer;
using DeliveryApp.BusinessLayer.Interfaces;
using DeliveryApp.BusinessLayer.Services;
using DeliveryApp.DataLayer.Models;

namespace DeliveryApp
{
    class Program
    {
        private Menu _loggingMenu = new Menu();
        private Menu _menu = new Menu();
        private IoHelper _ioHelper = new IoHelper();
        private DbService _dbService = new DbService();
        private readonly IGeographicDataService _geoDataService;
        private readonly IUsersService _usersService;
        private readonly IPackagesService _packagesService;
        private readonly IVehiclesService _vehiclesService;

        public Program(IGeographicDataService geoDataService,
                       IUsersService usersService,
                       IPackagesService packagesService,
                       IVehiclesService vehiclesService)
        {
            _geoDataService = geoDataService;
            _usersService = usersService;
            _packagesService = packagesService;
            _vehiclesService = vehiclesService;
        }

        static void Main()
        {
            new Program(new GeographicDataService(),
                new UsersService(),
                new PackagesService(),
                new VehiclesService())
                .Run();
        }

        void Run()
        {
            _dbService.EnsureDatabaseCreation();

            Console.WriteLine("Welcome to the BankApp.\n");
            
            int userChoice;
            RegisterMenuOptions();

            do
            {
                userChoice = GetUserOption(_menu);

                _menu.ExecuteOption(userChoice);

                if (userChoice == 0) return;
            }
            while (userChoice != 0);

        }

        private int GetUserOption(Menu menu)
        {
            menu.PrintAvailableOptions();
            Console.WriteLine("Press 0 to exit.");
            return _ioHelper.GetIntFromUser("\nChoose action");
        }

        private void RegisterMenuOptions()
        {
            _menu.AddOption(new MenuItem { Key = 1, Action = AddUser, Description = "Press 1 to add a user" });
            _menu.AddOption(new MenuItem { Key = 2, Action = AddPackage, Description = "Press 2 to send a package" });
            _menu.AddOption(new MenuItem { Key = 3, Action = AddVehicle, Description = "Press 3 to add a vehicle" });
        }

        private void AddVehicle()
        {
            throw new NotImplementedException();
        }

        private void AddPackage()
        {
            throw new NotImplementedException();
        }

        private void AddUser()
        {
            User user;
            var email = _ioHelper.GetTextFromUser("Provide an email");

            if (!_ioHelper.ValidateEmail(email))
            {
                Console.WriteLine("Email must contain \'@\' character!");
                return;
            }

            if (!_usersService.FindByEmail(email))
            {
                user = new User()
                {
                    Email = email,
                    FirstName = _ioHelper.GetTextFromUser("Enter your first name"),
                    LastName = _ioHelper.GetTextFromUser("Enter your last name"),
                    Address = new Address()
                    {
                        Street = _ioHelper.GetTextFromUser("Enter street name"),
                        Number = _ioHelper.GetUintFromUser("Enter building number"),
                        City = _ioHelper.GetTextFromUser("Enter city name"),
                        ZipCode = _ioHelper.GetTextFromUser("Enter zip code"),
                    },
                    UserType = (UserType)Convert
                    .ToInt32(_ioHelper.GetIntFromUser("Enter user type (1 - customer, 2 - courier)"))
                };

                _usersService.Add(user);
                Console.WriteLine("User added successfully!\n");

            }
            else
            {
                Console.WriteLine("User with given email already exists!");
            }

        }
    }
}
