namespace SoftTurFlights.Services.Cirium.Models
{
    public class Airline
    {
        public string fs { get; set; }
        public string iata { get; set; }
        public string icao { get; set; }
        public string name { get; set; }
        public bool active { get; set; }
    }

    public class Airport
    {
        public string fs { get; set; }
        public string iata { get; set; }
        public string icao { get; set; }
        public string faa { get; set; }
        public string name { get; set; }
        public string city { get; set; }
        public string cityCode { get; set; }
        public string stateCode { get; set; }
        public string countryCode { get; set; }
        public string countryName { get; set; }
        public string regionName { get; set; }
        public string timeZoneRegionName { get; set; }
        public string weatherZone { get; set; }
        public DateTime localTime { get; set; }
        public double utcOffsetHours { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int elevationFeet { get; set; }
        public int classification { get; set; }
        public bool active { get; set; }
    }

    public class Appendix
    {
        public List<Airline> airlines { get; set; }
        public List<Airport> airports { get; set; }
        public List<Equipment> equipments { get; set; }
    }

    public class Carrier
    {
        public string requestedCode { get; set; }
        public string fsCode { get; set; }
    }

    public class Codeshare
    {
        public string carrierFsCode { get; set; }
        public string flightNumber { get; set; }
        public string serviceType { get; set; }
        public List<string> serviceClasses { get; set; }
        public List<string> trafficRestrictions { get; set; }
        public int referenceCode { get; set; }
    }

    public class Date
    {
        public string year { get; set; }
        public string month { get; set; }
        public string day { get; set; }
        public string interpreted { get; set; }
    }

    public class Equipment
    {
        public string iata { get; set; }
        public string name { get; set; }
        public bool turboProp { get; set; }
        public bool jet { get; set; }
        public bool widebody { get; set; }
        public bool regional { get; set; }
    }

    public class FlightNumber
    {
        public string requested { get; set; }
        public string interpreted { get; set; }
    }

    public class Request
    {
        public Carrier carrier { get; set; }
        public FlightNumber flightNumber { get; set; }
        public bool departing { get; set; }
        public string url { get; set; }
        public Date date { get; set; }
    }

    public class ResultGetFlight
    {
        public Request request { get; set; }
        public List<ScheduledFlight> scheduledFlights { get; set; }
        public Appendix appendix { get; set; }
    }

    public class ScheduledFlight
    {
        public string carrierFsCode { get; set; }
        public string flightNumber { get; set; }
        public string departureAirportFsCode { get; set; }
        public string arrivalAirportFsCode { get; set; }
        public DateTime departureTime { get; set; }
        public DateTime arrivalTime { get; set; }
        public int stops { get; set; }
        public string departureTerminal { get; set; }
        public string flightEquipmentIataCode { get; set; }
        public bool isCodeshare { get; set; }
        public bool isWetlease { get; set; }
        public string serviceType { get; set; }
        public List<string> serviceClasses { get; set; }
        public List<object> trafficRestrictions { get; set; }
        public List<Codeshare> codeshares { get; set; }
        public string referenceCode { get; set; }
    }

}
