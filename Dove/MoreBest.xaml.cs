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
using Microsoft.Phone.Controls.Maps;
using System.Device.Location;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Shell;
using System.ComponentModel;
using System.Threading;

namespace Dove
{
    public partial class MoreBest : PhoneApplicationPage
    {
        bool ShowMap=false;

        public MoreBest()
        {
            InitializeComponent();

            //初始化MAP
            UsingMap Us = new UsingMap();
            Us.StartLocationService(GeoPositionAccuracy.Default, map1, "NotDetail");

            //初始化Application Bar
            initialAppBar();

            //Photo選取動畫啟動
            var story = PrepareCloseStory();
            story.Begin();
            var Main = MainStory();
            Main.Begin();
            if (!ShowMap)
            {
                StoryboardDown.Begin();
                ShowMap = true;
            }
            else
            {
                StoryboardUp.Begin();
                ShowMap = false;
            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (!ShowMap)
            {
                //地圖動畫啟動
                var story = PrepareCloseStory();
                story.Begin();

                //主畫面動畫啟動
                var Main = MainStory();
                Main.Begin();

                if (!ShowMap)
                {
                    StoryboardDown.Begin();
                    ShowMap = true;
                }
                else
                {
                    StoryboardUp.Begin();
                    ShowMap = false;
                }
                e.Cancel = true;
            }
        }

        //Code for initialization, capture completed, image availability events; also setting the source for the viewfinder.
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
        }

        CubicEase _EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
        private Storyboard PrepareCloseStory()
        {
            if (!ShowMap)
            {
                Storyboard story = new Storyboard();
                DoubleAnimation animation;

                //動畫參數設定
                animation = new DoubleAnimation();
                animation.From =0;
                animation.To = -520;
                animation.Duration = new Duration(TimeSpan.FromMilliseconds(500));
                animation.EasingFunction = _EasingFunction;
                Storyboard.SetTarget(animation, this.popupTransform);
                Storyboard.SetTargetProperty(animation, new PropertyPath("TranslateY"));
                story.Children.Add(animation);

                return story;
            }
            else
            {
                Storyboard story = new Storyboard();
                DoubleAnimation animation;

                //動畫參數設定
                animation = new DoubleAnimation();
                animation.From = -520;
                animation.To = 0;
                animation.Duration = new Duration(TimeSpan.FromMilliseconds(500));
                animation.EasingFunction = _EasingFunction;
                Storyboard.SetTarget(animation, this.popupTransform);
                Storyboard.SetTargetProperty(animation, new PropertyPath("TranslateY"));
                story.Children.Add(animation);

                return story;
            }
        }

        private Storyboard MainStory()
        {
            if (!ShowMap)
            {
                Storyboard story = new Storyboard();
                DoubleAnimation animation;

                //動畫參數設定
                animation = new DoubleAnimation();
                animation.From = 520;
                animation.To = 0;
                animation.Duration = new Duration(TimeSpan.FromMilliseconds(600));
                animation.EasingFunction = _EasingFunction;
                Storyboard.SetTarget(animation, this.MainTransform);
                Storyboard.SetTargetProperty(animation, new PropertyPath("TranslateY"));
                story.Children.Add(animation);

                MenuSelect.IsHitTestVisible = true;

                return story;
            }
            else
            {
                Storyboard story = new Storyboard();
                DoubleAnimation animation;

                //動畫參數設定
                animation = new DoubleAnimation();
                animation.From = 0;
                animation.To = 520;
                animation.Duration = new Duration(TimeSpan.FromMilliseconds(400));
                animation.EasingFunction = _EasingFunction;
                Storyboard.SetTarget(animation, this.MainTransform);
                Storyboard.SetTargetProperty(animation, new PropertyPath("TranslateY"));
                story.Children.Add(animation);

                MenuSelect.IsHitTestVisible = false;


                return story;
            }
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
            ApplicationBar.Buttons.Add(EditDelicious);

            ApplicationBarIconButton refresh = new ApplicationBarIconButton();
            refresh.IconUri = new Uri("/Image/refresh.png", UriKind.Relative);
            refresh.Text = "refresh";
            refresh.Click += refresh_Click;
            ApplicationBar.Buttons.Add(refresh);

            //ApplicationBarIconButton Edit = new ApplicationBarIconButton();
            //Edit.IconUri = new Uri("/Image/edit.png", UriKind.Relative);
            //Edit.Text = "編輯";
            //Edit.Click += Edit_Click;
            //ApplicationBar.Buttons.Add(Edit);
        }

        void refresh_Click(object sender, EventArgs e)
        {
           
        }

        void EditDelicious_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/EditDelicious.xaml", UriKind.Relative));
        }

        private void map1_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
           
        }

        private void BestItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/FoodDetail.xaml", UriKind.Relative));
        }

        private void ShowMap_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //地圖動畫啟動
            var story = PrepareCloseStory();
            story.Begin();

            //主畫面動畫啟動
            var Main = MainStory();
            Main.Begin();

            if (!ShowMap)
            {
                StoryboardDown.Begin();
                ShowMap = true;
            }
            else
            {
                StoryboardUp.Begin();
                ShowMap = false;
            }
        }

        private void Search_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/SearchPag.xaml", UriKind.Relative));
        }

        private void filter_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/FilterPag.xaml", UriKind.Relative));
        }

    }
}