using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using Xamarin.Forms;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace hwg_ll
{
    public class APIHelper
    {
        readonly HttpClient Client = new HttpClient();
        readonly string appid = "5c9e5c9e14238226c26f3994ab9275ca";

        public async Task<string> Get_response(string city)
        {
            string curr_weather_rq = "https://api.openweathermap.org/data/2.5/weather?q=" + city + "&appid=" + appid;

            string responseTask;

            try
            {
                responseTask = await Client.GetStringAsync(curr_weather_rq);
                return responseTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return await Get_response("Moscow");
            }
        }

        public async Task<string> Get_city(double lat, double lon)
        {
            string curr_city_rq = "https://api.openweathermap.org/data/2.5/weather?lat=" + lat.ToString() + "&lon=" + lon.ToString() + "&appid=" + appid;

            string responseTask = await Client.GetStringAsync(curr_city_rq);

            return responseTask;
        }

        public class APIResponse
        {
            public bool Success => ErrorMessage == null;
            public string ErrorMessage { get; set; }
            public string Response { get; set; }
        }
    }
}
