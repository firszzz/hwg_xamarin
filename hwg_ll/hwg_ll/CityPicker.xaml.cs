using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static hwg_ll.MainPage;

namespace hwg_ll
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CityPicker : ContentPage
    {
        
        public CityPicker()
        {
            InitializeComponent();
        }

        void PickCity(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(city_entry.Text))
            {
                CityData cd = new CityData();
                cd.City = city_entry.Text;

                MessagingCenter.Send<CityData>(cd, "ReceiveData");

                Navigation.PopModalAsync();
            }
        }

        public string CityName { get; set; }
    }
}