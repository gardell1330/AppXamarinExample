using IdentityModel.Client;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace XamarinFormsOIDCSample.Client.Views
{
    public partial class LoginView : ContentPage
    {        
        public LoginViewModel vm { get; set; }

        public LoginView()
        {
            InitializeComponent();
            vm = new LoginViewModel();
            BindingContext = vm;
                        
            wvLogin.Navigating += WvLogin_Navigating;
            wvLogin.Navigated += WvLogin_Navigated;
            wvLogin.Source = vm.StartFlow("id_token", "openid profile");                        
            wvLogin.IsVisible = true;
        }

        private void WvLogin_Navigated(object sender, WebNavigatedEventArgs e)
        {
            wvLogin.Eval(vm.ApplyStyle(e));
        }

        private void WvLogin_Navigating(object sender, WebNavigatingEventArgs e)
        {            
            vm.LoginUser(e, this);            
        }        
    }
}
