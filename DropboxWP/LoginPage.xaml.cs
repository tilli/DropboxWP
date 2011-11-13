using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Diagnostics;
using Microsoft.Phone.Controls;

namespace DropboxWP
{
    public partial class LoginPage : PhoneApplicationPage
    {
        private string requestToken;
        private string requestTokenSecret;
        private string accessToken;
        private string accessTokenSecret;

        public LoginPage()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(LoginPage_Loaded);
        }

        void LoginPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadRequestToken();
        }

        private void LoadRequestToken()
        {
            LoadToken("https://api.dropbox.com/1/oauth/request_token", true);
        }

        private void webBrowser1_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            // Step 2 does not work...
            Debug.WriteLine("LoadCompleted: " + e.Uri.ToString());
            //            if (e.Uri.Host == "localhost")
            if (e.Uri.ToString().EndsWith("#"))
            {
                LoadAccessToken();
            }
        }

        private void LoadAccessToken()
        {
            LoadToken("https://api.dropbox.com/1/oauth/access_token", false);
        }

        private void LoadToken(string baseUrl, bool isRequestToken)
        {
            string consumerKey = "<enter-key>";
            string consumerSecret = "<enter-secret>";
            long timestamp = GetEpochTime();
            int nonce = new Random().Next(10000000);

            string signedParams = "oauth_consumer_key=" + consumerKey
                + "&oauth_nonce=" + nonce
                + "&oauth_signature_method=HMAC-SHA1"
                + "&oauth_timestamp=" + timestamp
                + (isRequestToken ? "" : "&oauth_token=" + requestToken)
                + "&oauth_version=1.0";

            byte[] signKey = Encoding.UTF8.GetBytes(UpperCaseUrlEncode(consumerSecret) + "&" + (isRequestToken ? "" : UpperCaseUrlEncode(requestTokenSecret)));
            byte[] urlToSign = Encoding.UTF8.GetBytes("GET&" + UpperCaseUrlEncode(baseUrl) + "&" + UpperCaseUrlEncode(signedParams));
            byte[] hash = new HMACSHA1(signKey).ComputeHash(urlToSign);
            string signature = UpperCaseUrlEncode(Convert.ToBase64String(hash, 0, hash.Length));

            string authorizationHeader = "OAuth oauth_version=\"1.0\""
                + ", oauth_consumer_key=\"" + consumerKey + "\""
                + ", oauth_nonce=\"" + nonce + "\""
                + ", oauth_signature_method=\"HMAC-SHA1\""
                + ", oauth_timestamp=\"" + timestamp + "\""
                + (isRequestToken ? "" : ", oauth_token=\"" + requestToken + "\"")
                + ", oauth_signature=\"" + signature + "\"";

            Debug.WriteLine("INPUT: " + Encoding.UTF8.GetString(urlToSign, 0, urlToSign.Length));
            Debug.WriteLine("SIGNATURE: " + signature);

            HttpWebRequest request = WebRequest.CreateHttp(baseUrl);
            request.Headers[HttpRequestHeader.Authorization] = authorizationHeader;
            request.AllowReadStreamBuffering = true;
            request.BeginGetResponse(r =>
            {
                HttpWebRequest httpRequest = (HttpWebRequest)r.AsyncState;
                try
                {
                    Debug.WriteLine(request.Headers.ToString());
                    HttpWebResponse httpResponse = httpRequest.EndGetResponse(r) as HttpWebResponse;
                    StreamReader reader = new StreamReader(httpResponse.GetResponseStream());
                    string responseText = reader.ReadToEnd();
                    string[] list = responseText.Split('&');
                    string token = "";
                    string secret = "";
                    foreach (string s in list)
                    {
                        string[] nameval = s.Split('=');
                        if (nameval.Length == 2)
                        {
                            if (nameval[0] == "oauth_token") { token = nameval[1]; }
                            else if (nameval[0] == "oauth_token_secret") { secret = nameval[1]; }
                        }
                    }
                    if (token.Length > 0 && secret.Length > 0)
                    {
                        if (isRequestToken)
                        {
                            // Step 1 complete, got unauthorized token and secret
                            //  -> Step 2, launch web browser
                            requestToken = token;
                            requestTokenSecret = secret;
                            Debug.WriteLine("Unauthorized: Token: " + requestToken + ", Secret: " + requestTokenSecret);
                            Dispatcher.BeginInvoke(new Action(() => webBrowser1.Source = new Uri("https://www.dropbox.com/1/oauth/authorize?oauth_token=" + token + "&oauth_callback=http://localhost")));
                        }
                        else
                        {
                            // Step 3 completed, token and secret where authorized
                            Debug.WriteLine("Authorized: Token: " + token + ", Secret: " + secret);
                            try
                            {
                                accessToken = token;
                                accessTokenSecret = secret;
                                NavigationService.Navigate(new Uri("/MainPage.xaml?token=" + accessToken + "&secret=" + accessTokenSecret, UriKind.Relative));
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }, request);
        }

        private long GetEpochTime()
        {
            DateTime dtCurTime = DateTime.Now;
            DateTime dtEpochStartTime = Convert.ToDateTime("1/1/1970 8:00:00 AM");
            TimeSpan ts = dtCurTime.Subtract(dtEpochStartTime);

            long epochtime;
            epochtime = ((((((ts.Days * 24) + ts.Hours) * 60) + ts.Minutes) * 60) + ts.Seconds);
            return epochtime;
        }

        private static string UpperCaseUrlEncode(string s)
        {
            char[] temp = HttpUtility.UrlEncode(s).ToCharArray();
            for (int i = 0; i < temp.Length - 2; i++)
            {
                if (temp[i] == '%')
                {
                    temp[i + 1] = char.ToUpper(temp[i + 1]);
                    temp[i + 2] = char.ToUpper(temp[i + 2]);
                }
            }
            return new string(temp);
        }
    }
}