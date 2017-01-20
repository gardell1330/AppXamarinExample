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
        private ListContactsViewModel vm { get; set; }
        public ObservableCollection<Profile> ListaContactos { get; set; }
        public ListView ListViewContactos { get; set; }
        public Profile Account { get; set; }
        public Label UserName { get; set; }
        public ActivityIndicator Loading { get; set; }

        public ListContacts()
        {
            vm = new ListContactsViewModel();
            BindingContext = vm;

            Task.Factory.StartNew(() => vm.GetAccount(this));

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
                BindingContext=vm
            };


            var searchBar = new SearchBar
            {
                Placeholder = "Search nickname with 3 charts min"
            };

            searchBar.TextChanged += async (sender, args) => { await vm.SearchContacts(args.NewTextValue, this); };

            Content = new StackLayout {               
                Spacing = 10,

                Children = { Loading, searchBar, UserName, ListViewContactos  }
            };

            Loading.SetBinding(ActivityIndicator.IsVisibleProperty, "IsBusy");
            Loading.SetBinding(ActivityIndicator.IsRunningProperty, "IsBusy");

        }      
    }
}