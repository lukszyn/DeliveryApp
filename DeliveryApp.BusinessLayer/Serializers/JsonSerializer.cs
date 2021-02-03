using DeliveryApp.DataLayer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DeliveryApp.BusinessLayer.Serializers
{
    public class JsonSerializer : ISerializer
    {
        public void Serialize(string filePath, User dataSet)
        {
            var jsonData = JsonConvert.SerializeObject(dataSet, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            File.WriteAllText(filePath, jsonData);
        }
    }
}
