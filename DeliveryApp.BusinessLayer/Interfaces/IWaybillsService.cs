﻿using DeliveryApp.DataLayer.Models;
using GeoCoordinatePortable;
using System.Collections.Generic;

namespace DeliveryApp.BusinessLayer.Services
{
    public interface IWaybillsService
    {
        double CalculateDeliveryTime(User driver);
        User ChooseDriver(Package package, List<User> drivers);
        public double EstimateDriveTime(double avgSpeed, Position current, Position next, Position final);
        GeoCoordinate GetLocation(Address address);
        void MatchPackages();
    }
}