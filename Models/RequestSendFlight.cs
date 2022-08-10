namespace SoftTurFlights.Models
{
    public class RequestSendFlight
    {
        public string flight_code { get; set; }        
        public string api_name { get; set; }        
        public DateTime flight_date { get; set; }
    }
}
