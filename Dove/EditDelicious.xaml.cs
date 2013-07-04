using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Phone.Controls;
using Microsoft.Devices;
using Microsoft.Xna.Framework.Media;
using System.IO.IsolatedStorage;
using System.IO;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Shell;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Microsoft.Phone;
using System.Windows.Threading;
using System.Threading;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.SlideView;
using System.Windows.Resources;
using ExifLib;


namespace Dove
{
    public partial class EditDelicious : PhoneApplicationPage
    {
        // Variables
        PhotoCamera cam;
        public ObservableCollection<PhotoPath> CollectImagePath { get; set; }

        // Holds the current flash mode.
        private string currentFlashMode;

        // Holds the current resolution index.
        int currentResIndex = 0;

        // Constructor
        public EditDelicious()
        {
            InitializeComponent();
            SystemTray.SetIsVisible(this, false);

            //初始化相機
            InitialCamara();

            //紀錄相片數量
            MediaLibrary mediaLibrary = new MediaLibrary();
            App.savedCounter = mediaLibrary.Pictures.Count();
            StartAdd = App.savedCounter - 1;
            mediaLibrary.Dispose();

            if (App.MyPho.Count() <= 0)
            {
                for (int a = 0; a < App.savedCounter; a++)
                {
                    //初始化相片是否選擇
                    App.MyPho.Add(new MySelectPhotoList
                    {
                        Selected = false
                    });
                }
            }

            CollectImagePath = new ObservableCollection<PhotoPath>();
            PhotoScroller.ItemsSource = CollectImagePath;

            var DownUp = DownUpSelectPhoto(Down);
            DownUp.Begin();
        }

        int StartAdd=0;
        int EntAdd=0;
        int ReleasCount = 0;
        int ReloadCount = 0;
        private void GetScrollPhoto()
        {
            currentindex = 0;

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += (s, g) =>
            {
                Dispatcher.BeginInvoke(() =>
                    {
                        //配置相片空間
                        for (int a = App.savedCounter - 1; a >= 0; a--)
                        {
                            BitmapImage Scrollbit = new BitmapImage();
                            Scrollbit.CreateOptions = BitmapCreateOptions.BackgroundCreation;
                            Scrollbit.CreateOptions = BitmapCreateOptions.DelayCreation;
                            Scrollbit.UriSource = null;
                            CollectImagePath.Add(new PhotoPath()
                            {
                                ImagePath = Scrollbit
                            });
                        }
                        Thread.Sleep(1);
                    });
            };
            worker.RunWorkerAsync();
           
        }

        public byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);

            // 设置当前流的位置为流的开始 
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        private void initialAppBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.IsMenuEnabled = true;
            ApplicationBar.IsVisible = true;
            ApplicationBar.Opacity = 0.7;
            ApplicationBar.Mode = ApplicationBarMode.Default;

            if (PhotoView.SelectedIndex == 0)
            {
                ApplicationBarIconButton MyPhoto = new ApplicationBarIconButton();
                MyPhoto.IconUri = new Uri("/Image/gridview.png", UriKind.Relative);
                MyPhoto.Text = "相簿";
                MyPhoto.Click += new EventHandler(ToPhoto_Click);
                ApplicationBar.Buttons.Add(MyPhoto);

                ApplicationBarIconButton TakePhoto = new ApplicationBarIconButton();
                TakePhoto.IconUri = new Uri("/Image/feature.camera.png", UriKind.Relative);
                TakePhoto.Text = "拍照";
                TakePhoto.Click += new EventHandler(ShutterButton_Click);
                ApplicationBar.Buttons.Add(TakePhoto);

                ApplicationBarIconButton Edit = new ApplicationBarIconButton();
                Edit.IconUri = new Uri("/Image/compose.png", UriKind.Relative);
                Edit.Text = "編輯";
                Edit.Click += Edit_Click;
                ApplicationBar.Buttons.Add(Edit);
            }
            else if (PhotoView.SelectedIndex == 1)
            {
                ApplicationBarIconButton MyPhoto = new ApplicationBarIconButton();
                MyPhoto.IconUri = new Uri("/Image/gridview.png", UriKind.Relative);
                MyPhoto.Text = "相簿";
                MyPhoto.Click += new EventHandler(ToPhoto_Click);
                //search.Click += new EventHandler(PinToStart_Click);
                ApplicationBar.Buttons.Add(MyPhoto);

                ApplicationBarIconButton Edit = new ApplicationBarIconButton();
                Edit.IconUri = new Uri("/Image/compose.png", UriKind.Relative);
                Edit.Text = "編輯";
                Edit.Click += Edit_Click;
                ApplicationBar.Buttons.Add(Edit);

                ApplicationBarIconButton flip = new ApplicationBarIconButton();
                flip.IconUri = new Uri("/Image/compose.png", UriKind.Relative);
                flip.Text = "flip";
                flip.Click += flip_Click;
                ApplicationBar.Buttons.Add(flip);
            }
        }

        void flip_Click(object sender, EventArgs e)
        {
            if (PhotoScroller.TransitionMode == SlideViewTransitionMode.Slide)
            {
                PhotoScroller.TransitionMode = SlideViewTransitionMode.Flip;
            }
            else
            {
                PhotoScroller.TransitionMode = SlideViewTransitionMode.Slide;
            }
        }

        private void Edit_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Edit.xaml", UriKind.Relative));
        }

        PhotoChooserTask selectphoto;
        private void ToPhoto_Click(object sender, EventArgs e)
        {
            //selectphoto = new PhotoChooserTask();
            //selectphoto.Completed += new EventHandler<PhotoResult>(selectphoto_Completed);
            //selectphoto.Show();
            NavigationService.Navigate(new Uri("/ReviewPhoto.xaml", UriKind.Relative));
        }

        private void ShutterButton_Click(object sender, EventArgs e)
        {
            if (cam != null)
            {
                try
                {
                    // Start image capture.
                    cam.CaptureImage();
                }
                catch (Exception ex)
                {
                }
            }
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
        }

        //Code for initialization, capture completed, image availability events; also setting the source for the viewfinder.
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            initialAppBar();
            imgOutput.Height = 0;
        }

        bool isInitialCam = false;
        private void InitialCamara()
        {
            isInitialCam = false;
            ProgressBar.Visibility = Visibility;

            //背景初始化相機
            BackgroundWorker ba = new BackgroundWorker();
            ba.DoWork += (s, g) =>
            {
                Dispatcher.BeginInvoke(() =>
                {
                    if ((PhotoCamera.IsCameraTypeSupported(CameraType.Primary) == true) || (PhotoCamera.IsCameraTypeSupported(CameraType.FrontFacing) == true))
                    {
                        //cam = new Microsoft.Devices.PhotoCamera(CameraType.Primary);
                        //cam = new Microsoft.Devices.PhotoCamera();
                        //cam.Orientation = System.Windows.Controls.Orientation.Vertical;
                        cam = new Microsoft.Devices.PhotoCamera(CameraType.Primary);

                        cam.Initialized += new EventHandler<Microsoft.Devices.CameraOperationCompletedEventArgs>(cam_Initialized);

                        cam.CaptureCompleted += new EventHandler<CameraOperationCompletedEventArgs>(cam_CaptureCompleted);

                        cam.CaptureImageAvailable += new EventHandler<Microsoft.Devices.ContentReadyEventArgs>(cam_CaptureImageAvailable);

                        //cam.CaptureThumbnailAvailable += new EventHandler<ContentReadyEventArgs>(cam_CaptureThumbnailAvailable);

                        cam.AutoFocusCompleted += new EventHandler<CameraOperationCompletedEventArgs>(cam_AutoFocusCompleted);

                        this.viewfinderCanvas.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(focus_Tapped);

                        //CameraButtons.ShutterKeyHalfPressed += OnButtonHalfPress;

                        //CameraButtons.ShutterKeyPressed += OnButtonFullPress;

                        //CameraButtons.ShutterKeyReleased += OnButtonRelease;

                        this.viewfinderBrush.SetSource(cam);
                        //viewfinderBrush.RelativeTransform = new RotateTransform { CenterX = 0.5, CenterY = 0.5, Angle = cam.Orientation };
                    }
                    else
                    {
                        this.Dispatcher.BeginInvoke(delegate()
                        {
                            txtDebug.Text = "A Camera is not available on this device.";
                        });

                        // Disable UI.
                        //ShutterButton.IsEnabled = false;
                        //FlashButton.IsEnabled = false;
                        //AFButton.IsEnabled = false;
                        //ResButton.IsEnabled = false;
                    }
                });
                Thread.Sleep(5);
            };
            ba.RunWorkerAsync();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            //判斷是否已完成初始化相機
            if (isInitialCam)
            {
                //卸載相機
                DisableCamera();
            }
            else
            {
                e.Cancel=true;
            }
        }

        private void DisableCamera()
        {
            //判斷是否已完成初始化相機
            if (isInitialCam)
            {
                if (cam != null)
                {
                    try
                    {
                        // Dispose camera to minimize power consumption and to expedite shutdown.
                        cam.Dispose();

                        // Release memory, ensure garbage collection.
                        cam.Initialized -= cam_Initialized;
                        cam.CaptureCompleted -= cam_CaptureCompleted;
                        cam.CaptureImageAvailable -= cam_CaptureImageAvailable;
                        cam.CaptureThumbnailAvailable -= cam_CaptureThumbnailAvailable;
                        cam.AutoFocusCompleted -= cam_AutoFocusCompleted;
                        CameraButtons.ShutterKeyHalfPressed -= OnButtonHalfPress;
                        CameraButtons.ShutterKeyPressed -= OnButtonFullPress;
                        CameraButtons.ShutterKeyReleased -= OnButtonRelease;
                    }
                    catch { }
                }
            }
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (cam != null)
            {
                DisableCamera();
            }
        }

        void cam_Initialized(object sender, Microsoft.Devices.CameraOperationCompletedEventArgs e)
        {
            if (e.Succeeded)
            {
                this.Dispatcher.BeginInvoke(delegate()
                {
                    ProgressBar.Visibility = Visibility.Collapsed;
                    isInitialCam = true;

                    // Write message.
                    txtDebug.Text = "Camera initialized.";
                    FistLoad = false;

                    // Set flash button text.
                    //FlashButton.Content = "Fl:" + cam.FlashMode.ToString();
                });
            }

            Dispatcher.BeginInvoke(() =>
            {
                //PageOrientation pagroi = this.Orientation;
                previewTransform.Rotation = cam.Orientation;
            });

        }

        // Ensure that the viewfinder is upright in LandscapeRight.
        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            if (cam != null)
            {
                // LandscapeRight rotation when camera is on back of device.
                int landscapeRightRotation = 180;

                // Change LandscapeRight rotation for front-facing camera.
                if (cam.CameraType == CameraType.FrontFacing) landscapeRightRotation = -180;

                // Rotate video brush from camera.
                if (e.Orientation == PageOrientation.LandscapeRight)
                {
                    // Rotate for LandscapeRight orientation.
                    viewfinderBrush.RelativeTransform =
                        new CompositeTransform() { CenterX = 0.5, CenterY = 0.5, Rotation = landscapeRightRotation };
                }
                else
                {
                    // Rotate for standard landscape orientation.
                    viewfinderBrush.RelativeTransform =
                        new CompositeTransform() { CenterX = 0.5, CenterY = 0.5, Rotation = 0 };
                }
            }

            base.OnOrientationChanged(e);
        }

        void cam_CaptureCompleted(object sender, CameraOperationCompletedEventArgs e)
        {
            // Increments the savedCounter variable used for generating JPEG file names.
            App.savedCounter++;
            IsolatedStorageSettings.ApplicationSettings["savedCounter"] = App.savedCounter;
        }

        string PhotofileName = App.savedCounter + "_th.jpg";
        BitmapImage bitmap = new BitmapImage();
        void cam_CaptureImageAvailable(object sender, Microsoft.Devices.ContentReadyEventArgs e)
        {
            try
            {
                MediaLibrary mediaLibrary = new MediaLibrary();
                Dispatcher.BeginInvoke(() =>
                {
                    PhotofileName = App.savedCounter + "_th.jpg";

                    //儲存相片
                    using (var memoryStream = new MemoryStream())
                    {
                        //轉正相片串流
                        Stream pho = OrientationPhoto(e.ImageStream, PhotofileName);
                        bitmap.SetSource(pho);

                        //設定相片參數
                        var quality = 80;
                        var p = quality / 100.0;
                        var writeableBitmap = new WriteableBitmap(bitmap);
                        var width = writeableBitmap.PixelWidth * p;
                        var height = writeableBitmap.PixelHeight * p;
                        writeableBitmap.SaveJpeg(memoryStream, (int)width, (int)height, 0, quality);
                        memoryStream.SetLength(memoryStream.Position);
                        memoryStream.Seek(0, SeekOrigin.Begin);

                        //儲存相片至手機
                        mediaLibrary.SavePicture(PhotofileName, memoryStream);

                        //關閉串流
                        memoryStream.Close();
                        memoryStream.Dispose();
                        pho.Close();
                        pho.Dispose();
                    }

                    //关闭流
                    e.ImageStream.Close();
                    e.ImageStream.Dispose();
                    bitmap.UriSource = null;
                });

                // Write message to UI thread.
                Deployment.Current.Dispatcher.BeginInvoke(delegate()
                {
                    txtDebug.Text = "Thumbnail has been saved to isolated storage.";

                    Stream photo = mediaLibrary.Pictures.ElementAt(mediaLibrary.Pictures.Count() - 1).GetImage();
                    byte[] data = StreamToBytes(photo);
                    using (MemoryStream stream = new MemoryStream(data))
                    {
                        //加入相片
                        bitmap.SetSource(stream);
                        this.imgOutput.Source = bitmap;

                        //新增相片選擇狀態
                        App.MyPho.Add(new MySelectPhotoList
                        {
                            Selected = false
                        });

                        //相片向右滑動
                        imgOutput.Height = 800;
                        popupArea.Opacity = 100;
                        var story = PrepareCloseStory();
                        story.Begin();
                        story.Completed += story_Completed;

                        //關閉串流
                        stream.Close();
                        stream.Dispose();
                        data = null;
                    }

                    //關閉串流
                    photo.Close();
                    photo.Dispose();
                    mediaLibrary.Dispose();
                });

             
            }
            finally
            {
                // Close image stream
                //e.ImageStream.Close();
            }
        }

        Stream capturedImage;
        int _width;
        int _height;
        ExifLib.ExifOrientation _orientation;
        int _angle;
        private Stream OrientationPhoto(Stream ChosenPhoto, string OriginalFileName)
        {
            // figure out the orientation from EXIF data
            ChosenPhoto.Position = 0;
            JpegInfo info = ExifReader.ReadJpeg(ChosenPhoto, OriginalFileName);

            _width = info.Width;
            _height = info.Height;
            _orientation = info.Orientation;

            switch (info.Orientation)
            {
                case ExifOrientation.TopLeft:
                case ExifOrientation.Undefined:
                    _angle = 0;
                    break;
                case ExifOrientation.TopRight:
                    _angle = 90;
                    break;
                case ExifOrientation.BottomRight:
                    _angle = 180;
                    break;
                case ExifOrientation.BottomLeft:
                    _angle = 270;
                    break;
            }

            if (_angle > 0d)
            {
                capturedImage = RotateStream(ChosenPhoto, _angle);
            }
            else
            {
                capturedImage = ChosenPhoto;
            }

            return capturedImage;
        }

        private Stream RotateStream(Stream stream, int angle)
        {
            stream.Position = 0;
            if (angle % 90 != 0 || angle < 0) throw new ArgumentException();
            if (angle % 360 == 0) return stream;

            BitmapImage bitmap = new BitmapImage();
            bitmap.SetSource(stream);
            WriteableBitmap wbSource = new WriteableBitmap(bitmap);

            WriteableBitmap wbTarget = null;
            if (angle % 180 == 0)
            {
                wbTarget = new WriteableBitmap(wbSource.PixelWidth, wbSource.PixelHeight);
            }
            else
            {
                wbTarget = new WriteableBitmap(wbSource.PixelHeight, wbSource.PixelWidth);
            }

            for (int x = 0; x < wbSource.PixelWidth; x++)
            {
                for (int y = 0; y < wbSource.PixelHeight; y++)
                {
                    switch (angle % 360)
                    {
                        case 90:
                            wbTarget.Pixels[(wbSource.PixelHeight - y - 1) + x * wbTarget.PixelWidth] = wbSource.Pixels[x + y * wbSource.PixelWidth];
                            break;
                        case 180:
                            wbTarget.Pixels[(wbSource.PixelWidth - x - 1) + (wbSource.PixelHeight - y - 1) * wbSource.PixelWidth] = wbSource.Pixels[x + y * wbSource.PixelWidth];
                            break;
                        case 270:
                            wbTarget.Pixels[y + (wbSource.PixelWidth - x - 1) * wbTarget.PixelWidth] = wbSource.Pixels[x + y * wbSource.PixelWidth];
                            break;
                    }
                }
            }
            MemoryStream targetStream = new MemoryStream();
            wbTarget.SaveJpeg(targetStream, wbTarget.PixelWidth, wbTarget.PixelHeight, 0, 100);
            return targetStream;
        }
        // Informs when thumbnail picture has been taken, saves to isolated storage
        // User will select this image in the pictures application to bring up the full-resolution picture. 
        public void cam_CaptureThumbnailAvailable(object sender, ContentReadyEventArgs e)
        {
           
        }

        void story_Completed(object sender, EventArgs e)
        {
            imgOutput.Height = 0;
            imgOutput.Source = null;
            bitmap.UriSource = null;
        }

        CubicEase _EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseInOut };
        private Storyboard PrepareCloseStory()
        {
            Storyboard story = new Storyboard();
            DoubleAnimation animation;

            animation = new DoubleAnimation();
            animation.From = popupTransform.TranslateY;
            //animation.To = 0 - imgOutput.ActualHeight;
            //animation.To = imgOutput.ActualHeight;
            animation.To = 480;
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            animation.EasingFunction = _EasingFunction;
            Storyboard.SetTarget(animation, popupTransform);
            Storyboard.SetTargetProperty(animation, new PropertyPath("TranslateX"));
            story.Children.Add(animation);
            //animation = new DoubleAnimation();
            //animation.From = mask.Opacity;
            //animation.To = 0;
            //animation.Duration = new Duration(TimeSpan.FromMilliseconds(1000));
            //Storyboard.SetTarget(animation, mask);
            //Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.Opacity)"));
            //story.Children.Add(animation);


            return story;
            //Storyboard story = new Storyboard();
            //DoubleAnimation animation;
            //animation = new DoubleAnimation();
            //animation.From = 0 - imgOutput.ActualHeight;
            //animation.To = SystemTray.IsVisible ? 32 : 0;
            //animation.Duration = new Duration(TimeSpan.FromMilliseconds(300));
            //animation.EasingFunction = _EasingFunction;
            //Storyboard.SetTarget(animation, popupTransform);
            //Storyboard.SetTargetProperty(animation, new PropertyPath("TranslateY"));
            //story.Children.Add(animation);

            //animation = new DoubleAnimation();
            //animation.From = 0;
            //animation.To = 0.5;
            //animation.Duration = new Duration(TimeSpan.FromMilliseconds(300));
            //Storyboard.SetTarget(animation, mask);
            //Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.Opacity)"));
            //story.Children.Add(animation);
        }


        void cam_AutoFocusCompleted(object sender, CameraOperationCompletedEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(delegate()
            {
                // Write message to UI.
                txtDebug.Text = "Auto focus has completed.";

                // Hide the focus brackets.
                focusBrackets.Visibility = Visibility.Collapsed;

            });
        }

        // Provide touch focus in the viewfinder.
        void focus_Tapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (cam != null)
            {
                if (cam.IsFocusAtPointSupported == true)
                {
                    try
                    {
                        // Determine location of tap.
                        Point tapLocation = e.GetPosition(viewfinderCanvas);

                        // Position focus brackets with estimated offsets.
                        focusBrackets.SetValue(Canvas.LeftProperty, tapLocation.X - 30);
                        focusBrackets.SetValue(Canvas.TopProperty, tapLocation.Y - 28);

                        // Determine focus point.
                        double focusXPercentage = tapLocation.X / viewfinderCanvas.Width;
                        double focusYPercentage = tapLocation.Y / viewfinderCanvas.Height;

                        // Show focus brackets and focus at point
                        focusBrackets.Visibility = Visibility.Visible;
                        cam.FocusAtPoint(focusXPercentage, focusYPercentage);

                        // Write a message to the UI.
                        this.Dispatcher.BeginInvoke(delegate()
                        {
                            txtDebug.Text = String.Format("Camera focusing at point: {0:N2} , {1:N2}", focusXPercentage, focusYPercentage);
                        });
                    }
                    catch (Exception focusError)
                    {
                        // Cannot focus when a capture is in progress.
                        this.Dispatcher.BeginInvoke(delegate()
                        {
                            // Write a message to the UI.
                            txtDebug.Text = focusError.Message;
                            // Hide focus brackets.
                            focusBrackets.Visibility = Visibility.Collapsed;
                        });
                    }
                }
                else
                {
                    // Write a message to the UI.
                    this.Dispatcher.BeginInvoke(delegate()
                    {
                        txtDebug.Text = "Camera does not support FocusAtPoint().";
                    });
                }

                //try
                //{
                //    // Start image capture.
                //    cam.CaptureImage();
                //}
                //catch (Exception ex)
                //{
                //    this.Dispatcher.BeginInvoke(delegate()
                //    {
                //        // Cannot capture an image until the previous capture has completed.
                //        txtDebug.Text = ex.Message;
                //    });
                //}
            }
        }

        // Provide auto-focus with a half button press using the hardware shutter button.
        private void OnButtonHalfPress(object sender, EventArgs e)
        {
            if (cam != null)
            {
                // Focus when a capture is not in progress.
                try
                {
                    this.Dispatcher.BeginInvoke(delegate()
                    {
                        txtDebug.Text = "Half Button Press: Auto Focus";
                    });

                    cam.Focus();
                }
                catch (Exception focusError)
                {
                    // Cannot focus when a capture is in progress.
                    this.Dispatcher.BeginInvoke(delegate()
                    {
                        txtDebug.Text = focusError.Message;
                    });
                }
            }
        }

        // Capture the image with a full button press using the hardware shutter button.
        private void OnButtonFullPress(object sender, EventArgs e)
        {
            if (cam != null)
            {
                cam.CaptureImage();
            }
        }

        // Cancel the focus if the half button press is released using the hardware shutter button.
        private void OnButtonRelease(object sender, EventArgs e)
        {

            if (cam != null)
            {
                cam.CancelFocus();
            }
        }

        private void changeFlash_Clicked(object sender, RoutedEventArgs e)
        {
            switch (cam.FlashMode)
            {
                case FlashMode.Off:
                    if (cam.IsFlashModeSupported(FlashMode.On))
                    {
                        // Specify that flash should be used.
                        cam.FlashMode = FlashMode.On;
                        //FlashButton.Content = "閃光:On";
                        currentFlashMode = "Flash mode: On";
                    }
                    break;
                case FlashMode.On:
                    if (cam.IsFlashModeSupported(FlashMode.RedEyeReduction))
                    {
                        // Specify that the red-eye reduction flash should be used.
                        cam.FlashMode = FlashMode.RedEyeReduction;
                        //FlashButton.Content = "Fl:RER";
                        currentFlashMode = "Flash mode: RedEyeReduction";
                    }
                    else if (cam.IsFlashModeSupported(FlashMode.Auto))
                    {
                        // If red-eye reduction is not supported, specify automatic mode.
                        cam.FlashMode = FlashMode.Auto;
                        //FlashButton.Content = "閃光:Auto";
                        currentFlashMode = "Flash mode: Auto";
                    }
                    else
                    {
                        // If automatic is not supported, specify that no flash should be used.
                        cam.FlashMode = FlashMode.Off;
                        //FlashButton.Content = "閃光:Off";
                        currentFlashMode = "Flash mode: Off";
                    }
                    break;
                case FlashMode.RedEyeReduction:
                    if (cam.IsFlashModeSupported(FlashMode.Auto))
                    {
                        // Specify that the flash should be used in the automatic mode.
                        cam.FlashMode = FlashMode.Auto;
                        //FlashButton.Content = "閃光:Auto";
                        currentFlashMode = "Flash mode: Auto";
                    }
                    else
                    {
                        // If automatic is not supported, specify that no flash should be used.
                        cam.FlashMode = FlashMode.Off;
                        //FlashButton.Content = "閃光:Off";
                        currentFlashMode = "Flash mode: Off";
                    }
                    break;
                case FlashMode.Auto:
                    if (cam.IsFlashModeSupported(FlashMode.Off))
                    {
                        // Specify that no flash should be used.
                        cam.FlashMode = FlashMode.Off;
                        //FlashButton.Content = "閃光:Off";
                        currentFlashMode = "Flash mode: Off";
                    }
                    break;
            }

            // Display current flash mode.
            this.Dispatcher.BeginInvoke(delegate()
            {
                txtDebug.Text = currentFlashMode;
            });
        }

        private void focus_Clicked(object sender, RoutedEventArgs e)
        {
            if (cam.IsFocusSupported == true)
            {
                //Focus when a capture is not in progress.
                try
                {
                    cam.Focus();
                }
                catch (Exception focusError)
                {
                    // Cannot focus when a capture is in progress.
                    this.Dispatcher.BeginInvoke(delegate()
                    {
                        txtDebug.Text = focusError.Message;
                    });
                }
            }
            else
            {
                // Write message to UI.
                this.Dispatcher.BeginInvoke(delegate()
                {
                    txtDebug.Text = "Camera does not support programmable auto focus.";
                });
            }
        }

        private void imgOutput_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PhotoChooserTask pct = new PhotoChooserTask();
            pct.Completed += pct_Completed;

            //如果希望使用者可以直接使用相機把ShowCamera 設為true
            //pct.ShowCamera = true;
            pct.Show();
        }

        void pct_Completed(object sender, PhotoResult e)
        {
            //BitmapImage bmp = new BitmapImage();
            //bmp.SetSource(e.ChosenPhoto);
            //this.imgResult.Source = bmp;
        }

        private void myBitmap_DownloadProgress(object sender, DownloadProgressEventArgs e)
        {
            //ProgressBar.Visibility = Visibility.Visible;
        }

        private void image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        bool FistLoad=true;
        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Pivot pi = (Pivot)sender;
            if (pi.SelectedIndex == 0)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();

                //非第一次進入執行初始化相機
                if (!FistLoad)
                {
                    InitialCamara();
                }
                PhotoView.IsLocked = false;
                viewfinderCanvas.Visibility = Visibility;
                PhotoScroller.Visibility = Visibility.Collapsed;

                //PhotoView.IsHitTestVisible = true;
                    
                //初始化App;ication Bar
                initialAppBar();

                //釋放記憶體空間
                if (CollectImagePath.Count() > 0)
                {
                    for (int i = CollectImagePath.Count() - 1; i >= 0; i--)
                    {
                        CollectImagePath.RemoveAt(i);
                    }

                    StartAdd = App.savedCounter - 1;
                    currentindex = 0;
                }
            }
            else if (pi.SelectedIndex == 1)
            {
                //卸載相機
                DisableCamera();
                viewfinderCanvas.Visibility = Visibility.Collapsed;
                PhotoScroller.Visibility = Visibility;

                //載入相片
                StartAdd = App.savedCounter - 1;
                EntAdd = App.savedCounter - 10;
                GetScrollPhoto();

                //初始化App;ication Bar
                initialAppBar();
            }
            else if (pi.SelectedIndex == 2)
            {
                pi.SelectedIndex = 0;
            }
        }

        //PhotoChooserTask selectphoto = null;
        void selectphoto_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                BinaryReader reader = new BinaryReader(e.ChosenPhoto);
                Image image1 = new Image();
                image1.Source = new BitmapImage(new Uri(e.OriginalFileName));
            }
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            PhotoView.IsLocked = true;
            //PhotoView.IsHitTestVisible = false;

            BackgroundWorker bk = new BackgroundWorker();
            bk.DoWork += (s, g) =>
            {
                Thread.Sleep(50);
            };
            bk.RunWorkerAsync();
            bk.RunWorkerCompleted += (a, b) =>
            {
                LeaveTouch = false;
            };
        }

        bool LeaveTouch = false;
        bool StartSelectPhoto = false;
        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            PhotoView.IsLocked = false;
            //PhotoView.IsHitTestVisible = true;
            
            //ScrollMyPhoto();
            BackgroundWorker bk = new BackgroundWorker();
            bk.DoWork += (s, g) =>
                 {
                     Thread.Sleep(50);
                 };
            bk.RunWorkerAsync();
            bk.RunWorkerCompleted += (a, b) =>
                {
                    LeaveTouch = true;
                };
        }

        private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            base.OnOrientationChanged(e);
            switch (e.Orientation)
            {
                case PageOrientation.Landscape:
                case PageOrientation.LandscapeLeft:
                    previewTransform.Rotation = 0;
                    break;
                case PageOrientation.LandscapeRight:
                    previewTransform.Rotation = 180;
                    break;
                case PageOrientation.Portrait:
                case PageOrientation.PortraitUp:
                    previewTransform.Rotation = 90;
                    break;
                case PageOrientation.PortraitDown:
                    previewTransform.Rotation = 270;
                    break;
            }

            if (cam != null)
            {
                // LandscapeRight rotation when camera is on back of phone.
                int landscapeRightRotation = 180;

                // Change LandscapeRight rotation for front-facing camera.
                if (cam.CameraType == CameraType.FrontFacing) landscapeRightRotation = -180;

                // Rotate video brush from camera.
                if (e.Orientation == PageOrientation.LandscapeRight)
                {
                    // Rotate for LandscapeRight orientation.
                    viewfinderBrush.RelativeTransform =
                        new CompositeTransform() { CenterX = 0.5, CenterY = 0.5, Rotation = landscapeRightRotation };
                }
                else
                {
                    // Rotate for standard landscape orientation.
                    viewfinderBrush.RelativeTransform =
                        new CompositeTransform() { CenterX = 0.5, CenterY = 0.5, Rotation = 0 };
                }
            }

            base.OnOrientationChanged(e);
        }

        bool Down = false;
        private void ChoosePhoto_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Image im = (Image)sender;

            //判斷該相片是否選取
            BitmapImage bi = new BitmapImage();
            var se = PhotoScroller.SelectedItem as PhotoPath;
            int sel = 0;
            for (int i = 0; i < CollectImagePath.Count(); i++)
            {
                if (se.ImagePath == CollectImagePath.ElementAt(i).ImagePath)
                {
                    break;
                }
                else
                {
                    sel++;
                }
            }

            if (!App.MyPho.ElementAt(sel).Selected)
            {
                bi.UriSource = new Uri("Image/selected.png", UriKind.Relative);
                im.Source = bi;
                App.MyPho.ElementAt(sel).Selected = true;
                Down = true;
            }
            else
            {
                bi.UriSource = new Uri("Image/Unselected.png", UriKind.Relative);
                im.Source = bi;
                App.MyPho.ElementAt(sel).Selected = false;
                Down = false;
            }


            FoodMask.Source = CollectImagePath.ElementAt(sel).ImagePath;
            PhotoArea.Height = Double.NaN;

            BackgroundWorker bk = new BackgroundWorker();
            bk.DoWork += (a, b) =>
                {
                    Thread.Sleep(10);
                };
            bk.RunWorkerAsync();
            bk.RunWorkerCompleted += (sa, sg) =>
                {
                    var DownUp = DownUpSelectPhoto(Down);
                    DownUp.Begin();
                    DownUp.Completed += (s, g) =>
                        {
                            PhotoArea.Height = 0;
                            FoodMask.Source = null;
                        };
                };
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

        //private Storyboard DownUpSelectPhoto(UIElement obj)
        private Storyboard DownUpSelectPhoto(bool Down)
        {
            if (Down)
            {
                Storyboard story = new Storyboard();
                DoubleAnimation animation;
                animation = new DoubleAnimation();
                animation.From = 0;
                animation.To = 800;
                animation.Duration = new Duration(TimeSpan.FromMilliseconds(400));
                animation.EasingFunction = _EasingFunction;
                Storyboard.SetTarget(animation, PhotoTransform);
                Storyboard.SetTargetProperty(animation, new PropertyPath("TranslateY"));
                story.Children.Add(animation);
                return story;
            }
            else
            {
                Storyboard story = new Storyboard();
                DoubleAnimation animation;
                animation = new DoubleAnimation();
                animation.From = 800;
                animation.To = 0;
                animation.Duration = new Duration(TimeSpan.FromMilliseconds(400));
                animation.EasingFunction = _EasingFunction;
                Storyboard.SetTarget(animation, PhotoTransform);
                Storyboard.SetTargetProperty(animation, new PropertyPath("TranslateY"));
                story.Children.Add(animation);
                return story;
            }
        }

        private void FoodPhoto_Unloaded(object sender, RoutedEventArgs e)
        {
            Image im = (Image)sender;
            im.Source = null;
        }

        private void FoodPhoto_Loaded(object sender, RoutedEventArgs e)
        {
            Image im = (Image)sender;
            im.Visibility = Visibility;
            ProgressBar.Visibility = Visibility.Collapsed;
        }

        private void FoodPhoto_ImageOpened(object sender, RoutedEventArgs e)
        {
            //ProgressBar.Visibility = Visibility.Collapsed;
        }

        private double TotalImageScale = 1d;
        private Point ImagePosition = new Point(0, 0);
        private const double MAX_IMAGE_ZOOM = 2;
        private Point _oldFinger1;
        private Point _oldFinger2;
        private double _oldScaleFactor;
        private void OnPinchStarted(object sender, PinchStartedGestureEventArgs e)
        {
            var ImgZoom = sender as Image;
            //var transform = ImgZoom.RenderTransform as CompositeTransform; 
            _oldFinger1 = e.GetPosition(ImgZoom, 0);
            _oldFinger2 = e.GetPosition(ImgZoom, 1);
            _oldScaleFactor = 1;

            PhotoScroller.Height = 800;
            ChoosePhoto.Visibility = Visibility.Collapsed;
        }

        private bool IsScaleValid(double scaleDelta)
        {
            return (TotalImageScale * scaleDelta >= 1) && (TotalImageScale * scaleDelta <= MAX_IMAGE_ZOOM);
        }
        private Point GetTranslationDelta(Point currentFinger1, Point currentFinger2,Point oldFinger1, Point oldFinger2,Point currentPosition, double scaleFactor)
        {
            var newPos1 = new Point(
             currentFinger1.X + (currentPosition.X - oldFinger1.X) * scaleFactor,
             currentFinger1.Y + (currentPosition.Y - oldFinger1.Y) * scaleFactor);

            var newPos2 = new Point(
             currentFinger2.X + (currentPosition.X - oldFinger2.X) * scaleFactor,
             currentFinger2.Y + (currentPosition.Y - oldFinger2.Y) * scaleFactor);

            var newPos = new Point(
                (newPos1.X + newPos2.X) / 2,
                (newPos1.Y + newPos2.Y) / 2);

            return new Point(
                newPos.X - currentPosition.X,
                newPos.Y - currentPosition.Y);
        }
        private void UpdateImageScale(double scaleFactor, Image ImgZoom)
        {
            //var ImgZoom = sender as Image;
            //var transform = ImgZoom.RenderTransform as CompositeTransform; 
            TotalImageScale *= scaleFactor;
            ApplyScale(ImgZoom);
        }
        private void ApplyScale(Image ImgZoom)
        {
            //var ImgZoom = sender as Image;
            //var transform = ImgZoom.RenderTransform as CompositeTransform; 
            ((CompositeTransform)ImgZoom.RenderTransform).ScaleX = TotalImageScale;
            ((CompositeTransform)ImgZoom.RenderTransform).ScaleY = TotalImageScale;
        }
        private void UpdateImagePosition(Point delta, Image ImgZoom)
        {
            var newPosition = new Point(ImagePosition.X + delta.X, ImagePosition.Y + delta.Y);

            //var ImgZoom = sender as Image;
            //var transform = ImgZoom.RenderTransform as CompositeTransform; 

            if (newPosition.X > 0) newPosition.X = 0;
            if (newPosition.Y > 0) newPosition.Y = 0;

            if ((ImgZoom.ActualWidth * TotalImageScale) + newPosition.X < ImgZoom.ActualWidth)
                newPosition.X = ImgZoom.ActualWidth - (ImgZoom.ActualWidth * TotalImageScale);

            if ((ImgZoom.ActualHeight * TotalImageScale) + newPosition.Y < ImgZoom.ActualHeight)
                newPosition.Y = ImgZoom.ActualHeight - (ImgZoom.ActualHeight * TotalImageScale);

            ImagePosition = newPosition;

            ApplyPosition(ImgZoom);
        }
        private void ApplyPosition(Image ImgZoom)
        {
            //var ImgZoom = sender as Image;
            //var transform = ImgZoom.RenderTransform as CompositeTransform; 
            ((CompositeTransform)ImgZoom.RenderTransform).TranslateX = ImagePosition.X;
            ((CompositeTransform)ImgZoom.RenderTransform).TranslateY = ImagePosition.Y;
        }
        private void OnPinchDelta(object sender, PinchGestureEventArgs e)
        {
            var ImgZoom = sender as Image;

            var scaleFactor = e.DistanceRatio / _oldScaleFactor;
            if (!IsScaleValid(scaleFactor))
                return;

            var currentFinger1 = e.GetPosition(ImgZoom, 0);
            var currentFinger2 = e.GetPosition(ImgZoom, 1);

            var translationDelta = GetTranslationDelta(
                currentFinger1,
                currentFinger2,
                _oldFinger1,
                _oldFinger2,
                ImagePosition,
                scaleFactor);

            _oldFinger1 = currentFinger1;
            _oldFinger2 = currentFinger2;
            _oldScaleFactor = e.DistanceRatio;

            UpdateImageScale(scaleFactor, ImgZoom);
            UpdateImagePosition(translationDelta, ImgZoom);
        }

        private void OnDragDelta(object sender, DragDeltaGestureEventArgs e)
        {
            var ImgZoom = sender as Image;
            if (TotalImageScale == 1 && ImagePosition == new Point(0, 0))
            {
                //PhotoScroller.IsLocked = false;
                PhotoScroller.IsHitTestVisible = true;
            }
            else
            {
                
                var translationDelta = new Point(e.HorizontalChange, e.VerticalChange);
                if (IsDragValid(1, translationDelta, ImgZoom))
                    UpdateImagePosition(translationDelta, ImgZoom);
            }
        }
        private bool IsDragValid(double scaleDelta, Point translateDelta, Image ImgZoom)
        {
            //var ImgZoom = sender as Image;
            //var transform = ImgZoom.RenderTransform as CompositeTransform; 

            if (ImagePosition.X + translateDelta.X > 0 || ImagePosition.Y + translateDelta.Y > 0)
            {
                return false;
            }

            else if ((ImgZoom.ActualWidth * TotalImageScale * scaleDelta) + (ImagePosition.X + translateDelta.X) < ImgZoom.ActualWidth)
            {
                return false;
            }

            else if ((ImgZoom.ActualHeight * TotalImageScale * scaleDelta) + (ImagePosition.Y + translateDelta.Y) < ImgZoom.ActualHeight)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void OnDoubleTap(object sender, Microsoft.Phone.Controls.GestureEventArgs e)
        {
            var ImgZoom = sender as Image;
            //var transform = ImgZoom.RenderTransform as CompositeTransform;
            ResetImagePosition(ImgZoom);

            //PhotoScroller.IsLocked = false;
            PhotoScroller.IsHitTestVisible = true;
            ChoosePhoto.Visibility = Visibility;
            PhotoScroller.Height = 503;
        }
        private void ResetImagePosition(Image ImgZoom)
        {
            //var ImgZoom = sender as Image;
            //var transform = ImgZoom.RenderTransform as CompositeTransform; 
            TotalImageScale = 1;
            ImagePosition = new Point(0, 0);
            ApplyScale(ImgZoom);
            ApplyPosition(ImgZoom);
        }

        private void OnFlick(object sender, FlickGestureEventArgs e)
        {
            //if (PhotoScroller.SelectedIndex == PhotoScroller.Items.Count() - 1)
            //{
            //    if (e.HorizontalVelocity < 0)
            //    {
            //        //Right flick
            //        PhotoScroller.IsHitTestVisible = false;
            //    }
            //    else
            //    {
            //        //Left flick
            //        PhotoScroller.IsHitTestVisible = true;
            //    }
            //}

            //WPLock
            //if (TotalImageScale == 1 && ImagePosition == new Point(0, 0))
            //{
            //    PhotoScroller.IsHitTestVisible = false;
            //}
            //else
            //{
            //    PhotoScroller.IsHitTestVisible = true;
            //}
        }

        private void LayoutRoot_MouseEnter(object sender, MouseEventArgs e)
        {
            BackgroundWorker bk = new BackgroundWorker();
            bk.DoWork += ((s, g) =>
            {
                Thread.Sleep(5);
            });
            bk.RunWorkerAsync();
            bk.RunWorkerCompleted += ((a, b) =>
            {
                //WPLock
                if (TotalImageScale == 1 && ImagePosition == new Point(0, 0))
                {
                    PhotoScroller.IsHitTestVisible = true;
                }
                else
                {
                    PhotoScroller.IsHitTestVisible = false;
                }
            });
        }

        private void LayoutRoot_MouseLeave(object sender, MouseEventArgs e)
        {
            PhotoScroller.IsHitTestVisible = true;
        }

        private void PhotoScroller_UnloadedPivotItem(object sender, PivotItemEventArgs e)
        {
            //try
            //{
            //    Dispatcher.BeginInvoke(() =>
            //    {
            //        for (int i = 0; i < PhotoScroller.Items.Count(); i++)
            //        {
            //            var se = PhotoScroller.ItemContainerGenerator.ContainerFromIndex(i) as PivotItem;
            //            Image te = new Image();

            //            if (i != PhotoScroller.SelectedIndex)
            //            {
            //                try
            //                {
            //                    te = FindDescendant<Image>(se).FindName("FoodPhoto") as Image;
            //                }
            //                catch
            //                {
            //                    Debug.WriteLine("SomethingWrong");
            //                }
            //                te.Visibility = Visibility.Collapsed;
            //            }
            //            else
            //            {
            //                te = FindDescendant<Image>(se).FindName("FoodPhoto") as Image;
            //                te.Visibility = Visibility;
            //            }
            //        }
            //    });
            //}
            //catch { }
        }

        BitmapImage bi = new BitmapImage();
        private void ChangeSelected()
        {
            //判斷該相片是否選取
            var se = PhotoScroller.SelectedItem as PhotoPath ;
            int sel=0;
            for (int i = 0; i < CollectImagePath.Count(); i++)
            {
                if (se.ImagePath == CollectImagePath.ElementAt(i).ImagePath)
                {
                    break;
                }
                else
                {
                    sel++;
                }
            }

            if (!App.MyPho.ElementAt(sel).Selected)
            {
                bi.UriSource = new Uri("Image/Unselected.png", UriKind.Relative);
                ChoosePhoto.Source = bi;
                //im.Tag = "0";
                Down = false;
            }
            else
            {
                bi.UriSource = new Uri("Image/selected.png", UriKind.Relative);
                ChoosePhoto.Source = bi;
                //im.Tag = "1";
                Down = true;
            }

        }

        object Next;
        object Previous;
        int currentindex = 0;
        private void PhotoScroller_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BackgroundWorker ba = new BackgroundWorker();
            ba.DoWork += (a, b) =>
                {
                    Thread.Sleep(1);
                };
            ba.RunWorkerAsync();
            ba.RunWorkerCompleted += (s, g) =>
                {
                    //紀錄目前存放相片位置
                    if (PhotoScroller.SelectedItem == Next)
                    {
                        StartAdd--;
                        currentindex++;
                    }
                    else if (PhotoScroller.SelectedItem == Previous)
                    {
                        StartAdd++;
                        currentindex--;
                    }

                    //放入相片目前/前一張/下一張
                    try
                    {
                        AddCurrentPhoto();
                    }
                    catch { }
                    if (PhotoScroller.NextItem != null)
                    {
                        AddNextPhoto();
                    }
                    if (PhotoScroller.PreviousItem != null)
                    {
                        AddPreviousPhoto();
                    }

                    //紀錄上下張相片
                    Next = PhotoScroller.NextItem;
                    Previous = PhotoScroller.PreviousItem;
                };

            //判斷該相片是否選取
            try
            {
                ChangeSelected();
            }
            catch { }
        }
        int ReAddIndex = 0;

        private void AddCurrentPhoto()
        {
            MediaLibrary mediaLibrary = new MediaLibrary();
            using (Stream photo = mediaLibrary.Pictures.ElementAt(StartAdd).GetImage())
            {
                mediaLibrary.Pictures.ElementAt(StartAdd).GetImage().Close();
                mediaLibrary.Pictures.ElementAt(StartAdd).GetImage().Dispose();
                mediaLibrary.Dispose();

                BitmapImage Scrollbit = new BitmapImage();
                Scrollbit.CreateOptions = BitmapCreateOptions.BackgroundCreation;
                Scrollbit.CreateOptions = BitmapCreateOptions.DelayCreation;
                Scrollbit.SetSource(photo);

                //放入目前相片
                CollectImagePath.ElementAt(currentindex).ImagePath = Scrollbit;

                //移除前兩張相片
                if (currentindex - 2 >= 0)
                {
                    CollectImagePath.ElementAt(currentindex - 2).ImagePath = new BitmapImage() { UriSource = null };
                }

                //移除下兩張相片
                if (currentindex + 2 <= CollectImagePath.Count() - 1)
                {
                    CollectImagePath.ElementAt(currentindex + 2).ImagePath = new BitmapImage() { UriSource = null };
                }

                //關閉串流
                photo.Close();
                photo.Dispose();
            }
        }

        private void AddNextPhoto()
        {
            MediaLibrary mediaLibrary = new MediaLibrary();
            using (Stream photo = mediaLibrary.Pictures.ElementAt(StartAdd-1).GetImage())
            {
                mediaLibrary.Pictures.ElementAt(StartAdd-1).GetImage().Close();
                mediaLibrary.Pictures.ElementAt(StartAdd-1).GetImage().Dispose();
                mediaLibrary.Dispose();

                BitmapImage Scrollbit = new BitmapImage();
                Scrollbit.CreateOptions = BitmapCreateOptions.BackgroundCreation;
                Scrollbit.CreateOptions = BitmapCreateOptions.DelayCreation;
                Scrollbit.SetSource(photo);

                //放入下一張相片
                CollectImagePath.ElementAt(currentindex+1).ImagePath = Scrollbit;

                photo.Close();
                photo.Dispose();
            }
        }

        private void AddPreviousPhoto()
        {
            MediaLibrary mediaLibrary = new MediaLibrary();
            using (Stream photo = mediaLibrary.Pictures.ElementAt(StartAdd+1).GetImage())
            {
                mediaLibrary.Pictures.ElementAt(StartAdd+1).GetImage().Close();
                mediaLibrary.Pictures.ElementAt(StartAdd+1).GetImage().Dispose();
                mediaLibrary.Dispose();

                BitmapImage Scrollbit = new BitmapImage();
                Scrollbit.CreateOptions = BitmapCreateOptions.BackgroundCreation;
                Scrollbit.CreateOptions = BitmapCreateOptions.DelayCreation;
                Scrollbit.SetSource(photo);

                //放入前一張相片
                CollectImagePath.ElementAt(currentindex-1).ImagePath = Scrollbit;

                photo.Close();
                photo.Dispose();
            }
        }

        private void GoReviewPhoto_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PhotoView.SelectedIndex = 1;
        }

        private void PhotoScroller_RefreshRequested(object sender, EventArgs e)
        {
            PhotoView.SelectedIndex = 0;
            PhotoScroller.StopPullToRefreshLoading(true);
        }
    }

    public class PhotoPath
    {
        public ImageSource ImagePath { get; set; }
    }
}