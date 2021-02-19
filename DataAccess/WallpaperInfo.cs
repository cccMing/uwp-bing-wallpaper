using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary
{
    public class WallpaperInfo
    {
        public string ShowDate { get; set; }
        public string Title { get; set; }
        public string CopyRight { get; set; }
        public string Description { get; set; }
        public string Attribute { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public string Url { get; set; }
    }
}
