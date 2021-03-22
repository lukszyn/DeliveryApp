using DeliveryApp.DataLayer.Models;
using System.Threading.Tasks;

namespace DeliveryApp.BusinessLayer.Serializers
{
    public interface ISerializer
    {
        Task<User> DeserializeAsync(string filePath);
        void Serialize(string filePath, User dataSet);
    }
}
