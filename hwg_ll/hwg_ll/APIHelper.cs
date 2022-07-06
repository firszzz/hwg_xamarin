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
        HttpClient Client = new HttpClient();

        public async Task<string> Get_response()
        {
            string appid = "2f5c4fab286fa279ae7141338e7e967a";
            List<Coord_Response> coords = JsonConvert.DeserializeObject<List<Coord_Response>>(await Get_coord());
            double lat = coords[0].lat;
            double lon = coords[0].lon;

            string curr_weather_rq = "https://api.openweathermap.org/data/2.5/weather?lat=" + lat.ToString() + "&lon=" + lon.ToString() + "&appid=" + appid;

            string responseTask = await Client.GetStringAsync(curr_weather_rq);

            return responseTask;
        }

        public async Task<string> Get_coord(string city = "Moscow")
        {
            string appid = "2f5c4fab286fa279ae7141338e7e967a";

            string coord_rq = "https://api.openweathermap.org/geo/1.0/direct?q=" + city + "&limit=1&appid=" + appid;
            string get_coord = await Client.GetStringAsync(coord_rq);
            //Coord_Response coords = JsonConvert.DeserializeObject<Coord_Response>(get_coord);
            return get_coord;
        }

        public class APIResponse
        {
            public bool Success => ErrorMessage == null;
            public string ErrorMessage { get; set; }
            public string Response { get; set; }
        }
    }
}
