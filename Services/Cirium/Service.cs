using System.Net;
using RestSharp;
using Newtonsoft.Json;
using System.Web;
using SoftTurFlights.Services.Cirium.Models;
using SoftTurFlights.Utils;
using SoftTurFlights.Models;

namespace SoftTurFlights.Services.Cirium
{
    public class Service
    {
        private const string Url = "https://api.flightstats.com/flex/";
        public string BodyRequest { get; set; } = "";
        public string appId { get; set; } = "XXXX";
        public string appKey { get; set; } = "XXXX";
        protected static void PreConfigurationTls12()
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
        }
        public Service()
        {
            PreConfigurationTls12();
        }
        private async Task<T> Execute<T>(string url, RestSharp.Method method, DataFormat format, object body = null, string content_type = "application/json")
        {
            var client = new RestClient(url);
            var request = new RestRequest(url, method);
            request.AddHeader("appId", appId);
            request.AddHeader("appKey", appKey);
            var response = await client.ExecuteAsync(request);
            if (response.ErrorException != null)
            {
                Logger.Log(response.ErrorException);
            }
            var content = response.Content;
            Logger.Log(content, "response");
            try
            {
                return JsonConvert.DeserializeObject<T>(content!);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                throw new Exception(ex.Message);
            }
        }
        public async Task<ResultGetFlight> getFlight(RequestSendFlight req, List<string> airports)
        {
            var action = "schedules/rest/v1/";
            var flightCode = req.flight_code.Replace(" ", "");
            var count = flightCode.ToCharArray().Length;
            var carrier = flightCode.Substring(0, 2);
            var flightNumber = flightCode.Replace(carrier, "");

            var url = Url + action + string.Format("json/flight/{0}/{1}/departing/{2}/{3}/{4}",
                carrier, flightNumber, req.flight_date.Year, req.flight_date.Month < 10 ? "0" + req.flight_date.Month.ToString() : req.flight_date.Month,
                req.flight_date.Day < 10 ? "0" + req.flight_date.Day.ToString() : req.flight_date.Day);

            var result = await this.Execute<ResultGetFlight>(url, Method.Get, DataFormat.Json);
            if (result.scheduledFlights != null && result.scheduledFlights.Count > 0)
            {
                var voo = result.scheduledFlights.FirstOrDefault();
                if (voo.departureTime.Date < voo.arrivalTime.Date && !airports.Contains(voo.departureAirportFsCode))
                {
                    var url_arriving = Url + action + string.Format("json/flight/{0}/{1}/arriving/{2}/{3}/{4}",
                    carrier, flightNumber, req.flight_date.Year, req.flight_date.Month < 10 ? "0" + req.flight_date.Month.ToString() : req.flight_date.Month,
                    req.flight_date.Day < 10 ? "0" + req.flight_date.Day.ToString() : req.flight_date.Day);

                    return await this.Execute<ResultGetFlight>(url_arriving, Method.Get, DataFormat.Json);
                }
            }
            return result;
        }
    }
}