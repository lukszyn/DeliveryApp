using DeliveryApp.DataLayer.Models;

namespace DeliveryApp.BusinessLayer.Serializers
{
    public interface ISerializer
    {
        void Serialize(string filePath, User dataSet);
    }
}
