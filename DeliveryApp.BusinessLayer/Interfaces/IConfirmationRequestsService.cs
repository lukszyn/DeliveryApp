using DeliveryApp.BusinessLayer.Models;

namespace DeliveryApp.BusinessLayer.Services
{
    public interface IConfirmationRequestsService
    {
        void SendRequest(PackageData packageData);
    }
}