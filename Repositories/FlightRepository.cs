using System.Data;
using System.Text.Json;
using Dapper;
using SoftTurFlights.Factory;
using SoftTurFlights.Interfaces;
using SoftTurFlights.Models;
using SoftTurFlights.Services;
using SoftTurFlights.Services.Aviation.Models;
using SoftTurFlights.Utils;
using Cirium = SoftTurFlights.Services.Cirium;

namespace SoftTurFlights.Repositories
{
    public class FlightRepository : IFlightRepository
    {
        private IDbConnection _connection;
        public FlightRepository()
        {
            _connection = new SqlFactory().SqlConnection();
        }
        public dynamic Get(string api_key, DateTime start, DateTime end, string airport, string api_name)
        {
            try
            {
                var flights = new List<FlightModel>();
                var client = new ClientRepository();
                using (_connection)
                {
                    var cliente = client.GetClientByKey(api_key, _connection);
                    if (cliente == null)
                        return "Usuario nao autenticado";
                    else
                    {
                        var query = @"select * 
                          from [SoftTurFlights].[dbo].[flights]
                          where date >= @start 
                          and date <= @end 
                          and api_name = @api_name 
                          and airport_code = @airport";
                        var parameters = new
                        {
                            start = start,
                            end = end,
                            api_name = api_name,
                            airport = airport
                        };
                        flights = _connection.Query<FlightModel>(query, parameters).ToList();
                    }
                }
                return flights;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                throw new Exception(ex.Message);
            }
        }
        public async Task<string> Save(FlightModel flight)
        {
            try
            {
                _connection = new SqlFactory().SqlConnection();

                FlightModel v = null;
                var query = @"select * from [SoftTurFlights].[dbo].[flights]
                          where flight_code = @flight_code
                          and date = @date
                          and airport_code = @airport_code
                          and api_name = @api_name";
                var parameters = new
                {
                    flight_code = flight.flight_code,
                    api_name = flight.api_name,
                    date = flight.date.GetValueOrDefault().Date,
                    airport_code = flight.airport_code
                };
                using (_connection)
                {
                    v = _connection.Query<FlightModel>(query, parameters).FirstOrDefault();
                }
                if (v == null)
                {
                    return await Insert(flight);
                }
                else if ((DateTime.Now - flight.included.GetValueOrDefault()).TotalHours > 1)
                {
                    return await Update(flight);
                }
                else
                    return "success";
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return "error";
            }
        }
        public async Task<string> Insert(FlightModel flight)
        {
            try
            {
                _connection = new SqlFactory().SqlConnection();
                int result = 0;
                using (_connection)
                {
                    var query = @"insert into [SoftTurFlights].[dbo].[flights]
                                values
                                (                                               
                                   @flight_code,
                                   @airport_code,
                                   @type,
                                   @date,
                                   @time,
                                   @included,
                                   @api_name
                                )";
                    var parameters = new
                    {
                        flight_code = flight.flight_code.Replace("*", ""),
                        airport_code = flight.airport_code,
                        type = flight.type,
                        date = flight.date.GetValueOrDefault().Date,
                        time = flight.time,
                        included = flight.included,
                        api_name = flight.api_name
                    };
                    result = await _connection.ExecuteAsync(query, parameters);
                }
                return result > 0 ? "success" : "error";
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return "error";
            }
        }
        public async Task<string> Update(FlightModel flight)
        {
            try
            {
                _connection = new SqlFactory().SqlConnection();
                int result = 0;
                using (_connection)
                {
                    var query = @"update [SoftTurFlights].[dbo].[flights]
                              set time = @time, included = @included
                              where flight_code = @flight_code
                              and date = @date
                              and api_name = @api_name
                              and type = @type";
                    var parameters = new
                    {
                        flight_code = flight.flight_code.Replace("*", ""),
                        airport_code = flight.airport_code,
                        type = flight.type,
                        date = flight.date.GetValueOrDefault().Date,
                        time = flight.time,
                        included = flight.included,
                        api_name = flight.api_name
                    };
                    result = await _connection.ExecuteAsync(query, parameters);
                }
                return result > 0 ? "success" : "error";
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return "error";
            }
        }
        public FlightModel GetFlight(string flight_code, string type, string api_name, DateTime? date)
        {
            try
            {
                var flight = new FlightModel();
                var query = @"select * from [SoftTurFlights].[dbo].[flights]
                          where flight_code = @flight_code
                          and date = @date
                          and api_name = @api_name
                          and type = @type";
                var parameters = new
                {
                    flight_code = flight_code,
                    type = type,
                    api_name = api_name,
                    date = date
                };
                using (_connection)
                {
                    flight = _connection.Query<FlightModel>(query, parameters).FirstOrDefault();
                }
                return flight;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                throw new Exception(ex.Message);
            }
        }
        public bool ExistsRecent(RequestSendFlight req)
        {
            try
            {
                _connection = new SqlFactory().SqlConnection();

                var flight = new FlightModel();
                var query = @"select * from [SoftTurFlights].[dbo].[flights]
                          where flight_code = @flight_code
                          and date = @date
                          and api_name = @api_name";
                var parameters = new
                {
                    flight_code = req.flight_code.Replace(" ", ""),
                    api_name = req.api_name,
                    date = req.flight_date.Date
                };
                using (_connection)
                {
                    flight = _connection.Query<FlightModel>(query, parameters).FirstOrDefault();
                }                
                var res = flight != null && flight.included > DateTime.Now.AddHours(-6);
                return res;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return true;
            }
        }
        public async Task<string> SendFlight(List<RequestSendFlight> req)
        {
            try
            {
                foreach (var it in req)
                {
                    if (ExistsRecent(it))
                        continue;

                    var clientRepository = new ClientRepository();
                    var airports = clientRepository.GetAirports();
                    await GetFlightApi(it, airports.Distinct().ToList());
                    _connection = new SqlFactory().SqlConnection();
                }
                return "success";
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                throw new Exception(ex.Message);
            }
        }
        private async Task<string> GetFlightApi(RequestSendFlight req, List<string> airports)
        {
            try
            {
                var api = new Cirium.Service();
                var flight = await api.getFlight(req, airports);
                if (flight != null)
                {
                    if (flight.scheduledFlights != null)
                    {
                        var voo = flight.scheduledFlights.FirstOrDefault();
                        if (voo != null)
                        {
                            if (airports.Contains(voo.departureAirportFsCode) && airports.Contains(voo.arrivalAirportFsCode))
                            {
                                FlightModel f1 = new()
                                {
                                    airport_code = voo.departureAirportFsCode,
                                    api_name = req.api_name,
                                    date = voo.departureTime.Date,
                                    flight_code = voo.carrierFsCode.Replace("*", "") + voo.flightNumber,
                                    included = DateTime.Now,
                                    time = voo.departureTime.ToString("HH:mm"),
                                    type = "S"
                                };
                                FlightModel f2 = new()
                                {
                                    airport_code = voo.arrivalAirportFsCode,
                                    api_name = req.api_name,
                                    date = voo.arrivalTime.Date,
                                    flight_code = voo.carrierFsCode.Replace("*", "") + voo.flightNumber,
                                    included = DateTime.Now,
                                    time = voo.arrivalTime.ToString("HH:mm"),
                                    type = "C"
                                };
                                await Save(f1);
                                await Save(f2);
                            }
                            else if (airports.Contains(voo.departureAirportFsCode) && !airports.Contains(voo.arrivalAirportFsCode))
                            {
                                FlightModel f = new()
                                {
                                    airport_code = voo.departureAirportFsCode,
                                    api_name = req.api_name,
                                    date = voo.departureTime.Date,
                                    flight_code = voo.carrierFsCode.Replace("*", "") + voo.flightNumber,
                                    included = DateTime.Now,
                                    time = voo.departureTime.ToString("HH:mm"),
                                    type = "S"
                                };
                                await Save(f);
                            }
                            else if (!airports.Contains(voo.departureAirportFsCode) && airports.Contains(voo.arrivalAirportFsCode))
                            {
                                FlightModel f = new()
                                {
                                    airport_code = voo.arrivalAirportFsCode,
                                    api_name = req.api_name,
                                    date = voo.arrivalTime.Date,
                                    flight_code = voo.carrierFsCode.Replace("*", "") + voo.flightNumber,
                                    included = DateTime.Now,
                                    time = voo.arrivalTime.ToString("HH:mm"),
                                    type = "C"
                                };
                                await Save(f);
                            }
                        };
                    }
                    else
                    {
                        Logger.Log(JsonSerializer.Serialize(req), "scheduledNull");
                    }
                }
                else
                {
                    Logger.Log(JsonSerializer.Serialize(req), "flightNull");
                }
                return "success";
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return "error";
            }
        }
    }
}