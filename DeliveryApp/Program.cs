using System;
using DeliveryApp.BusinessLayer;
using DeliveryApp.BusinessLayer.Interfaces;
using DeliveryApp.BusinessLayer.Services;
using DeliveryApp.DataLayer.Models;
using Unity;

namespace DeliveryApp
{
    class Program
    {
        private readonly IMenu _menu;
        private readonly IIoHelper _ioHelper;
        private readonly IDbService _dbService;
        private readonly IWaybillsService _waybillsService;
        private readonly IUsersService _usersService;
        private readonly IPackagesService _packagesService;
        private readonly IVehiclesService _vehiclesService;
        private readonly IDeliveriesService _deliveriesService;

        public Program(IGeographicDataService geoDataService,
                       IMenu menu,
                       IIoHelper ioHelper,
                       IDbService dbService,
                       IWaybillsService waybillsService,
                       IUsersService usersService,
                       IPackagesService packagesService,
                       IVehiclesService vehiclesService,
                       IDeliveriesService deliveriesService)
        {
            _usersService = usersService;
            _packagesService = packagesService;
            _waybillsService = waybillsService;
            _vehiclesService = vehiclesService;
            _deliveriesService = deliveriesService;
            _menu = menu;
            _ioHelper = ioHelper;
            _dbService = dbService;
        }

        static void Main()
        {
            var container = new DiContainerProvider().GetContainer();

            container.Resolve<Program>().Run();
        }

        void Run()
        {
            _dbService.EnsureDatabaseCreation();

            new TimersService().SetTimer(_waybillsService.MatchPackages,
                new DateTime(TimeProvider.Now.Year,
                TimeProvider.Now.Month,

                TimeProvider.Now.Day, 0, 0, 0, 0)
                );

            _waybillsService.MatchPackages();
            _deliveriesService.StartDelivering();
            _deliveriesService.FinishDelivering();

            new TimersService().SetTimer(_deliveriesService.StartDelivering,
                new DateTime(TimeProvider.Now.Year,
                TimeProvider.Now.Month,
                TimeProvider.Now.Day, 8, 0, 0, 0)
                );

            new TimersService().SetTimer(_deliveriesService.FinishDelivering,
                new DateTime(TimeProvider.Now.Year,
                TimeProvider.Now.Month,
                TimeProvider.Now.Day, 18, 0, 0, 0)
                );

            Console.WriteLine("Welcome to the DeliveryApp.\n");
            
            int userChoice;
            RegisterMenuOptions();

            do
            {
                userChoice = GetUserOption(_menu);
                Console.WriteLine();
                _menu.ExecuteOption(userChoice);

                if (userChoice == 0) return;
            }
            while (userChoice != 0);

        }

        private int GetUserOption(IMenu menu)
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
                _ioHelper.DisplayInfo("Vehicle with given plates number already exists!\n", MessageType.Error);
                return;
            }

            vehicle = new Vehicle()
            {
                Make = _ioHelper.GetTextFromUser("Enter vehicle\'s make"),
                Model = _ioHelper.GetTextFromUser("Enter vehicle\'s model"),
                Plate = plate,
                Capacity = _ioHelper.GetUintFromUser("Enter vehicle\'s capacity [kg]"),
                AverageSpeed = _ioHelper.GetIntFromUser("Enter vehicle\'s average speed [km/h]"),
            };

            do
            {
                vehicle.UserId = _usersService.GetUserId(_ioHelper.GetTextFromUser("Enter courier\'s email"));

            } while (!_usersService.CheckIfValidCourier(vehicle.UserId) || vehicle.UserId == 0);

            _vehiclesService.AddAsync(vehicle).Wait();

            _ioHelper.DisplayInfo("Vehicle added successfully!\n", MessageType.Success);
        }

        private void AddPackage()
        {
            var userId = _usersService.GetUserId(_ioHelper.GetTextFromUser("Enter sender\'s email"));

            if (userId == 0)
            {
                _ioHelper.DisplayInfo("User with given email does not exist!\n", MessageType.Error);
                return;
            }

            Package package = new Package()
            {
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
                Size = (Size)Convert.ToInt32(_ioHelper.GetIntFromUser("Enter package weight")),
                Status = Status.PendingSending
            };

            _packagesService.AddAsync(package).Wait();

            _ioHelper.DisplayInfo("Package sent successfully!\n", MessageType.Success);

        }

        private void AddUser()
        {
            User user;
            var email = _ioHelper.GetTextFromUser("Provide an email");

            if (!_ioHelper.ValidateEmail(email))
            {
                _ioHelper.DisplayInfo("Email must contain \'@\' character!\n", MessageType.Error);
                return;
            }

            if (_usersService.CheckIfUserExists(email))
            {
                _ioHelper.DisplayInfo("User with given email already exists!\n", MessageType.Error);
                return;
            }

            var password = _ioHelper.GetTextFromUser("Provide a password (minimum 6 characters)");

            if (!_ioHelper.ValidatePassword(password))
            {
                Console.WriteLine("Password must have at least 6 characters!\n");
                return;
            }

            user = new User()
            {
                Email = email,
                Password = password,
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

            _usersService.AddAsync(user).Wait();

            _ioHelper.DisplayInfo("User added successfully!\n", MessageType.Success);
        }
    }
}
