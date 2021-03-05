using CommonUtil;
using SqliteManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UwpWallpaper.BingManager;
using UwpWallpaper.Util;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace UwpWallpaper.ViewModels
{
    public class TodayImageViewModel
    {
        public TodayImageViewModel()
        {
            ImageInfos = new ObservableCollection<ImageInfo>();
            ImageInfos.Clear();
        }

        public ObservableCollection<ImageInfo> ImageInfos { get; set; }

        /// <summary>
        /// 获取指定张数图片信息
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task GetDaysImageInfoAsync(int count)
        {
            ImageInfos.Clear();
            if (this.ImageInfos.Count() == count)
            {
                return;
            }

            IList<string> days = new List<string>();
            for (var i = 0; i < count; i++)
            {
                days.Add(DateHelper.GetDateStr(-1 * i));
            }

            await GetWallpaperInfoByDays(days);
        }

        /// <summary>
        /// 获取指定日期图片
        /// </summary>
        /// <param name="picid"></param>
        /// <returns></returns>
        public async Task GetDaysImageInfoAsync(string picid)
        {
            ImageInfos.Clear();//性能不好
            await GetWallpaperInfoByDays(new List<string> { picid });
        }

        private async Task GetWallpaperInfoByDays(IList<string> days)
        {

            List<WallpaperInfoPo> wallpaperInfos = await HttpManager.GetWallpaperInfosAsync(days);

            foreach (var i in wallpaperInfos)
            {
                if (ImageInfos.Any(q => q.ImgNo == i.WallpaperNo))
                {
                    continue;
                }

                var bi = await HttpManager.GetImageOrSave(i.WallpaperNo, i.PicUrl);

                if (bi == null)
                {
                    continue;
                }

                ImageInfos.Add(new ImageInfo(i, bi));
            }
        }
    }

    public class ImageInfo : ObservableObject
    {
        public ImageInfo(WallpaperInfoPo wallpaperInfo, BitmapImage bi)
        {
            ImageUri = bi;
            this._imgNo = wallpaperInfo.WallpaperNo;
            this._title = wallpaperInfo.Title;
            this._copyRight = wallpaperInfo.CopyRight;
            this._attribute = wallpaperInfo.Attribute;
            this._detail = wallpaperInfo.Description;
            this._introduction = string.IsNullOrEmpty(_title) ?
                                _copyRight :
                                $"{_title} - {_copyRight}";

            this._longitude = wallpaperInfo.Longitude;
            this._latitude = wallpaperInfo.Latitude;
            this._mapVisible = !(wallpaperInfo.Longitude == 0 || wallpaperInfo.Longitude == 0);
            this._detailVisible = !string.IsNullOrEmpty(_detail);
        }

        private string _title;
        private string _copyRight;
        private string _detail;

        private string _imgNo;
        public string ImgNo
        {
            get => _imgNo;
        }

        public ImageSource ImageUri { get; set; }

        private string _introduction;
        public string Introduction
        {
            get => _introduction;
            set => Set<string>(ref _introduction, value);
        }

        private double _textWidth;
        public double TextWidth
        {
            get => _textWidth;
            set => Set<double>(ref _textWidth, value);
        }

        private string _toggleBtnCnt = "\xE70E";
        public string ToggleBtnCnt
        {
            get => _toggleBtnCnt;
            set => Set<string>(ref _toggleBtnCnt, value);
        }

        /// <summary>
        /// 定位图标
        /// </summary>
        public string LocationBtnCnt => "\xE707";

        /// <summary>
        /// 链接图标
        /// </summary>
        public string InDetailCnt => "\xE71B";

        private double _longitude = 0;
        private double _latitude = 0;
        public Windows.Devices.Geolocation.Geopoint GPoint
        {
            get
            {
                Windows.Devices.Geolocation.BasicGeoposition basicposition;
                basicposition.Longitude = this._longitude;
                basicposition.Latitude = this._latitude;
                basicposition.Altitude = 0;
                return new Windows.Devices.Geolocation.Geopoint(basicposition);
            }
        }

        private string _attribute;
        public string Attribute
        {
            get => _attribute;
        }

        private bool _mapVisible;
        public Visibility MapVisible
        {
            get => _mapVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        private bool _detailVisible;
        /// <summary>
        /// 右下角的详细信息箭头是否可见
        /// </summary>
        public Visibility DetailVisible
        {
            get => _detailVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        private bool _expand = false;
        public void ToggleInfo_Click()
        {
            _expand = !_expand;
            if (_expand)//展示详细信息
            {
                Introduction = $"{_title} - {_copyRight}{Environment.NewLine}" +
                                $"{_detail}";
                ToggleBtnCnt = "\xE70D";
            }
            else
            {
                Introduction = $"{_title} - {_copyRight}";
                ToggleBtnCnt = "\xE70E";
            }
        }
    }
}
