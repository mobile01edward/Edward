using Microsoft.Phone.Controls.Maps;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Dove
{
    class UsingMap
    {
        GeoCoordinateWatcher watcher;
        //GeoCoordinateWatcher Myplace;
        Pushpin pin1 = new Pushpin();
        Pushpin pin2 = new Pushpin();
        MapPolyline poly = new MapPolyline();
        Map map1=new Map();
        Grid MaskPhoto = new Grid();

        public void StartLocationPrifile(GeoPositionAccuracy accuracy, Map Mymap, Grid Mask)
        {
            if (watcher == null)
            {
                watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);
                watcher.MovementThreshold = 20;
                watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
                watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(Profile_PositionChanged);
            }
            watcher.Start();

            map1 = Mymap;
            map1.CredentialsProvider = new ApplicationIdCredentialsProvider("AqLI5h-oHjJZPRgaGtsL6XEjwx6uzyurKRRW7sHpXxvLxi4UxbQ8py9nxa2vhzSG");
            map1.Mode = new RoadMode();

            MaskPhoto = Mask;
        }

        public void StartLocationService(GeoPositionAccuracy accuracy, Map Mymap,string MapType)
        {
            if (watcher == null)
            {
                watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);
                watcher.MovementThreshold = 20;
                watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);

                if (MapType == "NotDetail")
                {
                    watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
                }
                else if (MapType == "Detail")
                {
                    watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(Detailwatcher_PositionChanged);
                }
                else if (MapType == "Profile")
                {
                    watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(Profile_PositionChanged);
                }
            }
            watcher.Start();
            
            map1 = Mymap;
            map1.CredentialsProvider = new ApplicationIdCredentialsProvider("AqLI5h-oHjJZPRgaGtsL6XEjwx6uzyurKRRW7sHpXxvLxi4UxbQ8py9nxa2vhzSG");
            map1.Mode = new RoadMode();
        }

        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    MessageBox.Show("Location Service is not enabled on the device");
                    break;

                case GeoPositionStatus.NoData:
                    MessageBox.Show(" The Location Service is working, but it cannot get location data.");
                    break;
            }
        }

        Pushpin WhereAmI = new Pushpin();
        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            if (e.Position.Location.IsUnknown)
            {
                MessageBox.Show("Please wait while your prosition is determined....");
                return;
            }

            this.map1.Center = new GeoCoordinate(e.Position.Location.Latitude, e.Position.Location.Longitude);

            if (this.map1.Children.Count != 0)
            {
                var pushpin = map1.Children.FirstOrDefault(p => (p.GetType() == typeof(Pushpin) && ((Pushpin)p).Tag.ToString() == "locationPushpin"));

                if (pushpin != null)
                {
                    this.map1.Children.Remove(pushpin);
                }
            }

            //Pushpin WhereAmI = new Pushpin();
            //WhereAmI.Background = new SolidColorBrush(Colors.Purple);
            WhereAmI.Background = null;
            BitmapImage bit = new BitmapImage() { UriSource = new Uri("/Image/currentLocation.png",UriKind.Relative) };
            WhereAmI.Content = new Image() { Source = bit, Stretch = Stretch.None, Margin = new Thickness(-28, 0, 0, -85)};
            WhereAmI.Tag = "locationPushpin";
            WhereAmI.Location = watcher.Position.Location;
            try
            {
                this.map1.Children.Add(WhereAmI);
            }
            catch { }

            List<Location> PlaceList = new List<Location>();
            PlaceList.Add(new Location() { Lat = 25.080057, Lon = 121.568590 });
            PlaceList.Add(new Location() { Lat = 25.079036, Lon = 121.567084 });
            PlaceList.Add(new Location() { Lat = 25.083299, Lon = 121.568010 });
            PlaceList.Add(new Location() { Lat = 25.081722, Lon = 121.569280 });
            PlaceList.Add(new Location() { Lat = 25.082476, Lon = 121.566451 });

            for (int a = 0; a < PlaceList.Count(); a++)
            {
                Pushpin locationPushpin = new Pushpin();
                locationPushpin.Background = null;
                locationPushpin.Content = AddPoint(a+1);
                locationPushpin.Tag = "locationPushpin";
                locationPushpin.Location = new GeoCoordinate(PlaceList.ElementAt(a).Lat, PlaceList.ElementAt(a).Lon);
                this.map1.Children.Add(locationPushpin);
            }

            this.map1.SetView(watcher.Position.Location, 17.0);
        }

        void Detailwatcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            if (e.Position.Location.IsUnknown)
            {
                MessageBox.Show("Please wait while your prosition is determined....");
                return;
            }

            this.map1.Center = new GeoCoordinate(e.Position.Location.Latitude, e.Position.Location.Longitude);

            if (this.map1.Children.Count != 0)
            {
                var pushpin = map1.Children.FirstOrDefault(p => (p.GetType() == typeof(Pushpin) && ((Pushpin)p).Tag.ToString() == "locationPushpin"));

                if (pushpin != null)
                {
                    this.map1.Children.Remove(pushpin);
                }
            }

            //地點圖示設定
            Grid gr = new Grid();
            Uri imgUri = new Uri("Image/Detaillocation.png", UriKind.Relative);
            BitmapImage imgSourceR = new BitmapImage(imgUri);
            Image imgBrush = new Image() { Source = imgSourceR, Stretch = Stretch.None };
            gr.Children.Add(imgBrush);
            gr.Margin = new Thickness(-33, 0, 0, -32);
            //WhereAmI.Background = new SolidColorBrush(Colors.Purple);
            WhereAmI.Background = null;
            WhereAmI.Content = gr;
            WhereAmI.Tag = "locationPushpin";
            WhereAmI.Location = watcher.Position.Location;
            try
            {
                this.map1.Children.Add(WhereAmI);
            }
            catch { }


            this.map1.SetView(watcher.Position.Location, 17.0);
        }

        void Profile_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            if (e.Position.Location.IsUnknown)
            {
                MessageBox.Show("Please wait while your prosition is determined....");
                return;
            }

            this.map1.Center = new GeoCoordinate(e.Position.Location.Latitude, e.Position.Location.Longitude);

            if (this.map1.Children.Count != 0)
            {
                var pushpin = map1.Children.FirstOrDefault(p => (p.GetType() == typeof(Pushpin) && ((Pushpin)p).Tag.ToString() == "locationPushpin"));

                if (pushpin != null)
                {
                    this.map1.Children.Remove(pushpin);
                }
            }

            //Pushpin WhereAmI = new Pushpin();
            //WhereAmI.Background = new SolidColorBrush(Colors.Purple);
            WhereAmI.Background = null;
            BitmapImage bit = new BitmapImage() { UriSource = new Uri("/Image/currentLocation.png", UriKind.Relative) };
            WhereAmI.Content = new Image() { Source = bit, Stretch = Stretch.None, Margin = new Thickness(-28, 0, 0, -85) };
            WhereAmI.Tag = "locationPushpin";
            WhereAmI.Location = watcher.Position.Location;
            try
            {
                this.map1.Children.Add(WhereAmI);
            }
            catch { }

            List<Location> PlaceList = new List<Location>();
            PlaceList.Add(new Location() { Lat = 25.080057, Lon = 121.568590 });
            PlaceList.Add(new Location() { Lat = 25.079036, Lon = 121.567084 });
            PlaceList.Add(new Location() { Lat = 25.083299, Lon = 121.568010 });
            PlaceList.Add(new Location() { Lat = 25.081722, Lon = 121.569280 });
            PlaceList.Add(new Location() { Lat = 25.082476, Lon = 121.566451 });
            PlaceList.Add(new Location() { Lat = 25.082024, Lon = 121.566095 });
            PlaceList.Add(new Location() { Lat = 25.081592, Lon = 121.566215 });

            for (int a = 0; a < PlaceList.Count(); a++)
            {
                Pushpin locationPushpin = new Pushpin();
                locationPushpin.Background = null;
                //locationPushpin.Background = new SolidColorBrush(Colors.Purple);
                locationPushpin.Content = AddImagePoint(a + 1);
                locationPushpin.Tag = "locationPushpin";
                locationPushpin.Location = new GeoCoordinate(PlaceList.ElementAt(a).Lat, PlaceList.ElementAt(a).Lon);
                this.map1.Children.Add(locationPushpin);
            }

            this.map1.SetView(watcher.Position.Location, 17.0);
        }

        public class Location
        {
            public double Lat { get; set; }
            public double Lon { get; set; }
        }

        private Grid AddPoint(int count)
        {
            Grid gr = new Grid();
            Uri imgUri = new Uri("Image/pin.png", UriKind.Relative);
            BitmapImage imgSourceR = new BitmapImage(imgUri);
            Image imgBrush = new Image() { Source = imgSourceR, Stretch = Stretch.UniformToFill };
            imgBrush.Width = 33;
            imgBrush.Height = 47;
            TextBlock te = new TextBlock() { Text = count.ToString() };
            te.HorizontalAlignment = HorizontalAlignment.Center;
            te.Margin = new Thickness(0, 10, 0, 0);
            gr.Children.Add(imgBrush);
            gr.Children.Add(te);
            gr.Margin = new Thickness(-35,0,0,-35);

            return gr;
        }

        private Grid AddImagePoint(int count)
        {
            Grid gr = new Grid();

            //圖片參數設定
            Uri imgUri = new Uri(string.Format("TestImage/TestBest{0}.jpg", count), UriKind.Relative);
            BitmapImage imgSourceR = new BitmapImage(imgUri);
            Image imgBrush = new Image() { Source = imgSourceR, Stretch = Stretch.UniformToFill };
            imgBrush.Width = 70;
            imgBrush.Height = 70;
            imgBrush.Tap += imgBrush_Tap;
            imgBrush.Tag = count;

            //圖片文字設定
            Ellipse eli = new Ellipse() { Width = 30, Height = 30, Fill = new SolidColorBrush(Colors.Red) ,Margin=new Thickness(65,-65,0,0)};
            TextBlock te = new TextBlock() { Text = count.ToString() };
            te.HorizontalAlignment = HorizontalAlignment.Center;
            te.Margin = new Thickness(65, -10, 0, 0);

            //放入區塊
            gr.Children.Add(imgBrush);
            gr.Children.Add(eli);
            gr.Children.Add(te);
            gr.Margin = new Thickness(-5, -8, 0, -35);

            return gr;
        }

        public ObservableCollection<ProfileImage> CollectImagePath { get; set; }
        void imgBrush_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MaskPhoto.Visibility = Visibility.Visible;

            //將所有樣版設為不可見
            for (int i = 1; i < 7; i++)
            {
                var stac = MaskPhoto.FindName(string.Format("sample{0}",i)) as ScrollViewer;
                stac.Visibility = Visibility.Collapsed;
            }
            var st = MaskPhoto.FindName("sample7") as ListBox;
            st.Visibility = Visibility.Collapsed;


            //一群組相片數判斷使用哪種樣板
            Image imgBrush=(Image)sender;
            switch (imgBrush.Tag.ToString())
            {
                case "1":
                    var stac1 = MaskPhoto.FindName("sample1") as ScrollViewer;
                    stac1.Visibility = Visibility.Visible;
                    break;

                case "2":
                    var stac2 = MaskPhoto.FindName("sample2") as ScrollViewer;
                    stac2.Visibility = Visibility.Visible;
                    break;

                case "3":
                    var stac3 = MaskPhoto.FindName("sample3") as ScrollViewer;
                    stac3.Visibility = Visibility.Visible;
                    break;

                case "4":
                    var stac4 = MaskPhoto.FindName("sample4") as ScrollViewer;
                    stac4.Visibility = Visibility.Visible;
                    break;

                case "5":
                    var stac5 = MaskPhoto.FindName("sample5") as ScrollViewer;
                    stac5.Visibility = Visibility.Visible;
                    break;

                case "6":
                    var stac6 = MaskPhoto.FindName("sample6") as ScrollViewer;
                    stac6.Visibility = Visibility.Visible;
                    break;

                case "7":
                    var stac7 = MaskPhoto.FindName("sample7") as ListBox;
                    stac7.Visibility = Visibility.Visible;

                    //加入相片
                    CollectImagePath = new ObservableCollection<ProfileImage>();
                    int count = 1;
                    for (int a = 0; a < 4; a++)//迴圈總數=(相片總數/3)餘數+1
                    {
                        CollectImagePath.Add(new ProfileImage()
                        {
                            ImagePath0 = string.Format("TestImage/TestBest{0}.jpg", count),
                            ImagePath1 = string.Format("TestImage/TestBest{0}.jpg", count+1),
                            ImagePath2 = string.Format("TestImage/TestBest{0}.jpg", count+2),
                        }); ;

                        count = count + 3;//一列三張相片
                        stac7.ItemsSource = CollectImagePath;
                    }

                    break;
            }

        }

        private Grid MyLocation()
        {
            Grid gr = new Grid();

            Ellipse el1 = new Ellipse() { Fill = new SolidColorBrush(Color.FromArgb(255, 80, 80, 80)), Width = 30, Height = 30};
            Ellipse el2 = new Ellipse() { Fill = new SolidColorBrush(Color.FromArgb(255, 100, 184, 54)), Width = 23, Height = 23};
            gr.VerticalAlignment = VerticalAlignment.Center;
            gr.HorizontalAlignment = HorizontalAlignment.Center;
            gr.Children.Add(el1);
            gr.Children.Add(el2);
            gr.Margin = new Thickness(-28, 0, 0, -85);

            return gr;
        }

    }

    public class ProfileImage
    {
        public string ImagePath0 { get; set; }
        public string ImagePath1 { get; set; }
        public string ImagePath2 { get; set; }
    }
}
