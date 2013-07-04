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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace Dove
{
    public partial class MainPage : PhoneApplicationPage
    {
        string ReceiveRequest="";
        SendRequest sen = new SendRequest();
        public string test { get; set; }

        public MainPage()
        {
            InitializeComponent();

            //初始化Application Bar
            initialAppBar();

            //獲取首頁資訊
            BackgroundWorker ba = new BackgroundWorker();
            ba.DoWork += (a, b) =>
            {
                sen.NewRequest("http://www.mobile01.com/rest/mobile01app.php?method=topiclist&f=339&p=1", responseCallback);
            };
            ba.RunWorkerAsync();
        }

        public void responseCallback(IAsyncResult result)
        {
            try
            {
                //获取异步操作返回的的信息
                HttpWebRequest request = (HttpWebRequest)result.AsyncState;

                //结束对 Internet 资源的异步请求
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(result);

                using (Stream stream = response.GetResponseStream())
                {
                    //获取请求信息
                    StreamReader read = new StreamReader(stream);
                    UpdateNewFeed(read.ReadToEnd());

                    response.Close();
                    response.Dispose();
                }
            }
            catch (WebException e)
            {
                //连接失败
            }
            catch (Exception e)
            {
                //异常处理
            }
        }

        ObservableCollection<FleetsCollection> NewsContentList { get; set; }
        public void UpdateNewFeed(string Feed)
        {
            // 將 JSON 字串變成物件
            //JObject jObj = JObject.Parse(Feed);

            NewsContentList = new ObservableCollection<FleetsCollection>();

            //放入Jason字串進入解析器
            var dict = (JObject)JsonConvert.DeserializeObject(Feed);

            //將結果放入列表
            foreach (var obj in dict["topic_list"])//框內為回傳的列表名稱
            {
                NewsContentList.Add(new FleetsCollection()
                {
                    topic_title = obj["topic_title"].ToString()//框內為列表內各項目名稱
                });
            }

            //取出特定位置資訊
            string st = NewsContentList.ElementAt(20).topic_title;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("確定要離開嗎?", "", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                while (NavigationService.BackStack.Any())
                    NavigationService.RemoveBackEntry();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void initialAppBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.IsMenuEnabled = true;
            ApplicationBar.IsVisible = true;
            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.BackgroundColor = Color.FromArgb(255,51,51,51);

            ApplicationBarIconButton EditDelicious = new ApplicationBarIconButton();
            EditDelicious.IconUri = new Uri("/Image/feature.camera.png", UriKind.Relative);
            EditDelicious.Text = "編輯";
            EditDelicious.Click += EditDelicious_Click;
            ApplicationBar.Buttons.Add(EditDelicious);

            ApplicationBarIconButton search = new ApplicationBarIconButton();
            search.IconUri = new Uri("/Image/Barsearch.png", UriKind.Relative);
            search.Text = "Search";
            search.Click += search_Click;
            ApplicationBar.Buttons.Add(search);

            //ApplicationBarIconButton Edit = new ApplicationBarIconButton();
            //Edit.IconUri = new Uri("/Image/edit.png", UriKind.Relative);
            //Edit.Text = "編輯";
            //Edit.Click += Edit_Click;
            //ApplicationBar.Buttons.Add(Edit);
        }

        void search_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SearchPag.xaml", UriKind.Relative));
        }

        void EditDelicious_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/EditDelicious.xaml", UriKind.Relative));
        }

        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MoreBest.xaml", UriKind.Relative));
        }

        private void FinderNearby_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MoreNearby.xaml", UriKind.Relative));
        }

        private void MyIcon_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ProfileData.xaml", UriKind.Relative));
        }

        private void BestItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/FoodDetail.xaml", UriKind.Relative));
        }

        private void FeedItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ReviewPage.xaml", UriKind.Relative));
        }

        private void Mymap_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ProfileMap.xaml", UriKind.Relative));
        }
    }
}