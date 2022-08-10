namespace SoftTurFlights.Models
{
    public class FlightModel
    {
        public int id { get; set; }
        public string flight_code { get; set; }
        public string airport_code { get; set; }
        public string type { get; set; } //C = chegada, S = saida
        public DateTime? date { get; set; }
        public string time { get; set; }
        public DateTime? included { get; set; }
        public string api_name { get; set; }
    }
}