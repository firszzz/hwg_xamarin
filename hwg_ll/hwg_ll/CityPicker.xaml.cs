using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace hwg_ll
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CityPicker : ContentPage
    {
        MainPage mainPage = new MainPage();
        public CityPicker()
        {
            InitializeComponent();
        }

        void PickCity(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(city_entry.Text))
            {
                Navigation.PopModalAsync();
                mainPage.GetResponse(city_entry.Text);
                a.Text = "fasfas";  
            }
        }
    }
}