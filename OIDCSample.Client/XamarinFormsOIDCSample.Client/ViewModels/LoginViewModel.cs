using IdentityModel.Client;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace XamarinFormsOIDCSample.Client.Views
{
    public class LoginViewModel
    {
        public AuthorizeResponse _authResponse { get; set; }
        public static int CountLoad { get; set; }
        public string StartFlow(string responseType, string scope)
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
            return authorizeUri; 
        }

        internal string ApplyStyle(WebNavigatedEventArgs e)
        {
            var script = "";
            if (e.NavigationEvent.ToString() == "NewPage" && CountLoad!=1 && e.Url.Contains("https://na.wargaming.net/id/signin"))
            {
                script ="$(\".layout-airy\").css(\"background\",\"black\");$(\"#common_menu,footer,.signin-page-layout_item__social\").remove();$(\"h1\").text(\"CCMSI\").css(\"color\", \"white\");$(\".signin-page-layout_content\").prepend(\"<h2>Login</h2>\");$(\"h2\").css(\"color\", \"white\");";
                CountLoad = 1;
            }
            if (e.NavigationEvent.ToString() == "NewPage" &&  CountLoad != 2 && e.Url.Contains("https://api.worldoftanks.com/id/1018347922-GARdell/confirm"))
            {
                script = "$(\"p\").remove();$(\"p\").remove();$(\"p\").remove();$(\"p\").remove();$(\"p\").remove();$(\".b - header_content\").remove();$(\"ul\").remove();$(\"#content\").append(\"Desea permitir el acceso a esta aplicacion?\")";
                CountLoad = 2;
            }
            return script;
        }

        public void LoginUser(WebNavigatingEventArgs e, ContentPage page)
        {            
            if (e.Url.Contains("https://developers.wargaming.net/reference/all/wot/auth/login/?"))
            {                
                var tUrl = e.Url.ToString();
                if (tUrl.Substring(tUrl.IndexOf('?') + 1, 1) == "&")
                {
                    tUrl = tUrl.Remove(tUrl.IndexOf('?') + 1, 1);
                }

                var auth = new AuthorizeResponse(tUrl);
                App.Token = auth.AccessToken;
                App.PlayerId = Convert.ToInt32(auth.Values["account_id"]);
                page.Navigation.PushAsync(new ListContacts());                
            }            
        }



        internal WebViewSource UrlAuthorize()        
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
            return authorizeUri;
        }
    }
}