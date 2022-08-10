using SoftTurFlights.Interfaces;
using SoftTurFlights.Models;
using SoftTurFlights.Repositories;
using SoftTurFlights.Services.Aviation;
using SoftTurFlights.Services.Aviation.Models;
using SoftTurFlights.Utils;

namespace SoftTurFlights.Background
{
    public class Processador : BackgroundService, IDisposable
    {
        private Timer _timer;
        ClientRepository client_repository = new ClientRepository();
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //_timer = new Timer(startThreads, null, TimeSpan.Zero, TimeSpan.FromHours(12));
        }
        private void startThreads(object state)
        {
            //getSchedules();
        }
        private async void getSchedules()
        {
            try
            {
                Logger.Log("STARTING GET SCHEDULES");
                var airports = client_repository.GetAirports();
                var service = new Service();
                var flights = new List<FlightModel>();
                foreach (var airport in airports!)
                {
                    Logger.Log("CALLING API");
                    var dt = await service.getSchedules(
                       new RequestGetSchedule
                       {
                           iataCode = airport,
                           key = service.Token
                       });
                    flights = flights.Concat(dt!.Select(t => new FlightModel
                    {
                        airport_code = airport,
                        api_name = "aviation",
                        date = t.type == "arrival" ? t.arrival!.scheduledTime.Date : t.departure!.scheduledTime.Date,
                        flight_code = t.flight!.iataNumber!.ToUpper(),
                        included = DateTime.Now,
                        time = t.type == "arrival" ? t.arrival!.scheduledTime.ToString("HH:mm") : t.departure!.scheduledTime.ToString("HH:mm"),
                        type = t.type == "arrival" ? "C" : "S"
                    }).ToList()).ToList();
                }
                foreach (var flight in flights)
                {
                    try
                    {
                        FlightRepository flight_repository = new FlightRepository();
                        if (flight_repository.GetFlight(flight!.flight_code!, flight!.type!, flight!.api_name!, flight.date) == null)
                        {
                            flight_repository = new FlightRepository();
                            flight_repository.Insert(flight);
                        }
                        else
                        {
                            flight_repository = new FlightRepository();
                            flight_repository.Update(flight);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                        continue;
                    }
                }                
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            finally
            {
                getFutureSchedules();
            }
        }
        private async void getFutureSchedules()
        {
            try
            {
                Logger.Log("STARTING GET FUTURE SCHEDULES");
                ClientRepository client_repository = new ClientRepository();
                var airports = client_repository.GetAirports();
                var service = new Service();
                var flights = new List<FlightModel>();
                var lst_dates = new List<string>();
                var date_base = DateTime.Now.AddDays(8);
                lst_dates.Add(date_base.ToString("yyyy-MM-dd"));
                for (int i = 1; i < 5; i++)
                {
                    lst_dates.Add(date_base.AddDays(i).ToString("yyyy-MM-dd"));
                };
                foreach (var airport in airports!)
                {
                    foreach (var it in lst_dates)
                    {
                        Logger.Log("CALLING API");
                        var departure = (from c in await service.getFutureSchedules(
                           new RequestFutureSchedule
                           {
                               iataCode = airport,
                               key = service.Token,
                               date = it,
                               type = "departure",
                               lang = "pt"
                           })
                                         select new ResultGetFutureSchedules
                                         {
                                             aircraft = c.aircraft,
                                             airline = c.airline,
                                             arrival = c.arrival,
                                             codeshared = c.codeshared,
                                             departure = c.departure,
                                             flight = c.flight,
                                             dsSiglaAeroporto = airport,
                                             dtVoo = Convert.ToDateTime(it),
                                             tpVoo = "departure",
                                             weekday = c.weekday
                                         });

                        Logger.Log("CALLING API");
                        var arrival = (from c in await service.getFutureSchedules(
                           new RequestFutureSchedule
                           {
                               iataCode = airport,
                               key = service.Token,
                               date = it,
                               type = "arrival",
                               lang = "pt"
                           })
                                       select new ResultGetFutureSchedules
                                       {
                                           aircraft = c.aircraft,
                                           airline = c.airline,
                                           arrival = c.arrival,
                                           codeshared = c.codeshared,
                                           departure = c.departure,
                                           flight = c.flight,
                                           dsSiglaAeroporto = airport,
                                           dtVoo = Convert.ToDateTime(it),
                                           tpVoo = "arrival",
                                           weekday = c.weekday
                                       });
                        var voos = departure!.Concat(arrival!);
                        flights = flights.Concat(voos!.Where(t => !string.IsNullOrEmpty(t.flight!.iataNumber) && (!string.IsNullOrEmpty(t.arrival!.scheduledTime) 
                        || !string.IsNullOrEmpty(t.departure!.scheduledTime))).Select(t => new FlightModel
                        {
                            airport_code = airport,
                            api_name = "aviation",
                            date = Convert.ToDateTime(it),
                            flight_code = t.flight!.iataNumber!.ToUpper(),
                            included = DateTime.Now,
                            time = t.tpVoo == "arrival" ? Convert.ToDateTime(t.arrival!.scheduledTime).ToString("HH:mm") : Convert.ToDateTime(t.departure!.scheduledTime).ToString("HH:mm"),
                            type = t.tpVoo == "arrival" ? "C" : "S"
                        }).ToList()).ToList();
                    }
                }
                foreach (var flight in flights)
                {
                    try
                    {
                        FlightRepository flight_repository = new FlightRepository();
                        if (flight_repository.GetFlight(flight!.flight_code!, flight!.type!, flight!.api_name!, flight.date) == null)
                        {
                            flight_repository = new FlightRepository();
                            flight_repository.Insert(flight);
                        }
                        else
                        {
                            flight_repository = new FlightRepository();
                            flight_repository.Update(flight);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }
    }
}