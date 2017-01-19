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
using System.Windows.Input;

namespace XamarinFormsOIDCSample.Client.Views
{
    class ListContacts : ContentPage
    {
        private ActivityPageViewModel ViewModel { get; set; }
        public ObservableCollection<Profile> ListaContactos { get; set; }
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
                Text= "Wellcome",
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start
            };

            ListViewContactos = new ListView {
                HorizontalOptions= LayoutOptions.StartAndExpand,
                BindingContext=ViewModel
            };


            var searchBar = new SearchBar
            {
                Placeholder = "Search nickname with 3 charts min"
            };

            searchBar.TextChanged += async (sender, args) => { await SearchContacts(args.NewTextValue); };

            Content = new StackLayout {               
                Spacing = 10,

                Children = { Loading, searchBar, UserName, ListViewContactos  }
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
                    UserName.SetValue(Label.TextProperty, Account.nickname);
                }                
            }            
        }

        internal async Task  GetListaContactos()
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
                    var jObject = JObject.Parse(result);
                    ListaContactos = new ObservableCollection<Profile>();
                    foreach (var item in contactGroup)
                    {
                        var user = JsonConvert.DeserializeObject<Profile>(jObject["data"][item.ToString()].ToString());                        
                        if (UnixTimeStampToDateTime(user.logout_at) < UnixTimeStampToDateTime(user.last_battle_time))
                        {
                            user.status = "off";
                        }
                        else
                        {
                            if (UnixTimeStampToDateTime(user.last_battle_time).AddMinutes(5) <= DateTime.Now)
                            {
                                user.status = "on";
                                if (user.clan_id == Account.clan_id)
                                {
                                    user.status = "on-clan";
                                }                                
                            }
                            else
                            {
                                user.status = "hold";
                            }
                        }

                        ListaContactos.Add(user);                        
                    }
                                        
                    ListViewContactos.SetValue(ListView.ItemsSourceProperty, ListaContactos.OrderBy(r=>r.last_battle_time));
                    ListViewContactos.SetValue(ListView.ItemTemplateProperty, new DataTemplate(typeof(ContactsCell)));
                    ViewModel.IsBusy = false;
                }
            }
        }

        internal async Task SearchContacts(string newTextValue)
        {
            ViewModel.IsBusy = true;
            var url = "https://api.worldoftanks.com/wot/account/list/";
            if (newTextValue.Length < 3) {
                ViewModel.IsBusy = false;
                return;
            }
            var sBuilder = $"{url}?application_id=715ee34f2bb9baeb9a825cf74b717e75&search={WebUtility.UrlEncode(newTextValue)}";
            //&limit=10

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(sBuilder))
            using (HttpContent content = response.Content)
            {
                // ... Read the string.
                string result = await content.ReadAsStringAsync();                
                // ... Display the result.
                if (result != null)
                {
                    var listaGrupos = new ObservableCollection<GroupContacts>();
                    var grupoContactos = new GroupContacts("Contactos");
                    grupoContactos.AddRange(ListaContactos.Where(r=>r.nickname.Contains(newTextValue)));
                    grupoContactos.Add(new Profile { nickname = "Count " + ListaContactos.Count });
                    listaGrupos.Add(grupoContactos);
                    

                    var item = JObject.Parse(result);
                    var listaContactos = JsonConvert.DeserializeObject<List<Profile>>(item["data"].ToString());
                    
                    var grupoExterno = new GroupContacts("Todos");
                    var listaNoContactos = listaContactos.Where(r => !ListaContactos.Any(s => s.account_id == r.account_id)).ToList();
                    grupoExterno.AddRange(listaContactos);
                    grupoExterno.Add(new Profile { nickname = "Count " + listaNoContactos.Count });
                    listaGrupos.Add(grupoExterno);                    

                    ListViewContactos.SetValue(ListView.ItemsSourceProperty, listaGrupos);
                    ListViewContactos.SetValue(ListView.IsGroupingEnabledProperty, true);
                    ListViewContactos.GroupDisplayBinding = new Binding("Type");
                    ListViewContactos.GroupShortNameBinding = new Binding("Type");
                    ListViewContactos.SetValue(ListView.ItemTemplateProperty, new DataTemplate(typeof(ContactsCell)));
                    ViewModel.IsBusy = false;
                }
            }
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(Convert.ToDouble(unixTimeStamp)).ToLocalTime();
            return dtDateTime;
        }
    }
}