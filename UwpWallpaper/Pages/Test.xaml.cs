using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace UwpWallpaper.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Test : Page
    {
        public Test()
        {
            this.InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //MyImage.RenderTransform.
            //var stf = new ScaleTransform
            //{
            //    CenterX = MyImage.ActualWidth / 2,
            //    CenterY = MyImage.ActualHeight / 2,
            //    ScaleX = 1.1,
            //    ScaleY = 1.1
            //};
            //MyImage.RenderTransform = stf;


            //RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
            //await renderTargetBitmap.RenderAsync(MyGrid, 50, 50);
            //var picker = new FileSavePicker();
            //picker.FileTypeChoices.Add("JPEG Image", new string[] { ".jpg" });
            //StorageFile file = await picker.PickSaveFileAsync();
            //if (file != null)
            //{
            //    var pixels = await renderTargetBitmap.GetPixelsAsync();

            //    using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            //    {
            //        var encoder = await
            //            BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
            //        byte[] bytes = pixels.ToArray();
            //        encoder.SetPixelData(BitmapPixelFormat.Bgra8,
            //                             BitmapAlphaMode.Ignore,
            //                             150, 150,
            //                             96, 96, bytes);

            //        await encoder.FlushAsync();
            //    }
            //}
        }

        private void AnimationImage()
        {

            Duration duration = new Duration(TimeSpan.FromSeconds(3));
            Storyboard sb = new Storyboard();

            DoubleAnimation anix = new DoubleAnimation
            {
                From = 1.2,
                To = 1,
                Duration = duration
            };
            DoubleAnimation aniy = new DoubleAnimation
            {
                From = 1.2,
                To = 1,
                Duration = duration
            };

            sb.Children.Add(anix);
            sb.Children.Add(aniy);

            Storyboard.SetTarget(anix, this.imgScale);
            Storyboard.SetTarget(aniy, this.imgScale);
            Storyboard.SetTargetProperty(anix, "ScaleX");
            Storyboard.SetTargetProperty(aniy, "ScaleY");

            sb.Begin();
        }

        private void MyImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void MyImage_Loaded(object sender, RoutedEventArgs e)
        {
            AnimationImage();
        }
    }
}
