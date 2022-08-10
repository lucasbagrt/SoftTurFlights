namespace SoftTurFlights.Services.Aviation.Models
{
    public class Airline
    {
        public string iataCode { get; set; }
        public string icaoCode { get; set; }
        public string name { get; set; }
    }

    public class Arrival
    {
        public object actualRunway { get; set; }
        public object actualTime { get; set; }
        public string baggage { get; set; }
        public string delay { get; set; }
        public object estimatedRunway { get; set; }
        public DateTime? estimatedTime { get; set; }
        public string gate { get; set; }
        public string iataCode { get; set; }
        public string icaoCode { get; set; }
        public DateTime scheduledTime { get; set; }
        public string terminal { get; set; }
        public string portuguese { get; set; }
    }

    public class Codeshared
    {
        public Airline airline { get; set; }
        public Flight flight { get; set; }
    }

    public class Departure
    {
        public DateTime? actualRunway { get; set; }
        public DateTime? actualTime { get; set; }
        public object baggage { get; set; }
        public string delay { get; set; }
        public DateTime? estimatedRunway { get; set; }
        public DateTime? estimatedTime { get; set; }
        public string gate { get; set; }
        public string iataCode { get; set; }
        public string icaoCode { get; set; }
        public DateTime scheduledTime { get; set; }
        public string terminal { get; set; }
        public string portuguese { get; set; }
    }

    public class Flight
    {
        public string iataNumber { get; set; }
        public string icaoNumber { get; set; }
        public string number { get; set; }
    }

    public class ResultGetSchedule
    {
        public Airline airline { get; set; }
        public Arrival arrival { get; set; }
        public Codeshared codeshared { get; set; }
        public Departure departure { get; set; }
        public Flight flight { get; set; }
        public string status { get; set; }
        public Translations translations { get; set; }
        public string type { get; set; }
    }

    public class Translations
    {
        public Arrival arrival { get; set; }
        public Departure departure { get; set; }
    }


    public class Aircraft
    {
        public string modelCode { get; set; }
        public string modelText { get; set; }
    }

    public class ArrivalFuture
    {
        public string iataCode { get; set; }
        public string icaoCode { get; set; }
        public string terminal { get; set; }
        public string gate { get; set; }
        public string scheduledTime { get; set; }
    }

    public class DepartureFuture
    {
        public string iataCode { get; set; }
        public string icaoCode { get; set; }
        public string terminal { get; set; }
        public string gate { get; set; }
        public string scheduledTime { get; set; }
    }

    public class ResultGetFutureSchedules
    {
        public string weekday { get; set; }
        public string tpVoo { get; set; }
        public string dsSiglaAeroporto { get; set; }
        public DepartureFuture departure { get; set; }
        public ArrivalFuture arrival { get; set; }
        public Aircraft aircraft { get; set; }
        public Airline airline { get; set; }
        public Flight flight { get; set; }
        public Codeshared codeshared { get; set; }
        public DateTime dtVoo { get; set; }
    }

    public class RequestGetSchedule
    {
        public string key { get; set; }
        public string iataCode { get; set; }
        public string type { get; set; }
    }
    public class RequestFutureSchedule
    {
        public string key { get; set; }
        public string iataCode { get; set; }
        public string lang { get; set; }
        public string type { get; set; }
        public string date { get; set; } //YYYY-MM-DD
    }
}