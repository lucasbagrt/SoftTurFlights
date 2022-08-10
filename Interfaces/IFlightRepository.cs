using SoftTurFlights.Models;

namespace SoftTurFlights.Interfaces
{
    public interface IFlightRepository
    {
        dynamic Get(string api_key, DateTime start, DateTime end, string airport, string api_name);
        Task<string> Insert(FlightModel flight);
        Task<string> Update(FlightModel flight);
        FlightModel GetFlight(string flight_code, string type, string api_name, DateTime? date);
        Task<string> SendFlight(List<RequestSendFlight> req);
        Task<string> Save(FlightModel flight);
        bool ExistsRecent(RequestSendFlight req);
    }
}