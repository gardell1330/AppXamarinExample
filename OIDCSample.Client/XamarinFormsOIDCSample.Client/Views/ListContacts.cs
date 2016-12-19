using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace XamarinFormsOIDCSample.Client.Views
{
    class ListContacts : ContentPage
    {
        private ActivityPageViewModel ViewModel { get; set; }
        public ObservableCollection<Contacts> ListaContactos { get; set; }
        public ListView ListViewContactos { get; set; }
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

            ListViewContactos = new ListView();

            Content = new StackLayout {
                Padding = 60,
                Spacing = 10,
                Children = { Loading, UserName, ListViewContactos  }
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
                    await Task.Factory.StartNew(() => GetListaContactos());
                    ViewModel.IsBusy = false;                    
                    ListViewContactos.ItemsSource = ListaContactos;
                    UserName.SetValue(Label.TextProperty, Account.nickname);
                }                
            }            
        }

        private async Task  GetListaContactos()
        {
            var url = "https://api.worldoftanks.com/wot/account/info/";
            var contactGroup = Account.@private.grouped_contacts.ungrouped;
            var contacts = "";
            foreach (var item in contactGroup)
            {
                contacts += item.ToString() + ",";
            }

            var sBuilder = $"{url}?application_id=715ee34f2bb9baeb9a825cf74b717e75&access_token={App.Token}&language=es&account_id={contacts}&fields={WebUtility.UrlEncode("nickname,account_id,ban_info,clan_id,last_battle_time,logout_at,private.grouped_contacts")}";

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(sBuilder))
            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();

                // ... Display the result.
                if (result != null)
                {
                    var item = JObject.Parse(result);
                }
            }
        }
    }
}