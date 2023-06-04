using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp.Application.Services;

namespace StudentHousing.Location
{
    public class LocationAppService : ApplicationService, ILocationAppService
    {
        private readonly IConfiguration _configuration;
        public LocationAppService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<int?> GetDistanceInMetersAsync(string fromCoords, string toCoords)
        {
            try
            {
                var client = new RestClient($"{_configuration["TrueWayApiEndpoint"]}/CalculateDrivingMatrix?origins={fromCoords.Replace(",", "%2C")}&destinations={toCoords.Replace(",", "%2C")}");
                var request = new RestRequest();
                request.AddHeader("X-RapidAPI-Key", _configuration["TrueWayApiKey"]);
                request.AddHeader("X-RapidAPI-Host", "trueway-matrix.p.rapidapi.com");
                RestResponse response = client.Execute(request);

                var distance = JObject.Parse(response.Content)["distances"][0].ToString().Replace("[", "").Replace("]", "").Replace("\r", "").Replace("\n", "").Trim();
                Console.WriteLine(response.Content);
                if (int.TryParse(distance, out int d))
                {
                    return await Task.FromResult(d);
                }
                else
                {
                    return null;
                }
            }catch(Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> GetCoordsFromAddress(string Address)
        {
            try
            {
                var client = new RestClient($"https://geocode.maps.co/search?q={Address.Replace(" ", "+")}");
                var request = new RestRequest();
                RestResponse response = client.Execute(request);
                string coord = string.Empty;
                JArray array = JArray.Parse(response.Content);
                foreach (JObject obj in array.Children<JObject>())
                {
                    foreach (JProperty singleProp in obj.Properties())
                    {
                        string name = singleProp.Name;
                        string value = singleProp.Value.ToString();
                        if (name == "lat") coord += value;
                        if (name == "lon") coord += $",{value}";
                    }
                }

                return await Task.FromResult(coord);
            }catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
