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
        public LoginPage()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(LoginPage_Loaded);
        }

        void LoginPage_Loaded(object sender, RoutedEventArgs e)
        {
            string baseUrl = "https://api.dropbox.com/1/oauth/request_token";
            string consumerKey = "<enter-key>";
            string consumerSecret = "<enter-secret>";
            long timestamp = GetEpochTime();
            int nonce = new Random().Next(10000000);

            string signedParams = "oauth_consumer_key=" + consumerKey
                + "&oauth_nonce=" + nonce
                + "&oauth_signature_method=HMAC-SHA1"
                + "&oauth_timestamp=" + timestamp
                + "&oauth_version=1.0";

            byte[] signKey = Encoding.UTF8.GetBytes(UpperCaseUrlEncode(consumerSecret) + "&");
            byte[] urlToSign = Encoding.UTF8.GetBytes("GET&" + UpperCaseUrlEncode(baseUrl) + "&" + UpperCaseUrlEncode(signedParams));
            byte[] hash = new HMACSHA1(signKey).ComputeHash(urlToSign);
            string signature = UpperCaseUrlEncode(Convert.ToBase64String(hash, 0, hash.Length));

            string authorizationHeader = "OAuth oauth_version=\"1.0\""
                + ", oauth_consumer_key=\"" + consumerKey + "\""
                + ", oauth_nonce=\"" + nonce + "\""
                + ", oauth_signature_method=\"HMAC-SHA1\""
                + ", oauth_timestamp=\"" + timestamp + "\""
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
                    foreach (string s in list) {
                        string[] nameval = s.Split('=');
                        if (nameval.Length == 2) {
                            if (nameval[0] == "oauth_token") { token = nameval[1]; }
                            else if (nameval[0] == "oauth_token_secret") { secret = nameval[1]; }
                        }
                    }
                    Debug.WriteLine("Token: " + token + ", Secret: " + secret);
                    if (token.Length > 0 && secret.Length > 0)
                    {
                        object[] objArr = { token, secret };
                        Dispatcher.BeginInvoke(new Action(() => webBrowser1.Source = new Uri("https://www.dropbox.com/1/oauth/authorize?oauth_token=" + token + "&oauth_callback=http://localhost")));
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }, request);
        }


        public long GetEpochTime()
        {
            DateTime dtCurTime = DateTime.Now;
            DateTime dtEpochStartTime = Convert.ToDateTime("1/1/1970 8:00:00 AM");
            TimeSpan ts = dtCurTime.Subtract(dtEpochStartTime);

            long epochtime;
            epochtime = ((((((ts.Days * 24) + ts.Hours) * 60) + ts.Minutes) * 60) + ts.Seconds);
            return epochtime;
        }

        public static string UpperCaseUrlEncode(string s)
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

        private void webBrowser1_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
        }

        private void webBrowser1_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            Debug.WriteLine("LoadCompleted: " + e.Uri.ToString());
            if (e.Uri.Host == "localhost")
            {
                Debug.WriteLine("Finished!!!");
            }
        }
    }
}