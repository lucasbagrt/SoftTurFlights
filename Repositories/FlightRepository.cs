using System.Data;
using Dapper;
using SoftTurFlights.Factory;
using SoftTurFlights.Interfaces;
using SoftTurFlights.Models;
using SoftTurFlights.Services;
using SoftTurFlights.Services.Aviation.Models;

namespace SoftTurFlights.Repositories
{
    public class FlightRepository : IFlightRepository
    {
        private readonly IDbConnection _connection;
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
                throw new Exception(ex.Message);
            }
        }
        public void Insert(FlightModel flight)
        {
            try
            {
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
                        flight_code = flight.flight_code,
                        airport_code = flight.airport_code,
                        type = flight.type,
                        date = flight.date,
                        time = flight.time,
                        included = flight.included,
                        api_name = flight.api_name
                    };
                    var result = _connection.Execute(query, parameters);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void Update(FlightModel flight)
        {
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
                    flight_code = flight.flight_code,
                    airport_code = flight.airport_code,
                    type = flight.type,
                    date = flight.date,
                    time = flight.time,
                    included = flight.included,
                    api_name = flight.api_name
                };
                var result = _connection.Execute(query, parameters);
            }
        }
        public FlightModel? GetFlight(string flight_code, string type, string api_name, DateTime? date)
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
    }
}