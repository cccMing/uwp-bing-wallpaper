using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DapperSqlite.Model;
using Newtonsoft.Json;

namespace DapperSqlite
{
    public class SqlQuery
    {
        private static IDbConnection db = new SqliteConnection("Filename=sqliteFavorite.db");

        public static List<WallpaperInfo> SuggestQuery(string s)
        {

            string likequery = s.Replace("[", "[[]").Replace("%", "[%]");

            var query = db.Query<WallpaperInfo>(@"SELECT title,
                                                        wallpaperno,
                                                        attribute,
                                                        copyright,
                                                        description
                                                    FROM wallpaperinfo
                                                    WHERE wallpaperno LIKE @lq OR 
                                                        copyright LIKE @lq OR 
                                                        description LIKE @lq OR 
                                                        attribute LIKE @lq ;", new { lq = "%" + likequery + "%" });

            return query.ToList();
        }

        /// <summary>
        /// 保存今日图片数据，有直接取，没有则保存
        /// </summary>
        /// <param name="data1">Archive</param>
        /// <param name="data2">CoverStory</param>
        public static WallpaperInfo SaveBingWallpaperInfo(string data1,string data2)
        {
            var info = GetDaysWallpaperInfo(DateTime.Now.ToString("yyyyMMdd"));
            if (info != null)
            {
                return info;
            }

            BingArchive archive = JsonConvert.DeserializeObject<BingArchive>(data1);
            BingConverStory converStory = JsonConvert.DeserializeObject<BingConverStory>(data2);

            double lon = 0;
            if (!string.IsNullOrEmpty(converStory.Longitude))
            {
                try
                {
                    lon = Convert.ToDouble(converStory.Longitude);
                }
                catch { }
            }
            double lat = 0;
            if (!string.IsNullOrEmpty(converStory.Latitude))
            {
                try
                {
                    lat = Convert.ToDouble(converStory.Latitude);
                }
                catch { }
            }

            WallpaperInfo winfo = new WallpaperInfo()
            {
                WallpaperNo = DateTime.Now.ToString("yyyyMMdd"),
                Title = converStory.Title,
                CopyRight = archive.Images[0]?.Copyright,
                Description = converStory.Para1,
                Attribute = converStory.Attribute,
                Longitude = lon,
                Latitude = lat,
                PicUrl = "http://www.bing.com" + archive.Images[0]?.Url,
                IsFavorite = false,
                OriginData1 = data1,
                OriginData2 = data2,
            };
            //Message=SafeHandle cannot be null.

            var query = db.Execute(@"INSERT INTO wallpaperinfo (    wallpaperno,
                                                                    title,
                                                                    copyright,
                                                                    description,
                                                                    attribute,
                                                                    longitude,
                                                                    latitude,
                                                                    picurl,
                                                                    isfavorite,
                                                                    origindata1,
                                                                    origindata2
                                                                )
                                                                VALUES (
                                                                    @WallpaperNo,
                                                                    @Title,
                                                                    @CopyRight,
                                                                    @Description,
                                                                    @Attribute,
                                                                    @Longitude,
                                                                    @Latitude,
                                                                    @PicUrl,
                                                                    @IsFavorite,
                                                                    @OriginData1,
                                                                    @OriginData2
                                                                );", winfo);
            return winfo;
        }

        
        public static WallpaperInfo GetDaysWallpaperInfo(string dateNo)
        {
            var beforeCreate = db.Execute(@"CREATE TABLE IF NOT EXISTS wallpaperinfo (
                                                wallpaperno VARCHAR (10),
                                                title       VARCHAR (100),
                                                copyright   TEXT,
                                                description TEXT,
                                                attribute   VARCHAR (255),
                                                longitude   NUMERIC (18, 8),
                                                latitude    NUMERIC (18, 8),
                                                picurl      VARCHAR (255),
                                                isfavorite  BOOLEAN,
                                                origindata1 TEXT,
                                                origindata2 TEXT
                                            ); ");

            var query = db.Query<WallpaperInfo>(@"SELECT *
                                                        FROM wallpaperinfo WHERE wallpaperno=@No;", new { No = dateNo });

            return query.FirstOrDefault();
        }

        public static List<WallpaperInfo> GetDaysWallpaperInfo(IList<string> datelist)
        {
            return GetDaysWallpaperInfo(datelist.ToArray());
        }

        public static List<WallpaperInfo> GetDaysWallpaperInfo(string[] dateNos)
        {
            var query = db.Query<WallpaperInfo>(@"SELECT *
                                                        FROM wallpaperinfo WHERE wallpaperno in @Nos;", new { Nos = dateNos });

            return query.ToList();
        }

        public static bool CheckIsAlreadySaved()
        {
            var query = db.Query<WallpaperInfo>(@"SELECT wallpaperno
                                                        FROM wallpaperinfo WHERE wallpaperno=@No;", new { No = DateTime.Now.ToString("yyyyMMdd") });

            return query.ToList().Count > 0;
        }

        public static void AddFaovriteByDayId(string dayid)
        {
            var query = db.Execute(@"UPDATE wallpaperinfo
                                       SET isfavorite = 1
                                     WHERE wallpaperno = @No;", new { No = dayid });
        }

        public static void DelFaovriteByDayId(string dayid)
        {
            var query = db.Execute(@"UPDATE wallpaperinfo
                                       SET isfavorite = 0
                                     WHERE wallpaperno = @No;", new { No = dayid });
        }

        public static void DeleteImageByDayId(string dayid)
        {
            var query = db.Execute(@"DELETE FROM wallpaperinfo
                                                        WHERE wallpaperno=@No;", new { No = dayid });
        }
    }
}
