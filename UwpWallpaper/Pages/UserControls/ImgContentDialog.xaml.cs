using CommonUtil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UwpWallpaper.BingManager;
using UwpWallpaper.Util;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System.UserProfile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace UwpWallpaper.Pages.UserControls
{
    public sealed partial class ImgContentDialog : ContentDialog
    {
        /// <summary>
        /// 图片id
        /// </summary>
        public string ImgId { get; set; }
        public ImgContentDialog()
        {
            this.PrimaryButtonText = LangResource.GetString("SetImgAsBackground");
            this.SecondaryButtonText = LangResource.GetString("SetImgAsLockScreen");
            this.CloseButtonText = LangResource.GetString("Close");

            this.InitializeComponent();
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            await WallpaperSetting.SetWallpaper(ImgId, BackgroundEnum.Destop);
        }

        private async void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            await WallpaperSetting.SetWallpaper(ImgId, BackgroundEnum.LockScreen);
        }
        
        private async void ContentDialog_Loading(FrameworkElement sender, object args)
        {
            var wallinfo = await HttpManager.GetWallpaperInfoAsync(ImgId);
            if (wallinfo == null)
            {
                return;
            }

            BitmapImage img = await HttpManager.GetImageOrSave(ImgId, "");
            if (img == null)
            {
                return;
            }

            this.Title = wallinfo.CopyRight;
            this.ContentImage.Source = img;
        }
    }
}
