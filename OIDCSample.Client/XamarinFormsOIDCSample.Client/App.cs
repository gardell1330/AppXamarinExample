using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using XamarinFormsOIDCSample.Client.Views;

namespace XamarinFormsOIDCSample.Client
{
    public class App : Application
    {
        public static string Token { get; set; }
        public static int PlayerId { get; set; }        

        public App()
        {
            // The root page of your application
            MainPage = new NavigationPage(new LoginView());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
