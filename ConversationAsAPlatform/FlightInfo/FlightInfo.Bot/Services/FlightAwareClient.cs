using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FlightInfo.Bot.Services
{
    public static class FlightAwareClient
    {
        public static async Task<FlightInformation> GetFlightData(string userName, string apiKey, string tailNumber)
        {
            // Documentation: http://flightxml.flightaware.com/soap/FlightXML2/doc#op_FlightInfo
            // Sample URL: http://flightxml.flightaware.com/json/FlightXML2/FlightInfo?ident=BA49&howMany=1
            string uri = @"http://flightxml.flightaware.com/json/FlightXML2/FlightInfo?ident=" + tailNumber + "&howMany=1";
            string authParam = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", userName, apiKey)));
            var authorization = new AuthenticationHeaderValue("Basic", authParam);
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = authorization;
            var result = await client.GetStringAsync(uri);
            client.Dispose();
            return JsonConvert.DeserializeObject<FlightInformation>(result);
        }

        public static string GetFlightStatus(FlightInformation flightInfo)
        {
            string flightStatus = String.Empty;
            if(
                flightInfo.FlightInfoResult.flights[0].actualdeparturetime == 0)
            {
                flightStatus = "Waiting for departure";
            }
            else if(
                flightInfo.FlightInfoResult.flights[0].actualdeparturetime != 0 &&
                flightInfo.FlightInfoResult.flights[0].actualarrivaltime == 0)
            {
                flightStatus = "En route";
            }
            else if(
                flightInfo.FlightInfoResult.flights[0].actualdeparturetime != 0 &&
                flightInfo.FlightInfoResult.flights[0].actualarrivaltime != 0 && 
                flightInfo.FlightInfoResult.flights[0].actualdeparturetime != flightInfo.FlightInfoResult.flights[0].actualarrivaltime)
            {
                flightStatus = "Landed";
            }
            else
            {
                flightStatus = "Unknown";
            }
            return flightStatus;
        }
    }

    public class FlightInformation
    {
        public Flightinforesult FlightInfoResult { get; set; }
    }

    public class Flightinforesult
    {
        public int next_offset { get; set; }
        public Flight[] flights { get; set; }
    }

    public class Flight
    {
        public string ident { get; set; }
        public string aircrafttype { get; set; }
        public string filed_ete { get; set; }
        public int filed_time { get; set; }
        public int filed_departuretime { get; set; }
        public int filed_airspeed_kts { get; set; }
        public string filed_airspeed_mach { get; set; }
        public int filed_altitude { get; set; }
        public string route { get; set; }
        public int actualdeparturetime { get; set; }
        public int estimatedarrivaltime { get; set; }
        public int actualarrivaltime { get; set; }
        public string diverted { get; set; }
        public string origin { get; set; }
        public string destination { get; set; }
        public string originName { get; set; }
        public string originCity { get; set; }
        public string destinationName { get; set; }
        public string destinationCity { get; set; }
    }
}