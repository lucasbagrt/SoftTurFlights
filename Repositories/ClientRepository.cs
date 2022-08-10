using System.Data;
using Dapper;
using SoftTurFlights.Factory;
using SoftTurFlights.Interfaces;
using SoftTurFlights.Models;
using SoftTurFlights.Utils;

namespace SoftTurFlights.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly IDbConnection _connection;

        public ClientRepository()
        {
            _connection = new SqlFactory().SqlConnection();
        }
        public dynamic Get(string api_key)
        {
            try
            {                
                var clients = new List<ClientModel>();
                var parameter = new
                {
                    api_key = api_key
                };
                using (_connection)
                {
                    var client = GetClientByKey(api_key, _connection);
                    if (client == null)
                        return "Usuario nao autenticado";
                    else
                    {
                        var query = "";
                        if (client.role == "admin")
                        {
                            query = "select * from [SoftTurFlights].[dbo].[clients]";
                            clients = _connection.Query<ClientModel>(query).ToList();
                        }
                        else
                        {
                            query = "select * from [SoftTurFlights].[dbo].[clients] where api_key = @api_key";
                            clients = _connection.Query<ClientModel>(query, parameter).ToList();
                        }
                    }
                }

                return clients;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                throw new Exception(ex.Message);
            }
        }
        public bool Delete(string api_key)
        {
            try
            {
                var query = "delete [SoftTurFlights].[dbo].[clients] where [api_key] = @api_key";
                var parameters = new { api_key = api_key };
                int result = 0;
                using (_connection)
                {
                    result = _connection.Execute(query, parameters);
                }
                return (result != 0 ? true : false);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                throw new Exception(ex.Message);
            }
        }
        public bool Insert(ClientModel client)
        {
            try
            {
                int result = 0;
                using (_connection)
                {
                    var cliente = GetClientByKey(client.api_key, _connection);
                    if (cliente == null || cliente.role != "admin")
                        return false;
                    else
                    {
                        var query = @"insert into [SoftTurFlights].[dbo].[clients]
                                values
                                (
                                   @client,               
                                   @url,
                                   @role,
                                   @api_key,
                                   @aiports
                                )";
                        var parameters = new
                        {
                            client = client.client,
                            url = client.url,
                            role = "client",
                            api_key = Guid.NewGuid().ToString().Split("-")[0],
                            airports = client.airports
                        };
                        result = _connection.Execute(query, parameters);
                    }
                }
                return (result != 0 ? true : false);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                throw new Exception(ex.Message);
            }
        }
        public bool Update(string api_key, string client, string url)
        {            
            var query =
            @"update [SoftTurFlights].[dbo].[clients] set url = @url, client = @client where api_key = @api_key";
            var parameters = new
            {
                url = url,
                client = client,
                api_key = api_key
            };
            int result = 0;
            using (_connection)
            {
                result = _connection.Execute(query, parameters);
            }
            return (result != 0 ? true : false);
        }
        public ClientModel GetClientByKey(string api_key, IDbConnection _connection)
        {
            try
            {                
                var query = @"select * from [SoftTurFlights].[dbo].[clients] where api_key = @api_key";
                var parameter = new
                {
                    api_key = api_key
                };
                var client = _connection.Query<ClientModel>(query, parameter).FirstOrDefault();
                return client;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                throw new Exception(ex.Message);
            }
        }
        public List<string> GetAirports()
        {
            try
            {
                var clients = new List<ClientModel>();
                var res = new List<string>();
                using (_connection)
                {
                    var query = @"select * from [SoftTurFlights].[dbo].[clients]";
                    clients = _connection.Query<ClientModel>(query).ToList();
                }
                var airports = clients.Select(t => t.airports).ToList();
                foreach (var it in airports)
                {
                    if (!string.IsNullOrEmpty(it))
                        res = res.Concat(it!.Split(';')).ToList();
                }
                return res;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                throw new Exception(ex.Message);
            }
        }    
    }
}