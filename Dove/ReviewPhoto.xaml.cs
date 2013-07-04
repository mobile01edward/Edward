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
using System.IO;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;
using Microsoft.Phone;
using System.Threading;

namespace Dove
{
    public partial class ReviewPhoto : PhoneApplicationPage
    {
        int col = 0;
        int row = 0;
        int count = 0;
        int leng = 0;
        int ImagTg = 0;
        //WriteableBitmap[] ImageArry;
        Stream[] ImageArry;
        bool Re = false;
        bool Re2 = false;
        bool Re3 = false;
        bool Re4 = false;

        public ReviewPhoto()
        {
            InitializeComponent();
            initialAppBar();

            BackgroundWorker ba = new BackgroundWorker();
            ba.DoWork += (a,b) =>
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        AddPhoto();
                    });
                };
            ba.RunWorkerAsync();
        }

        private void initialAppBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.IsMenuEnabled = true;
            ApplicationBar.IsVisible = true;
            ApplicationBar.Opacity = 0.7;

            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBarIconButton MyCamara = new ApplicationBarIconButton();
            MyCamara.IconUri = new Uri("/Image/feature.camera.png", UriKind.Relative);
            MyCamara.Text = "相機";
            MyCamara.Click += MyCamara_Click;
            ApplicationBar.Buttons.Add(MyCamara);

            ApplicationBarIconButton Edit = new ApplicationBarIconButton();
            Edit.IconUri = new Uri("/Image/compose.png", UriKind.Relative);
            Edit.Text = "編輯";
            Edit.Click += Edit_Click;
            ApplicationBar.Buttons.Add(Edit);

        }

        void MyCamara_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        void Edit_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Edit.xaml", UriKind.Relative));
        }

        public byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);

            // 设置当前流的位置为流的开始 
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        } 

        private void AddPhoto()
        {
            ProgressBar.Visibility = Visibility.Visible;
            var mediaLibrary = new MediaLibrary();

            //初始化相片存放空間
            ImageArry = new Stream[mediaLibrary.Pictures.Count()];

            //放入相片
            //count = 0;
            //foreach (var p in mediaLibrary.Pictures)
            //{
            //    //Stream photo = p.GetImage();
            //    Stream photo = p.GetThumbnail();
            //    ImageArry[count] = photo;
            //    count++;
            //}

            //放入相片
            for (int a = App.savedCounter - 1; a >= 0; a--)
            {
                Stream photo = mediaLibrary.Pictures.ElementAt(a).GetThumbnail();
                byte[] data = StreamToBytes(photo);

                using (MemoryStream stream = new MemoryStream(data))
                {
                    BitmapImage bit = new BitmapImage();
                    bit.SetSource(stream);

                    ImageArry[count] = photo;

                    stream.Close();
                }
                count++;
            }

            //ImageArry = new WriteableBitmap[App.savedCounter];
            //for (int a = 0; a < App.savedCounter; a++)
            //{
            //    string fileName = a + "_th.jpg";
            //    using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            //    {
            //        using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(fileName, FileMode.Open, isoStore))
            //        {
            //            WriteableBitmap bitmap = PictureDecoder.DecodeJpeg(stream);
            //            ImageArry[count] = bitmap;
            //            count++;
            //            stream.Close();
            //        }
            //    }
            //}

           
            //else
            //{
            //    for (int a = 0; a < App.savedCounter; a++)
            //    {
            //        //初始化相片是否選擇
            //        App.MyPho.Add(new MySelectPhotoList
            //        {
            //            Selected = false
            //        });
            //    }
            //}

            //for (int a = 0; a < App.savedCounter; a++)
            //{
            //    string fileName = a + "_th.jpg";
            //    IsolatedStorageSettings.ApplicationSettings.TryGetValue<Stream>(fileName, out ImageArry[count]);
            //    count++;
            //}

            //計算需多少列
            if (count % 4 != 0)
            {
                leng = (int)(ImageArry.Length / 4) + 1;
            }
            else
            {
                leng = count / 4;
            }

            //增加列數
            ContentPanel.Children.Clear();
            for (int i = 0; i < leng; i++)
            {
                if (i < 15 && i >= 0)
                {
                    RowDefinition rd = new RowDefinition();
                    //rd.Height = new GridLength(130, GridUnitType.Star);
                    ContentPanel.RowDefinitions.Add(rd);
                }
                else if (i < 30 && i >= 15)
                {
                    RowDefinition rd = new RowDefinition();
                    ContentPanel2.RowDefinitions.Add(rd);
                }
                else if (i < 45 && i >= 30)
                {
                    RowDefinition rd = new RowDefinition();
                    ContentPanel3.RowDefinitions.Add(rd);
                }
                else
                {
                    RowDefinition rd = new RowDefinition();
                    ContentPanel4.RowDefinitions.Add(rd);
                }
            }

            var bw = new BackgroundWorker();
            bw.DoWork += (s, a) =>
            {
                for (int i = 0; i < count; i++)
                {
                    int i2 = i;
                    Dispatcher.BeginInvoke(() =>
                    {
                        ThreadProc(i2);
                    });

                    Thread.Sleep(5);
                }

            };
            bw.RunWorkerCompleted += (s, a) =>
            {
                ProgressBar.Visibility = Visibility.Collapsed;
            };
            bw.RunWorkerAsync();
        }

        private void ThreadProc(int a)
        {
            Dispatcher.BeginInvoke(() =>
            {
                //ProgressBar.IsVisible = true;
                //string fileName = a + "_th.jpg";
                //string fileName = "Shared/ShellContent/" + a + "_th.jpg";

                //編列圖片放置位址
                if (a < 60 && a >= 0)
                {
                    if (Re == false)
                    {
                        col = 0;
                        row = 0;
                        Re = true;
                    }
                    if (col >= 4)
                    {
                        col = 0;
                        row++;
                    }

                    AddImageToGrid(ContentPanel, ImageArry[a]);
                }
                else if (a >= 60 && a < 120)
                {
                    if (Re2 == false)
                    {
                        col = 0;
                        row = 0;
                        Re2 = true;
                    }
                    if (col >= 4)
                    {
                        col = 0;
                        row++;
                    }
                    AddImageToGrid(ContentPanel2, ImageArry[a]);
                }
                else if (a >= 120 && a < 180)
                {
                    if (Re3 == false)
                    {
                        col = 0;
                        row = 0;
                        Re3 = true;
                    }
                    if (col >= 4)
                    {
                        col = 0;
                        row++;
                    }

                    AddImageToGrid(ContentPanel3, ImageArry[a]);
                }
                else
                {
                    if (Re4 == false)
                    {
                        col = 0;
                        row = 0;
                        Re4 = true;
                    }
                    if (col >= 4)
                    {
                        col = 0;
                        row++;
                    }

                    AddImageToGrid(ContentPanel4, ImageArry[a]);
                }
            });
        }

        private void AddImageToGrid(Grid im, Stream fileStream)
        {
            //初始化樣板
            ListBoxItem lis = new ListBoxItem();
            Grid gr = new Grid();//框架
            Image bu = new Image();//圖片
            BitmapImage bit = new BitmapImage();
            bit.SetSource(fileStream);
            bit.CreateOptions = BitmapCreateOptions.BackgroundCreation;
            bit.CreateOptions = BitmapCreateOptions.DelayCreation;
            Grid MaskSelect = new Grid();//遮罩

            //建立圖片參數
            bu.Source = bit;
            bu.Width = 110;
            bu.Height = 110;
            bu.Stretch = Stretch.UniformToFill;
            bu.Margin = new Thickness(5, 5, 5, 5);

            //建立選取圖片參數
            Image check = new Image();
            var bmp = new BitmapImage();
            bmp.UriSource = new Uri("Image/selectedGridview.png", UriKind.Relative);
            check.Source = bmp;
            check.HorizontalAlignment = HorizontalAlignment.Right;
            check.VerticalAlignment = VerticalAlignment.Bottom;
            check.Height = 24;
            check.Width = 24;
            check.Stretch = Stretch.UniformToFill;
            check.Margin = new Thickness(0, 0, 5, 5);

            //建立遮罩參數
            MaskSelect.Width = 110;
            MaskSelect.Height = 110;
            MaskSelect.Margin = new Thickness(5, 5, 5, 5);
            MaskSelect.Background = new SolidColorBrush(Colors.Black);
            MaskSelect.Children.Add(check);
            MaskSelect.Tap += MaskSelect_Tap;

            //判斷圖片是否已經選取
            if (!App.MyPho.ElementAt(ImagTg).Selected)
            {
                MaskSelect.Opacity = 0;
            }
            else
            {
                MaskSelect.Opacity = 0.5;
            }

            //建立遮罩編號
            MaskSelect.Tag = ImagTg;
            ImagTg++;

            //設定圖片放入位置
            gr.Children.Add(bu);
            gr.Children.Add(MaskSelect);
            lis.Content = gr;
            lis.SetValue(Grid.ColumnProperty, col);
            lis.SetValue(Grid.RowProperty, row);
            im.Children.Add(lis);

            //圖片轉向90度
            //Point Pot = new Point();
            //Pot.X = 0.5;
            //Pot.Y = 0.5;
            //bu.RenderTransformOrigin = Pot;
            //bu.RenderTransform = new CompositeTransform() { Rotation = 90, TranslateX = 0.5, TranslateY = 0.5 };

            //加入點選物件TiltEffect
            lis.SetValue(TiltEffect.IsTiltEnabledProperty, true);

            col++;
        }

        void MaskSelect_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid gr = (Grid)sender;

            if (!App.MyPho.ElementAt(Convert.ToInt32(gr.Tag)).Selected)
            {
                App.MyPho.ElementAt(Convert.ToInt32(gr.Tag)).Selected = true;
                gr.Opacity = 0.5;
                
            }
            else
            {
                App.MyPho.ElementAt(Convert.ToInt32(gr.Tag)).Selected = false;
                gr.Opacity = 0;
            }
        }

        private void myBitmap_DownloadProgress(object sender, DownloadProgressEventArgs e)
        {
            //ProgressBar.Visibility = Visibility.Visible;
        }

        private void image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            
        }
    }

    public class MySelectPhotoList
    {
        public bool Selected { get; set; }
    }
}