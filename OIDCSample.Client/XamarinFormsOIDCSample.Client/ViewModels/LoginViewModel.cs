using IdentityModel.Client;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace XamarinFormsOIDCSample.Client.Views
{
    public class LoginViewModel
    {
        public bool LoginUser(WebNavigatingEventArgs e, AuthorizeResponse auth)
        {
            if (e.Url.Contains("https://developers.wargaming.net/reference/all/wot/auth/login/?"))
            {                
                var tUrl = e.Url.ToString();
                if (tUrl.Substring(tUrl.IndexOf('?') + 1, 1) == "&")
                {
                    tUrl = tUrl.Remove(tUrl.IndexOf('?') + 1, 1);
                }

                auth = new AuthorizeResponse(tUrl);

                App.Token = auth.AccessToken;
                App.PlayerId = Convert.ToInt32(auth.Values["account_id"]);                
                return true;
            }
            return false;
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