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
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Device.Location;
using Microsoft.Phone.Controls.Maps;
using System.Windows.Media.Imaging;

namespace Dove
{
    public partial class FoodDetail2 : PhoneApplicationPage
    {
        public FoodDetail2()
        {
            InitializeComponent();
            SystemTray.SetIsVisible(this, false);

            UsingMap Us = new UsingMap();
            Us.StartLocationService(GeoPositionAccuracy.Default, map1, "Detail");

            initialAppBar();
        }

        private void initialAppBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.IsMenuEnabled = true;
            ApplicationBar.IsVisible = true;
            ApplicationBar.Opacity = 0.8;
            ApplicationBar.Mode = ApplicationBarMode.Default;

            ApplicationBarIconButton Edit = new ApplicationBarIconButton();
            Edit.IconUri = new Uri("/Image/compose.png", UriKind.Relative);
            Edit.Text = "編輯";
            Edit.Click += Edit_Click;
            ApplicationBar.Buttons.Add(Edit);

            ApplicationBarIconButton upload = new ApplicationBarIconButton();
            upload.IconUri = new Uri("/Image/Barupload.png", UriKind.Relative);
            upload.Text = "upload";
            upload.Click += upload_Click;
            ApplicationBar.Buttons.Add(upload);

            ApplicationBarIconButton list = new ApplicationBarIconButton();
            list.IconUri = new Uri("/Image/Barlist.png", UriKind.Relative);
            list.Text = "list";
            list.Click += list_Click;
            ApplicationBar.Buttons.Add(list);

            ApplicationBarIconButton share = new ApplicationBarIconButton();
            share.IconUri = new Uri("/Image/Barshare.png", UriKind.Relative);
            share.Text = "Barshare";
            share.Click += share_Click;
            ApplicationBar.Buttons.Add(share);
        }

        void share_Click(object sender, EventArgs e)
        {

        }

        void list_Click(object sender, EventArgs e)
        {

        }

        void upload_Click(object sender, EventArgs e)
        {

        }

        void Edit_Click(object sender, EventArgs e)
        {

        }

        private void FoodItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/FoodDetail.xaml", UriKind.Relative));
        }

        private void ReviewItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ReviewPage.xaml", UriKind.Relative));
        }
    }
}