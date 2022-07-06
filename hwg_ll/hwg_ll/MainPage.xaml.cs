using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace hwg_ll
{
    public partial class MainPage : ContentPage
    {
        bool anim = false;
        float speed;

        public async void Anim_propeller(object sender, System.EventArgs e)
        {
            anim = !anim;

            if (!anim) { anim_btn.Text = "Enable animation"; return; }
            else if (anim) { anim_btn.Text = "Disable animation"; }

            int final_speed = (int)speed * 1000;
            while (anim)
            {
                await propeller.RelRotateTo(360, (uint)final_speed);
            }
        }

        public string ConvertTime(double unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            string dt = dtDateTime.ToString();
            string[] dtL = dt.Split(' ');

            return dtL[0];
        }

        public string FindDirection(int deg)
        {
            double val = Math.Floor((deg / 22.5) + 0.5);
            string[] arr = new string[] { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW" };
            var dir = val % 16;
            return arr[(int)dir];
        }

        public async void Share_info(object sender, System.EventArgs e)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Title = $"Погода в городе {city.Text}",
                Text = $"На данный момент температура в городе {city.Text} равна: {temp.Text}. \n Влажность: {humidity.Text}. \n Скорость ветра: {wind.Text}. \n Направление ветра: {wind_dir.Text}. \n Состояние погоды: {weather.Text}."
            });
        }

        public MainPage()
        {
            InitializeComponent();

            APIHelper aPIHelper = new APIHelper();

            GetResponse();

            async void GetResponse()
            {
                string result = await aPIHelper.Get_response();
                JObject json = JObject.Parse(result);

                string img_icon = "https://openweathermap.org/img/wn/" + json["weather"][0]["icon"].ToString() + "@4x.png";
                int wind_deg = int.Parse(json["wind"]["deg"].ToString());
                string wind_speed = json["wind"]["speed"].ToString();

                speed = float.Parse(wind_speed);
                city.Text = json["name"].ToString();
                float tempCel = float.Parse(json["main"]["temp"].ToString());
                double dt = double.Parse(json["dt"].ToString());
                temp.Text = Math.Round(tempCel - 273.15, 1).ToString();
                weather.Text = json["weather"][0]["main"].ToString();
                img_weather.Source = new UriImageSource
                {
                    CachingEnabled = false,
                    Uri = new System.Uri(img_icon)
                };
                datetime.Text = ConvertTime(dt).ToString();
                humidity.Text = json["main"]["humidity"].ToString() + "%";
                wind.Text = json["wind"]["speed"].ToString() + " m/s";
                wind_dir.Text = FindDirection(wind_deg);
                arrow.Rotation = wind_deg;

                newlay.ResolveLayoutChanges();
            }
        }
    }
}
