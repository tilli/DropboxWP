using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;


namespace DropboxWP
{
    public class FileViewModel : INotifyPropertyChanged
    {
        public FileViewModel()
        {
            this.LeftItems = new ObservableCollection<FileModelItem>();
            this.RightItems = new ObservableCollection<FileModelItem>();
        }

        public ObservableCollection<FileModelItem> LeftItems { get; private set; }
        public ObservableCollection<FileModelItem> RightItems { get; private set; }

        private string _sampleProperty = "Sample Runtime Property Value";

        public string SampleProperty
        {
            get
            {
                return _sampleProperty;
            }
            set
            {
                if (value != _sampleProperty)
                {
                    _sampleProperty = value;
                    NotifyPropertyChanged("SampleProperty");
                }
            }
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        public void LoadData()
        {
            LeftItems.Add(new FileModelItem() { FileName = "Foo.txt" });
            LeftItems.Add(new FileModelItem() { FileName = "Foo.txt" });
            LeftItems.Add(new FileModelItem() { FileName = "Foo.txt" });
            LeftItems.Add(new FileModelItem() { FileName = "Foo.txt" });
            LeftItems.Add(new FileModelItem() { FileName = "Foo.txt" });
            LeftItems.Add(new FileModelItem() { FileName = "Foo.txt" });
            LeftItems.Add(new FileModelItem() { FileName = "Foo.txt" });
            LeftItems.Add(new FileModelItem() { FileName = "Foo.txt" });
            LeftItems.Add(new FileModelItem() { FileName = "Foo.txt" });
            LeftItems.Add(new FileModelItem() { FileName = "Foo.txt" });

            RightItems.Add(new FileModelItem() { FileName = "Bar.txt" });
            RightItems.Add(new FileModelItem() { FileName = "Bar.txt" });
            RightItems.Add(new FileModelItem() { FileName = "Bar.txt" });
            RightItems.Add(new FileModelItem() { FileName = "Bar.txt" });
            RightItems.Add(new FileModelItem() { FileName = "Bar.txt" });
            RightItems.Add(new FileModelItem() { FileName = "Bar.txt" });
            RightItems.Add(new FileModelItem() { FileName = "Bar.txt" });
            RightItems.Add(new FileModelItem() { FileName = "Bar.txt" });
            RightItems.Add(new FileModelItem() { FileName = "Bar.txt" });
            RightItems.Add(new FileModelItem() { FileName = "Bar.txt" });

            this.IsDataLoaded = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}