using SoftTurFlights.Models;

namespace SoftTurFlights.Interfaces
{
    public interface IFlightRepository
    {
         dynamic Get(string api_key, DateTime start, DateTime end, string airport, string api_name);
         void Insert(FlightModel flight);
         void Update(FlightModel flight);
         FlightModel? GetFlight(string flight_code, string type, string api_name, DateTime? date);
    }
}