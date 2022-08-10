using SoftTurFlights.Models;

namespace SoftTurFlights.Interfaces
{
    public interface IClientRepository
    {
         dynamic Get(string api_key);       
         bool Insert(ClientModel client);         
         bool Update(string api_key, string client, string url);         
         bool Delete(string api_key);
         List<string> GetAirports();
    }
}