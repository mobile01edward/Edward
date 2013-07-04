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

namespace Dove
{
    public partial class ReviewPage : PhoneApplicationPage
    {
        public ReviewPage()
        {
            InitializeComponent();
            SystemTray.SetIsVisible(this, false);
            initialAppBar();
        }

        private void initialAppBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.IsMenuEnabled = true;
            ApplicationBar.IsVisible = true;
            ApplicationBar.Opacity = 0.8;
            ApplicationBar.Mode = ApplicationBarMode.Default;

            ApplicationBarIconButton Like = new ApplicationBarIconButton();
            Like.IconUri = new Uri("/Image/Barlike.png", UriKind.Relative);
            Like.Text = "Like";
            Like.Click += Like_Click;
            ApplicationBar.Buttons.Add(Like);

            ApplicationBarIconButton comment = new ApplicationBarIconButton();
            comment.IconUri = new Uri("/Image/Barcomments.png", UriKind.Relative);
            comment.Text = "Comment";
            comment.Click += comment_Click;
            ApplicationBar.Buttons.Add(comment);

            ApplicationBarIconButton share = new ApplicationBarIconButton();
            share.IconUri = new Uri("/Image/Barshare.png", UriKind.Relative);
            share.Text = "share";
            share.Click += share_Click;
            ApplicationBar.Buttons.Add(share);
        }

        void share_Click(object sender, EventArgs e)
        {
            
        }

        void comment_Click(object sender, EventArgs e)
        {
            
        }

        void Like_Click(object sender, EventArgs e)
        {
           
        }

        void EditDelicious_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void ReviewIcon_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ProfileData.xaml", UriKind.Relative));
        }
    }
}