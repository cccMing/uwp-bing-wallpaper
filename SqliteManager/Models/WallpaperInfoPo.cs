using SQLite.Net.Attributes;

namespace SqliteManager.Models
{
    /// <summary>
    /// 数据库wallpaperinfo对应的model
    /// </summary>
    [Table("WallpaperInfo")]
    public class WallpaperInfoPo
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        [Column("wallpaperno")]
        public string WallpaperNo { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("copyright")]
        public string CopyRight { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("attribute")]
        public string Attribute { get; set; }

        [Column("longitude")]
        public double Longitude { get; set; }

        [Column("latitude")]
        public double Latitude { get; set; }

        [Column("picurl")]
        public string PicUrl { get; set; }

        [Column("isfavorite")]
        public bool IsFavorite { get; set; }

        [Column("origindata1")]
        public string OriginData1 { get; set; }

        [Column("origindata2")]
        public string OriginData2 { get; set; }
    }
}
