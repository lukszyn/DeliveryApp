using System;
using DeliveryApp.BusinessLayer;
using DeliveryApp.BusinessLayer.Interfaces;
using DeliveryApp.BusinessLayer.Services;
using DeliveryApp.DataLayer.Models;
using DeliveryApp.BusinessLayer.Serializers;

namespace DeliveryApp
{
    class Program
    {
        private Menu _menu = new Menu();
        private IoHelper _ioHelper = new IoHelper();
        private DbService _dbService = new DbService();
        private TimerService _timer = new TimerService();
        private WaybillsService _waybillsService;
        private readonly IUsersService _usersService;
        private readonly IPackagesService _packagesService;
        private readonly IVehiclesService _vehiclesService;

        public Program(IGeographicDataService geoDataService,
                       IUsersService usersService,
                       IPackagesService packagesService,
                       IVehiclesService vehiclesService)
        {
            _usersService = usersService;
            _packagesService = packagesService;
            _waybillsService = new WaybillsService(packagesService, usersService,
                vehiclesService, new JsonSerializer());
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
            _waybillsService.MatchPackages();
            _timer.SetTimer(_waybillsService.MatchPackages, 1000 * 60 * 24);

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
            Vehicle vehicle;
            var plate = _ioHelper.GetTextFromUser("Enter the licence plate");

            if (_vehiclesService.FindByPlate(plate))
            {
                _ioHelper.DisplayInfo("Vehicle with given plates number already exists!\n", MessageType.ERROR);
                return;
            }

            vehicle = new Vehicle()
            {
                Make = _ioHelper.GetTextFromUser("Enter vehicle\'s make"),
                Model = _ioHelper.GetTextFromUser("Enter vehicle\'s model"),
                Plate = plate,
                Capacity = _ioHelper.GetUintFromUser("Enter vehicle\'s capacity [kg]"),
            };

            do
            {
                vehicle.UserId = _usersService.GetUserId(_ioHelper.GetTextFromUser("Enter courier\'s email"));

            } while (!_usersService.CheckIfValidCourier(vehicle.UserId) || vehicle.UserId == 0);

            _vehiclesService.Add(vehicle);

            _ioHelper.DisplayInfo("Vehicle added successfully!\n", MessageType.SUCCESS);
        }

        private void AddPackage()
        {
            var userId = _usersService.GetUserId(_ioHelper.GetTextFromUser("Enter sender\'s email"));

            if (userId == 0)
            {
                _ioHelper.DisplayInfo("User with given email does not exist!\n", MessageType.ERROR);
                return;
            }

            Package package = new Package()
            {
                Number = Guid.NewGuid(),
                SenderId = userId,
                Receiver = _ioHelper.GetTextFromUser("Enter receiver\'s first name") + " "
                              + _ioHelper.GetTextFromUser("Enter receiver\'s last name"),
                ReceiverAddress = new Address()
                {
                    Street = _ioHelper.GetTextFromUser("Enter street name"),
                    Number = _ioHelper.GetUintFromUser("Enter building number"),
                    City = _ioHelper.GetTextFromUser("Enter city name"),
                    ZipCode = _ioHelper.GetTextFromUser("Enter zip code"),
                },
                RegisterDate = AcceleratedDateTime.Now,
                Size = (Size)Convert.ToInt32(_ioHelper.GetIntFromUser("Enter package weight")),
                Status = Status.PENDING_SENDING
            };

            _packagesService.Add(package);

            _ioHelper.DisplayInfo("Package sent successfully!\n", MessageType.SUCCESS);

        }

        private void AddUser()
        {
            User user;
            var email = _ioHelper.GetTextFromUser("Provide an email");

            if (!_ioHelper.ValidateEmail(email))
            {
                _ioHelper.DisplayInfo("Email must contain \'@\' character!\n", MessageType.ERROR);
                return;
            }

            if (_usersService.CheckIfUserExists(email))
            {
                _ioHelper.DisplayInfo("User with given email already exists!\n", MessageType.ERROR);
                return;
            }

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

            _ioHelper.DisplayInfo("User added successfully!\n", MessageType.SUCCESS);
        }
    }
}
