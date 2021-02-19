using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace UwpWallpaper.Models
{
    public class WallpaperViews
    {
        /// <summary>
        /// 图片本地地址
        /// </summary>
        public string ImageUri { get; set; }

        /// <summary>
        /// 图片标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public BitmapImage Image { get; set; }

        public string Descript { get; set; }

        public string CopyRight { get; set; }

        public string ImageId { get; set; }

        /// <summary>
        /// &#xEB51;EB52
        /// </summary>
        public string HeartSymbol { get; set; }

        /// <summary>
        /// 地图点
        /// </summary>
        public Windows.Devices.Geolocation.Geopoint Gpoint {
            get
            {
                Windows.Devices.Geolocation.BasicGeoposition geoposition;

                geoposition.Longitude = this.Longitude;
                geoposition.Latitude = this.Latitude;
                geoposition.Altitude = 0;

                return new Windows.Devices.Geolocation.Geopoint(geoposition);
            }
        }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}
