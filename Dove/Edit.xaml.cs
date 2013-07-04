using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using System.IO;
using System.Collections.ObjectModel;
using Microsoft.Phone;
using System.Windows.Resources;
using Microsoft.Xna.Framework.Media;
using System.ComponentModel;
using System.Threading;

namespace Dove
{
    public partial class Edit : PhoneApplicationPage
    {
        public ObservableCollection<PhotoPath> CollectImagePath { get; set; }

        public Edit()
        {
            InitializeComponent();

            //初始化相片空間
            CollectImagePath = new ObservableCollection<PhotoPath>();

            //取得選取的照片
            GetScrollPhoto();

            //控制系統上方Bar
            SystemTray.SetIsVisible(this, false);
        }

        private void ShutterButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/EditDelicious.xaml", UriKind.Relative));
            //NavigationService.GoBack();
        }

        private void CheckSubmit_Click(object sender, EventArgs e)
        {
            //NavigationService.Navigate(new Uri("/EditDelicious.xaml", UriKind.Relative));
            NavigationService.GoBack();
        }

        int SelectCount = 0;
        private void GetScrollPhoto()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += (s, g) =>
                {
                    SelectCount = 0;
                    for (int a = App.savedCounter - 1; a >= 0; a--)
                    {
                        int a2 = a;
                        Dispatcher.BeginInvoke(() =>
                            {
                                //判斷圖片是否已經選取
                                if (App.MyPho.ElementAt(a2).Selected)
                                {
                                    MediaLibrary mediaLibrary = new MediaLibrary();

                                    using (Stream photo = mediaLibrary.Pictures.ElementAt(App.savedCounter - 1 - a2).GetThumbnail())
                                    {
                                        BitmapImage Scrollbit = new BitmapImage();
                                        Scrollbit.SetSource(photo);
                                        Scrollbit.CreateOptions = BitmapCreateOptions.BackgroundCreation;

                                        CollectImagePath.Add(new PhotoPath()
                                        {
                                            ImagePath = Scrollbit
                                        });
                                        PhotoScroller.ItemsSource = CollectImagePath;

                                        //計算選擇相片數量
                                        SelectCount++;

                                        photo.Close();
                                        photo.Dispose();
                                    }
                                }
                            });
                        Thread.Sleep(5);
                    }
                };
            worker.RunWorkerAsync();
            worker.RunWorkerCompleted += (s,g) =>
                {
                    //無選取照片顯示訊息
                    if (SelectCount <= 0)
                    {
                        SelectMessage.Visibility = Visibility;
                    }
                };
        }

        private void ListBoxItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/PickPlace.xaml", UriKind.Relative));
        }

        private void PlaceName_Loaded(object sender, RoutedEventArgs e)
        {
            //傳入所選地點
            PlaceName.Text = App.PickYourPlace;
        }

        private void Rate0_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BitmapImage bi0 = new BitmapImage();
            bi0.UriSource = new Uri("/Image/RateIcon0.png", UriKind.Relative);

            BitmapImage bi1 = new BitmapImage();
            bi1.UriSource = new Uri("/Image/RateIcon1.png", UriKind.Relative);

            Rate0.Source = bi1;
            Rate0.Tag = "1";
            Rate1.Source = bi0;
            Rate1.Tag = "0";
            Rate2.Source = bi0;
            Rate2.Tag = "0";
            Rate3.Source = bi0;
            Rate3.Tag = "0";
            Rate4.Source = bi0;
            Rate4.Tag = "0";
        }

        private void Rate1_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BitmapImage bi0 = new BitmapImage();
            bi0.UriSource = new Uri("/Image/RateIcon0.png", UriKind.Relative);

            BitmapImage bi1 = new BitmapImage();
            bi1.UriSource = new Uri("/Image/RateIcon1.png", UriKind.Relative);

            Rate0.Source = bi1;
            Rate0.Tag = "1";
            if (Rate1.Tag.ToString() == "0")
            {
                Rate1.Source = bi1;
                Rate1.Tag = "1";
            }
            else
            {
                Rate1.Source = bi0;
                Rate1.Tag = "0";
            }
            
            Rate2.Source = bi0;
            Rate2.Tag = "0";
            Rate3.Source = bi0;
            Rate3.Tag = "0";
            Rate4.Source = bi0;
            Rate4.Tag = "0";
        }

        private void Rate2_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BitmapImage bi0 = new BitmapImage();
            bi0.UriSource = new Uri("/Image/RateIcon0.png", UriKind.Relative);

            BitmapImage bi1 = new BitmapImage();
            bi1.UriSource = new Uri("/Image/RateIcon1.png", UriKind.Relative);

            Rate0.Source = bi1;
            Rate0.Tag = "1";
            Rate1.Source = bi1;
            Rate1.Tag = "1";

            if (Rate2.Tag.ToString() == "0")
            {
                Rate2.Source = bi1;
                Rate2.Tag = "1";
            }
            else
            {
                Rate2.Source = bi0;
                Rate2.Tag = "0";
            }

            Rate3.Source = bi0;
            Rate3.Tag = "0";
            Rate4.Source = bi0;
            Rate4.Tag = "0";
        }

        private void Rate3_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BitmapImage bi0 = new BitmapImage();
            bi0.UriSource = new Uri("/Image/RateIcon0.png", UriKind.Relative);
            BitmapImage bi1 = new BitmapImage();
            bi1.UriSource = new Uri("/Image/RateIcon1.png", UriKind.Relative);

            Rate0.Source = bi1;
            Rate0.Tag = "1";
            Rate1.Source = bi1;
            Rate1.Tag = "1";
            Rate2.Source = bi1;
            Rate2.Tag = "1";

            if (Rate3.Tag.ToString() == "0")
            {
                Rate3.Source = bi1;
                Rate3.Tag = "1";
            }
            else
            {
                Rate3.Source = bi0;
                Rate3.Tag = "0";
            }

            Rate4.Source = bi0;
            Rate4.Tag = "0";
        }

        private void Rate4_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BitmapImage bi0 = new BitmapImage();
            bi0.UriSource = new Uri("/Image/RateIcon0.png", UriKind.Relative);
            BitmapImage bi1 = new BitmapImage();
            bi1.UriSource = new Uri("/Image/RateIcon1.png", UriKind.Relative);

            Rate0.Source = bi1;
            Rate0.Tag = "1";
            Rate1.Source = bi1;
            Rate1.Tag = "1";
            Rate2.Source = bi1;
            Rate2.Tag = "1";
            Rate3.Source = bi1;
            Rate3.Tag = "1";
            if (Rate4.Tag.ToString() == "0")
            {
                Rate4.Source = bi1;
                Rate4.Tag = "1";
            }
            else
            {
                Rate4.Source = bi0;
                Rate4.Tag = "0";
            }

        }

        private void RateThis(Image im, string a, string b, string c, string d, string e)
        {
            if (Rate0.Tag.ToString() == a && Rate1.Tag.ToString() == b && Rate2.Tag.ToString() == c && Rate3.Tag.ToString() == d && Rate4.Tag.ToString() == e)
            {
                BitmapImage bi = new BitmapImage();
                bi.UriSource = new Uri("/Image/RateIcon1.png", UriKind.Relative);
                im.Source = bi;
                im.Tag = "1";
            }
           
        }

        private void UnRateThis(Image im, string a, string b, string c, string d, string e)
        {
            if (Rate0.Tag.ToString() == a && Rate1.Tag.ToString() == b && Rate2.Tag.ToString() == c && Rate3.Tag.ToString() == d && Rate4.Tag.ToString() == e)
            {
                BitmapImage bi = new BitmapImage();
                bi.UriSource = new Uri("/Image/RateIcon0.png", UriKind.Relative);
                im.Source = bi;
                im.Tag = "0";
            }
        }
    }
}