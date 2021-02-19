using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UwpWallpaper.Models
{
    public class ExploreMap
    {
        /// <summary>
        /// 本地图片地址，不包括前面一串
        /// </summary>
        public string ImageId { get; set; }

        public string ImgSource { get; set; }

        public Windows.Devices.Geolocation.Geopoint Location
        {
            get
            {
                var basicPosition = new Windows.Devices.Geolocation.BasicGeoposition();
                basicPosition.Longitude = this.Longitude;
                basicPosition.Latitude = this.Latitude;
                basicPosition.Altitude = 0;
                return new Windows.Devices.Geolocation.Geopoint(basicPosition);
            }
        }


        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}
