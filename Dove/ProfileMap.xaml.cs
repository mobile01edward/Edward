using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Device.Location;
using System.Collections.ObjectModel;

namespace Dove
{
    public partial class ProfileMap : PhoneApplicationPage
    {
        public ObservableCollection<LocationData> CollectImagePath { get; set; }

        public ProfileMap()
        {
            InitializeComponent();

            SystemTray.SetIsVisible(this,false);

            //初始化MAP
            UsingMap Us = new UsingMap();
            Us.StartLocationPrifile(GeoPositionAccuracy.Default, map1, MaskPhoto);

        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (MaskPhoto.Visibility == Visibility.Visible)
            {
                MaskPhoto.Visibility = Visibility.Collapsed;
                e.Cancel = true;
            }
        }

        private void MaskPhoto_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MaskPhoto.Visibility = Visibility.Collapsed;
        }
    }

}