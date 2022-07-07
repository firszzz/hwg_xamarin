using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Threading;

namespace hwg_ll
{
    public partial class MainPage : ContentPage
    {
        float speed;

        private void PickCity(object sender, EventArgs e)
        {
            var cityPicker = new CityPicker();

            Navigation.PushModalAsync(cityPicker);
        }

        public async void Anim_propeller(object sender, System.EventArgs e)
        {
            int final_speed = (int)speed * 1000;

            if (anim_cb.IsChecked)
            {
                text_cb.Text = "Animation is enabled";

                await propeller.RotateTo(360, 800, Easing.Linear);
            }
            else if (!anim_cb.IsChecked)
            {
                text_cb.Text = "Animation is disabled";
            }
        }

        public string ConvertDTime(double unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            string dt = dtDateTime.ToString();
            string[] dtL = dt.Split(' ');

            return dtL[0];
        }

        public string ConvertSTime(double unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            string dt = dtDateTime.ToString();
            string[] dtL = dt.Split(' ');

            string[] hm = dtL[1].Split(':');

            return hm[0] + ":" + hm[1] + " " + dtL[2];
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
                Title = $"Погода в городе {city_name.Text}",
                Text = $"На данный момент температура в городе {city_name.Text} равна: {temp.Text}. \n Влажность: {humidity.Text}. \n Скорость ветра: {wind.Text}. \n Направление ветра: {wind_dir.Text}. \n Состояние погоды: {weather.Text}."
            });
        }

        public async void GetResponse(string city = "Moscow")
        {
            APIHelper aPIHelper = new APIHelper();

            string result = await aPIHelper.Get_response(city);
            JObject json = JObject.Parse(result);

            string img_icon = "https://openweathermap.org/img/wn/" + json["weather"][0]["icon"].ToString() + "@4x.png";
            int wind_deg = int.Parse(json["wind"]["deg"].ToString());
            string wind_speed = json["wind"]["speed"].ToString();
            double sunrise = double.Parse(json["sys"]["sunrise"].ToString()) + 36000;
            double sunset = double.Parse(json["sys"]["sunset"].ToString()) + 36000;
            double dt = double.Parse(json["dt"].ToString()) + 36000;

            speed = float.Parse(wind_speed);
            city_name.Text = json["name"].ToString();
            float tempCel = float.Parse(json["main"]["temp"].ToString());
            temp.Text = Math.Round(tempCel - 273.15, 1).ToString();
            weather.Text = json["weather"][0]["main"].ToString();
            img_weather.Source = new UriImageSource
            {
                CachingEnabled = false,
                Uri = new System.Uri(img_icon)
            };
            datetime.Text = ConvertDTime(dt).ToString();
            humidity.Text = json["main"]["humidity"].ToString() + "%";
            wind.Text = json["wind"]["speed"].ToString() + " m/s";
            wind_dir.Text = FindDirection(wind_deg);
            arrow.Rotation = wind_deg;
            sunrise_time.Text = ConvertSTime(sunrise);
            sunset_time.Text = ConvertSTime(sunset);

            if (dt > sunset || dt < sunrise)
            {
                sun_lay.Children.Clear();
            }

            if (dt < (sunrise + sunset) / 2)
            {
                if ((sunrise + sunset) / 2 - dt > 13718)
                {
                    sun.TranslationX = -120;
                    sun.TranslationY = 70;
                }
                else if ((sunrise + sunset) / 2 - dt < 13718)
                {
                    sun.TranslationX = -60;
                    sun.TranslationY = 0;
                }
            }
            else if (dt > (sunrise + sunset) / 2)
            {
                if (dt - (sunrise + sunset) / 2 > 13718)
                {
                    sun.TranslationX = 120;
                    sun.TranslationY = 70;
                }
                else if (dt - (sunrise + sunset) / 2 < 13718)
                {
                    sun.TranslationX = 60;
                    sun.TranslationY = 0;
                }
            }

            newlay.ResolveLayoutChanges();
        }

        public MainPage()
        {
            InitializeComponent();

            GetResponse();

            Device.StartTimer(new TimeSpan(0, 0, 300), () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    GetResponse();
                });

                return true;
            });
        }
    }
}
