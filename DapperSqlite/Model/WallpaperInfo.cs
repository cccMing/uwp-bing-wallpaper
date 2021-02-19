using System;
using System.Collections.Generic;
using System.Text;

namespace DapperSqlite.Model
{
    /// <summary>
    /// 数据库wallpaperinfo对应的model
    /// </summary>
    public class WallpaperInfo
    {
        public string WallpaperNo { get; set; }
        public string Title { get; set; }
        public string CopyRight { get; set; }
        public string Description { get; set; }
        public string Attribute { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string PicUrl { get; set; }
        public bool IsFavorite { get; set; }
        public string OriginData1 { get; set; }
        public string OriginData2 { get; set; }
    }
}
