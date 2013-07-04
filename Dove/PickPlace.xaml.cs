using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Controls.Maps;
using System.Windows.Media;
using System.Device.Location;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;

namespace Dove
{
    public partial class PickPlace : PhoneApplicationPage
    {
        public ObservableCollection<LocationData> CollectImagePath { get; set; }

        public PickPlace()
        {
            InitializeComponent();

            CollectImagePath = new ObservableCollection<LocationData>();
            for (int a = 0; a < 10; a++)
            {
                CollectImagePath.Add(new LocationData()
                {
                    Name = string.Format("我是店名 {0}",a),
                    Address = string.Format("我是地址 {0}", a),
                });

                PickList.ItemsSource = CollectImagePath;
            }

            initialAppBar();
        }

        private void initialAppBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.IsMenuEnabled = true;
            ApplicationBar.IsVisible = true;
            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.BackgroundColor = Color.FromArgb(255, 51, 51, 51);

            ApplicationBarIconButton search = new ApplicationBarIconButton();
            search.IconUri = new Uri("/Image/Barsearch.png", UriKind.Relative);
            search.Text = "Search";
            search.Click += search_Click;
            ApplicationBar.Buttons.Add(search);

            ApplicationBarIconButton location = new ApplicationBarIconButton();
            location.IconUri = new Uri("/Image/Barlocate.png", UriKind.Relative);
            location.Text = "Location";
            location.Click += location_Click;
            ApplicationBar.Buttons.Add(location);
        }

        void location_Click(object sender, EventArgs e)
        {
           
        }

        void search_Click(object sender, EventArgs e)
        {
           
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lis = (ListBox)sender;

            if (lis.SelectedIndex == -1)
                return;

            ListBoxItem currentSelectedListBoxItem = this.PickList.ItemContainerGenerator.ContainerFromIndex(lis.SelectedIndex) as ListBoxItem;

            if (currentSelectedListBoxItem == null)
                return;

            TextBlock te = new TextBlock();
            te = FindDescendant<TextBlock>(currentSelectedListBoxItem).FindName("StoreName") as TextBlock;
            App.PickYourPlace = te.Text;

            NavigationService.GoBack();
        }

        public T FindDescendant<T>(DependencyObject obj) where T : DependencyObject
        {
            // Check if this object is the specified type
            if (obj is T)
                return obj as T;

            // Check for children
            int childrenCount = VisualTreeHelper.GetChildrenCount(obj);
            if (childrenCount < 1)
                return null;

            // First check all the children
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child is T)
                    return child as T;
            }

            // Then check the childrens children
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = FindDescendant<T>(VisualTreeHelper.GetChild(obj, i));
                if (child != null && child is T)
                    return child as T;
            }

            return null;
        }
    }

    public class LocationData
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }
}