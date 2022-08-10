using System.Net;
using RestSharp;
using Newtonsoft.Json;
using System.Web;
using SoftTurFlights.Services.Aviation.Models;
using SoftTurFlights.Utils;

namespace SoftTurFlights.Services.Aviation
{
    public class Service
    {
        private const string Url = "http://aviation-edge.com/v2/public/";
        public string BodyRequest { get; set; } = "";
        public string Token { get; set; } = "xxxxx";
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
            var response = await client.ExecuteAsync(request);            
            if (response.ErrorException != null)
            {                
                Logger.Log(response.ErrorException);                
            }
            var content = response.Content;            
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
        public async Task<List<ResultGetSchedule>> getSchedules(RequestGetSchedule obj)
        {            
            var action = "timetable?";
            var url = Url + action + getQueryStringSchedule(obj);
            return await this.Execute<List<ResultGetSchedule>>(url, Method.Get, DataFormat.Json);       
        }
        public async Task<List<ResultGetFutureSchedules>> getFutureSchedules(RequestFutureSchedule obj)
        {         
            var action = "flightsFuture?";
            var url = Url + action + getQueryStringFutureSchedule(obj);
            return await this.Execute<List<ResultGetFutureSchedules>>(url, Method.Get, DataFormat.Json);
        }
        public static string getQueryStringSchedule(RequestGetSchedule obj)
        {
            var properties = from p in obj.GetType().GetProperties()
                             where p.GetValue(obj, null) != null
                             select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null)?.ToString());
            return String.Join("&", properties.ToArray());
        }
        public static string getQueryStringFutureSchedule(RequestFutureSchedule obj)
        {
            var properties = from p in obj.GetType().GetProperties()
                             where p.GetValue(obj, null) != null
                             select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null)?.ToString());
            return String.Join("&", properties.ToArray());
        }
    }
}