using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using XamarinFormsOIDCSample.Client.Views;

namespace XamarinFormsOIDCSample.Client
{
    public class ListContactsViewModel : BaseViewModel
    {
        private string _buttonText;

        public string ButtonText
        {
            get { return _buttonText; }
            set
            {
                _buttonText = value;
                // This is very important. It indicates to the app that you've changed the content of this property
                OnPropertyChanged();
            }
        }

        internal async Task GetAccount(ListContacts page)
        {
            IsBusy = true;
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
                    page.Account = responseData;
                    await Task.Factory.StartNew(() => GetListaContactos(page));
                    page.UserName.SetValue(Label.TextProperty, page.Account.nickname);
                }
            }
        }

        internal async Task GetListaContactos(ListContacts page)
        {
            var url = "https://api.worldoftanks.com/wot/account/info/";
            var contactGroup = page.Account.@private.grouped_contacts.ungrouped;
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
                    page.ListaContactos = new ObservableCollection<Profile>();
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
                                if (user.clan_id == page.Account.clan_id)
                                {
                                    user.status = "on-clan";
                                }
                            }
                            else
                            {
                                user.status = "hold";
                            }
                        }

                        page.ListaContactos.Add(user);
                    }

                    page.ListViewContactos.SetValue(ListView.ItemsSourceProperty, page.ListaContactos.OrderBy(r => r.last_battle_time));
                    page.ListViewContactos.SetValue(ListView.ItemTemplateProperty, new DataTemplate(typeof(ContactsCell)));
                    IsBusy = false;
                }
            }
        }

        internal async Task SearchContacts(string newTextValue, ListContacts page)
        {
            IsBusy = true;
            var url = "https://api.worldoftanks.com/wot/account/list/";
            if (newTextValue.Length < 3)
            {
                IsBusy = false;
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
                    var listaFiltrada = page.ListaContactos.Where(r => r.nickname.Contains(newTextValue)).ToList();
                    grupoContactos.AddRange(listaFiltrada);
                    grupoContactos.Add(new Profile { nickname = "Count " + listaFiltrada.Count });
                    listaGrupos.Add(grupoContactos);


                    var item = JObject.Parse(result);
                    var listaContactos = JsonConvert.DeserializeObject<List<Profile>>(item["data"].ToString());

                    var grupoExterno = new GroupContacts("Todos");
                    var listaNoContactos = listaContactos.Where(r => !page.ListaContactos.Any(s => s.account_id == r.account_id)).ToList();
                    grupoExterno.AddRange(listaContactos);
                    grupoExterno.Add(new Profile { nickname = "Count " + listaNoContactos.Count });
                    listaGrupos.Add(grupoExterno);

                    page.ListViewContactos.SetValue(ListView.ItemsSourceProperty, listaGrupos);
                    page.ListViewContactos.SetValue(ListView.IsGroupingEnabledProperty, true);
                    page.ListViewContactos.GroupDisplayBinding = new Binding("Type");
                    page.ListViewContactos.GroupShortNameBinding = new Binding("Type");
                    page.ListViewContactos.SetValue(ListView.ItemTemplateProperty, new DataTemplate(typeof(ContactsCell)));
                    IsBusy = false;
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