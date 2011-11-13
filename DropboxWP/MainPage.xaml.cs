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
using System.Windows.Navigation;
using System.Diagnostics;
using System.IO;
using Microsoft.Phone.Controls;

namespace DropboxWP
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                Debug.WriteLine("Navigated to main page, token: " + e.Uri.ToString());
                NavigationService.RemoveBackEntry();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }

            //string baseUrl = "https://api.dropbox.com/1/account/info";
            //HttpWebRequest request = WebRequest.CreateHttp(baseUrl);
            //request.BeginGetResponse(r =>
            //{
            //    HttpWebRequest httpRequest = (HttpWebRequest)r.AsyncState;
            //    try
            //    {
            //        Debug.WriteLine(request.Headers.ToString());
            //        HttpWebResponse httpResponse = httpRequest.EndGetResponse(r) as HttpWebResponse;
            //        StreamReader reader = new StreamReader(httpResponse.GetResponseStream());
            //        Debug.WriteLine(reader.ReadToEnd());
            //    }
            //    catch (Exception ex)
            //    {
            //        Debug.WriteLine(ex.Message);
            //    }
            //}, request);
        }
    }
}