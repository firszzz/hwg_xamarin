using Newtonsoft.Json.Linq;
using System;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Threading;

namespace hwg_ll
{
    public partial class MainPage : ContentPage
    {
        float speed;
        public string city = new CityData().City;
        readonly APIHelper aPIHelper = new APIHelper();
        CancellationTokenSource cts;

        public class CityData
        {
            public string City { get; set; } = "Moscow";
        }

        private void PickCity(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new CityPicker());

            MessagingCenter.Subscribe<CityData>(this, "ReceiveData", (value) =>
            {
                city = value.City;
                GetResponse(city);
            });
        }

        private async void GetCoord()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                cts = new CancellationTokenSource();
                var location = await Geolocation.GetLocationAsync(request, cts.Token);

                MessagingCenter.Subscribe<CityData>(this, "ReceiveData", (value) =>
                {
                    city = value.City;
                    GetResponse(city);
                });

                if (location != null)
                {
                    string result = await aPIHelper.Get_city(location.Latitude, location.Longitude);
                    JObject json = JObject.Parse(result);

                    if (city != json["name"].ToString())
                    {
                        CityData cd = new CityData
                        {
                            City = json["name"].ToString()
                        };

                        MessagingCenter.Send<CityData>(cd, "ReceiveData");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Anim_propeller(object sender, System.EventArgs e)
        {
            int final_speed = (int)speed * 1000;
            
            if (cb_anim.IsChecked)
            {
                cb_label.Text = "Animation is enabled";
                rotate();
            }
            else if (!cb_anim.IsChecked)
            {
                cb_label.Text = "Animation is disabled";
                ViewExtensions.CancelAnimations(propeller);
            }

            async void rotate()
            {
                while (cb_anim.IsChecked)
                {
                    await propeller.RelRotateTo(360, (uint)final_speed);
                }
            } 
        }

        public bool CheckInternetConnection()
        {
            var current = Connectivity.NetworkAccess;

            bool res = true;

            if (current != NetworkAccess.Internet)
            {
                DisplayAlert("Internet connection", "Inernet connection is disabled", "Ok");
                res = false;
            }

            return res;
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

        public void CalculateSS(double dt, double sr, double ss)
        {
            
        }

        public async void GetResponse(string city)
        {
            if (CheckInternetConnection())
            {
                string result = await aPIHelper.Get_response(city);

                JObject json = JObject.Parse(result);

                string wind_speed = json["wind"]["speed"].ToString();
                string img_icon = "https://openweathermap.org/img/wn/" + json["weather"][0]["icon"].ToString() + "@4x.png";

                int wind_deg = int.Parse(json["wind"]["deg"].ToString());

                double sunrise = double.Parse(json["sys"]["sunrise"].ToString()) + 36000;
                double sunset = double.Parse(json["sys"]["sunset"].ToString()) + 36000;
                double dt = double.Parse(json["dt"].ToString()) + 36000;

                float tempCel = float.Parse(json["main"]["temp"].ToString());

                speed = float.Parse(wind_speed);

                city_name.Text = json["name"].ToString();
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

                if (dt < (sunrise + sunset) / 2)
                {
                    if ((sunrise + sunset) / 2 - dt > 13718)
                    {
                        sun.TranslationX = -120;
                        sun.TranslationY = 75;
                    }
                    else if ((sunrise + sunset) / 2 - dt < 13718)
                    {
                        sun.TranslationX = -55;
                        sun.TranslationY = 0;
                    }
                }
                else if (dt > (sunrise + sunset) / 2)
                {
                    if (dt - (sunrise + sunset) / 2 > 13718)
                    {
                        sun.TranslationX = 120;
                        sun.TranslationY = 75;
                    }
                    else if (dt - (sunrise + sunset) / 2 < 13718)
                    {
                        sun.TranslationX = 55;
                        sun.TranslationY = 0;
                    }
                }

                if (dt > sunset || dt < sunrise)
                {
                    sun.TranslationX = 0;
                    sun.TranslationY = 100;
                }
            }
            else { return; }

            newlay.ResolveLayoutChanges();
        }

        public MainPage()
        {
            InitializeComponent();

            GetCoord();
            GetResponse(city);

            Device.StartTimer(new TimeSpan(0, 0, 300), () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    GetResponse(city);
                });

                return true;
            });
        }
    }
}
