using SqliteManager.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UwpWallpaper.BingManager;
using UwpWallpaper.Services.Navigation;
using UwpWallpaper.Util;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace UwpWallpaper.ViewModels
{
    public class TodayWallpaperViewModel : ObservableObject
    {

        private INavigationService _navigationService;

        public TodayWallpaperViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public async Task Load_TodayWallPaper(object sender)
        {
            ProcessRingVisibility = Visibility.Visible;

            WallpaperInfo winfo;
            try
            {
                //页面加载完获取今日壁纸数据
                winfo = await HttpManager.GetWallpaperInfoAsync();
            }
            catch (System.Net.Http.HttpRequestException)
            {
                await new MessageDialog(LangResource.GetString("NetConnectError")).ShowAsync();
                return;
            }

            BitmapImage bi = await HttpManager.GetImageOrSave(winfo.WallpaperNo, winfo.PicUrl);
            if (bi == null)
            {
                await new MessageDialog(LangResource.GetString("NetConnectError")).ShowAsync();
                return;
            }
            TodayImage = bi;

            BriefDescription = $"{winfo.Title} - {winfo.CopyRight}";
            ExpandDescription = $"{winfo.Title} - {winfo.CopyRight}{Environment.NewLine}" +
                                    $"{winfo.Description}";
            Longitude = winfo.Longitude;
            Latitude = winfo.Latitude;

            CoverPanelWidth = GetCoverWidth((sender as TodayWallpaper).ActualWidth, (sender as TodayWallpaper).ActualHeight);

            //ToolTip toolTip = new ToolTip
            //{//地图按钮的提示：地点
            //    Content = winfo.Attribute
            //};
            //ToolTipService.SetToolTip(this.ShowMapButton, toolTip);

            if (winfo.Latitude == 0 || winfo.Longitude == 0)
            {
                _mapBtnVisible = Visibility.Collapsed;
            }

            ProcessRingVisibility = Visibility.Collapsed;
        }

        #region 前端使用到的控件绑定

        private Visibility _ringVisible = Visibility.Visible;
        public Visibility ProcessRingVisibility
        {
            get => _ringVisible;
            set => Set(ref _ringVisible, value);
        }

        private Visibility _mapBtnVisible = Visibility.Collapsed;
        public Visibility MapButtonVisibility
        {
            get => _mapBtnVisible;
            set => Set(ref _mapBtnVisible, value);
        }

        public string Attribute { get; private set; }


        private BitmapImage _todayImage;
        public BitmapImage TodayImage
        {
            get => _todayImage;
            private set => Set(ref _todayImage, value);
        }

        private double _coverPanelWidth;
        public double CoverPanelWidth
        {
            get => _coverPanelWidth;
            set => Set(ref _coverPanelWidth, value);
        }

        public string CollaspedBtnName
        {
            get => _descExpaned ? "\xE70D" : "\xE70E";
        }

        private bool _descExpaned = false;
        public bool DescExpand
        {
            get => _descExpaned;
            set => Set(ref _descExpaned, value);
        }

        public string ImageDescription
        {
            get => _descExpaned ? _expandDesc : _briefDesc;
        }

        private string _briefDesc = string.Empty;
        private string BriefDescription
        {
            get => _briefDesc;
            set => Set(ref _briefDesc, value, nameof(ImageDescription));
        }

        private string _expandDesc = string.Empty;
        private string ExpandDescription
        {
            get => _expandDesc;
            set => Set(ref _expandDesc, value, nameof(ImageDescription));
        }

        #endregion

        private double Longitude { get; set; }
        private double Latitude { get; set; }
        public Windows.Devices.Geolocation.Geopoint gpoint
        {
            get
            {
                Windows.Devices.Geolocation.BasicGeoposition basicposition;
                basicposition.Longitude = this.Longitude;
                basicposition.Latitude = this.Latitude;
                basicposition.Altitude = 0;
                return new Windows.Devices.Geolocation.Geopoint(basicposition);
            }
        }

        #region ClickEvents

        /// <summary>
        /// 切换底部信息的show hide
        /// </summary>
        public void ToggleDetail_Click()
        {
            _descExpaned = !_descExpaned;
            NotifyPropertyChanged(nameof(ImageDescription));
            NotifyPropertyChanged(nameof(CollaspedBtnName));
        }

        /// <summary>
        /// 展示图片在地图中的位置
        /// </summary>
        public void GotoMap_Click()
        {

        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// 图像信息
        /// </summary>
        private class ImagePack
        {
            public BitmapImage Image { get; set; }

            public string Description { get; set; }
            
        }

        /// <summary>
        /// 获取图片下简介的宽度
        /// </summary>
        /// <param name="pagewidth">页面宽度</param>
        /// <param name="pageheight">页面高度</param>
        /// <returns></returns>
        private double GetCoverWidth(double pagewidth, double pageheight)
        {
            double coverwidth = 500;
            if (pagewidth > 0 && pageheight > 0)//这个页面的宽度
            {
                double whp = 1920 / 1080.0;//图片宽高比
                coverwidth = (pagewidth / pageheight) > whp ? whp * pageheight : pagewidth;
            }

            return coverwidth;
        }

        #endregion
    }
}
