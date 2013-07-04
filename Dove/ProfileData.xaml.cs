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
using System.Collections;
using Microsoft.Phone.Shell;
using System.ComponentModel;

namespace Dove
{
    public partial class ProfileData : PhoneApplicationPage
    {
        public ProfileData()
        {
            InitializeComponent();
            SystemTray.SetIsVisible(this,false);
            initialAppBar();

            BackgroundWorker bk = new BackgroundWorker();
            bk.DoWork += (a,b) =>
                {
                    Dispatcher.BeginInvoke(() =>
                        {
                            List<JumpDemo> source = new List<JumpDemo>();
                            source.Add(new JumpDemo() { Name = "A-Name 1", GroupBy = "A" });
                            source.Add(new JumpDemo() { Name = "A-Name 2", GroupBy = "A" });
                            source.Add(new JumpDemo() { Name = "A-Name 3", GroupBy = "A" });
                            source.Add(new JumpDemo() { Name = "A-Name 4", GroupBy = "A" });
                            source.Add(new JumpDemo() { Name = "A-Name 5", GroupBy = "A" });
                            source.Add(new JumpDemo() { Name = "B-Name 1", GroupBy = "B" });
                            source.Add(new JumpDemo() { Name = "B-Name 2", GroupBy = "B" });
                            source.Add(new JumpDemo() { Name = "C-Name 1", GroupBy = "C" });
                            source.Add(new JumpDemo() { Name = "C-Name 2", GroupBy = "C" });
                            source.Add(new JumpDemo() { Name = "C-Name 3", GroupBy = "C" });
                            source.Add(new JumpDemo() { Name = "C-Name 4", GroupBy = "C" });
                            source.Add(new JumpDemo() { Name = "D-Name 1", GroupBy = "D" });
                            source.Add(new JumpDemo() { Name = "D-Name 2", GroupBy = "D" });
                            source.Add(new JumpDemo() { Name = "D-Name 3", GroupBy = "D" });
                            source.Add(new JumpDemo() { Name = "E-Name 1", GroupBy = "E" });
                            source.Add(new JumpDemo() { Name = "E-Name 2", GroupBy = "E" });
                            source.Add(new JumpDemo() { Name = "F-Name 2", GroupBy = "F" });
                            source.Add(new JumpDemo() { Name = "G-Name 2", GroupBy = "G" });
                            source.Add(new JumpDemo() { Name = "G-Name 2", GroupBy = "G" });

                            var groupBy = from jumpdemo in source
                                          group jumpdemo by jumpdemo.GroupBy into c
                                          orderby c.Key
                                          select new Group<JumpDemo>(c.Key, c);

                            this.citiesListGropus.ItemsSource = groupBy;
                        });
                };
            bk.RunWorkerAsync();

        }

        private void tap_JumpListItem(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TextBlock hb = sender as TextBlock;
            MessageBox.Show(hb.Text + " " + "tapped");
        }

        private void initialAppBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.IsMenuEnabled = true;
            ApplicationBar.IsVisible = true;
            ApplicationBar.Opacity = 0.8;
            ApplicationBar.Mode = ApplicationBarMode.Default;

            ApplicationBarIconButton EditDelicious = new ApplicationBarIconButton();
            EditDelicious.IconUri = new Uri("/Image/feature.camera.png", UriKind.Relative);
            EditDelicious.Text = "編輯";
            EditDelicious.Click += EditDelicious_Click;
            //search.Click += new EventHandler(PinToStart_Click);
            ApplicationBar.Buttons.Add(EditDelicious);

            //ApplicationBarIconButton TakePhoto = new ApplicationBarIconButton();
            //TakePhoto.IconUri = new Uri("/Image/feature.camera.png", UriKind.Relative);
            //TakePhoto.Text = "拍照";
            //TakePhoto.Click += new EventHandler(ShutterButton_Click);
            //ApplicationBar.Buttons.Add(TakePhoto);

            //ApplicationBarIconButton Edit = new ApplicationBarIconButton();
            //Edit.IconUri = new Uri("/Image/edit.png", UriKind.Relative);
            //Edit.Text = "編輯";
            //Edit.Click += Edit_Click;
            //ApplicationBar.Buttons.Add(Edit);
        }

        void EditDelicious_Click(object sender, EventArgs e)
        {
           
        }

        private void ReviewItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ReviewPage.xaml", UriKind.Relative));
        }

        private void MyMap_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ProfileMap.xaml", UriKind.Relative));
        }
    }

    public class JumpDemo
    {
        public string Name { get; set; }
        public string GroupBy { get; set; }
    }

    public class Group<T> : IEnumerable<T>
    {
        public Group(string name, IEnumerable<T> items)
        {
            this.Title = name;
            this.Items = new List<T>(items);
        }

        public override bool Equals(object obj)
        {
            Group<T> that = obj as Group<T>;
            return (that != null) && (this.Title.Equals(that.Title));
        }

        public string Title
        {
            get;
            set;
        }

        public IList<T> Items
        {
            get;
            set;
        }

        #region IEnumerable<T> Members
        public IEnumerator<T> GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }
        #endregion

        #region IEnumerable Members
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }
        #endregion
    }

}