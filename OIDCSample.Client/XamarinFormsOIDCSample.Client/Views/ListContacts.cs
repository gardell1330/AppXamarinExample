using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace XamarinFormsOIDCSample.Client.Views
{
    class ListContacts : ContentPage
    {
        private ActivityPageViewModel ViewModel { get; set; }
        public GroupedContacts ListaContactos { get; set; }
        public Profile Account { get; set; }
        public Label UserName { get; set; }
        public ActivityIndicator Loading { get; set; }

        public ListContacts()
        {
            ViewModel = new ActivityPageViewModel();
            BindingContext = ViewModel;

            Task.Factory.StartNew(() => GetAccount());

            Loading = new ActivityIndicator
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Color = Color.White,
                Opacity=5,
                IsVisible = false
            };

            UserName = new Label
            {                
                FontSize = 16,
                Text= "Bienvenido",
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.StartAndExpand
            };            

            Content = new StackLayout {
                Padding = 60,
                Spacing = 10,
                Children = { Loading, UserName,  }
            };

            Loading.SetBinding(ActivityIndicator.IsVisibleProperty, "IsBusy");
            Loading.SetBinding(ActivityIndicator.IsRunningProperty, "IsBusy");

        }

        
        internal async Task GetAccount()
        {
            ViewModel.IsBusy = true;
            var url = "https://api.worldoftanks.com/wot/account/info/";            
            var sBuilder = $"{url}?application_id=715ee34f2bb9baeb9a825cf74b717e75&access_token={App.Token}&language=es&account_id={App.PlayerId}&extra=private.grouped_contacts&fields={WebUtility.UrlEncode("nickname,account_id,ban_info,clan_id,last_battle_time,logout_at,private.grouped_contacts")}";

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(sBuilder))
            using (HttpContent content = response.Content)
            {
                // ... Read the string.
                string result = await content.ReadAsStringAsync();

                // ... Display the result.
                if (result != null)
                {
                    var item = JObject.Parse(result);
                    var idplayer = App.PlayerId;
                    var responseData = JsonConvert.DeserializeObject<Profile>(item["data"][idplayer.ToString()].ToString());
                    Account = responseData;
                    ViewModel.IsBusy = false;
                    UserName.SetValue(Label.TextProperty, Account.nickname);
                }                
            }            
        }
    }
}