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
        private AuthorizeResponse _authResponse;

        public LoginView()
        {
            InitializeComponent();
            btnGetIdToken.Clicked += GetIdToken;
            btnGetAccessToken.Clicked += GetAccessToken;
            btnGetIdTokenAndAccessToken.Clicked += GetIdTokenAndAccessToken;
            wvLogin.Navigating += WvLogin_Navigating;
        }

        private void WvLogin_Navigating(object sender, WebNavigatingEventArgs e)
        {
            if (e.Url.Contains("https://developers.wargaming.net/reference/all/wot/auth/login/?"))
            {
                wvLogin.IsVisible = false;
                var tUrl = e.Url.ToString();
                if (tUrl.Substring(tUrl.IndexOf('?') + 1, 1) == "&")
                {
                    tUrl=tUrl.Remove(tUrl.IndexOf('?') + 1,1);
                }

                _authResponse = new AuthorizeResponse(tUrl);

                App.Token= _authResponse.AccessToken;
                App.PlayerId = Convert.ToInt32(_authResponse.Values["account_id"]);

                Navigation.PushAsync(new ListContacts());
            }
        }

        private void GetIdToken(object sender, EventArgs e)
        {
            // id_tokens don't contain resource scopes, only ask for
            // openid and profile
            StartFlow("id_token", "openid profile");
        }

        private void GetAccessToken(object sender, EventArgs e)
        {
            // access tokens are for resource authorization, only ask for
            // resource scopes
            StartFlow("token", "read write");
        }

        private void GetIdTokenAndAccessToken(object sender, EventArgs e)
        {
            // when asking both, we can ask for identity-related scopes
            // as well as resource scopes
            StartFlow("id_token token", "openid profile read write");
        }

        public void StartFlow(string responseType, string scope)
        {
            var authorizeRequest = new AuthorizeRequest("https://api.worldoftanks.com/wot/auth/login/");

            // dictionary with values for the authorize request
            var dic = new Dictionary<string, string>();
            dic.Add("application_id", "715ee34f2bb9baeb9a825cf74b717e75");
            dic.Add("display", "page");
            dic.Add("expires_at", "1482537600");
            dic.Add("nofollow", "0");
            dic.Add("redirect_uri", "https://developers.wargaming.net/reference/all/wot/auth/login/");

            var authorizeUri = authorizeRequest.Create(dic);

            wvLogin.Source = authorizeUri;
            wvLogin.IsVisible = true;
        }

        
        //public static string DecodeToken(string token)
        //{
        //    var parts = token.Split('.');

        //    string partToConvert = parts[1];
        //    partToConvert = partToConvert.Replace('-', '+');
        //    partToConvert = partToConvert.Replace('_', '/');
        //    switch (partToConvert.Length % 4)
        //    {
        //        case 0:
        //            break;
        //        case 2:
        //            partToConvert += "==";
        //            break;
        //        case 3:
        //            partToConvert += "=";
        //            break;
        //    }

        //    var partAsBytes = Convert.FromBase64String(partToConvert);
        //    var partAsUTF8String = Encoding.UTF8.GetString(partAsBytes, 0, partAsBytes.Count());

        //    return JObject.Parse(partAsUTF8String).ToString();
        //}
    }
}
