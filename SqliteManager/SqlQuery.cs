using Microsoft.Data.Sqlite;
using SQLite.Net;
using SqliteManager.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Platform.WinRT;
using System.IO;
using Newtonsoft.Json;
using CommonUtil;

namespace SqliteManager
{
    public class SqlQuery
    {
        private static string path = Path.Combine(UwpBing.Folder.Path, "sqliteFavorite.db");

        /// <summary>
        /// 现在没用到
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static List<WallpaperInfo> SuggestQuery(string s)
        {
            string likequery = s.Replace("[", "[[]").Replace("%", "[%]");
            likequery = "%" + likequery + "%";

            using (var conn = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                var query = conn.Query<WallpaperInfo>(@"SELECT title,
                                                        wallpaperno,
                                                        attribute,
                                                        copyright,
                                                        description
                                                    FROM wallpaperinfo
                                                    WHERE wallpaperno LIKE ? OR 
                                                        copyright LIKE ? OR 
                                                        description LIKE ? OR 
                                                        attribute LIKE ? ;", likequery, likequery, likequery, likequery);

                return query;
            }
        }

        /// <summary>
        /// 保存今日图片数据，有直接取，没有则保存
        /// </summary>
        /// <param name="data1">Archive</param>
        /// <param name="data2">CoverStory</param>
        public static WallpaperInfo SaveBingWallpaperInfo(string data1, string data2)
        {
            var info = GetDaysWallpaperInfo(DateHelper.CurrentDateStr);
            if (info != null)
            {
                return info;
            }

            BingArchive archive = JsonConvert.DeserializeObject<BingArchive>(data1);
            BingConverStory converStory = new BingConverStory();

            try//别的地区好像获取不到图片故事
            {
                converStory = JsonConvert.DeserializeObject<BingConverStory>(data2);
            }
            catch { }

            WallpaperInfo wallinfo = new WallpaperInfo()
            {
                WallpaperNo = DateHelper.CurrentDateStr,
                Title = converStory?.Title,
                CopyRight = archive.Images[0]?.Copyright,
                Description = converStory?.Para1,
                Attribute = converStory?.Attribute,
                Longitude = UwpConverter.ToDouble(converStory?.Longitude),
                Latitude = UwpConverter.ToDouble(converStory?.Latitude),
                PicUrl = archive.Images[0]?.Url,
                IsFavorite = false,
                OriginData1 = data1,
                OriginData2 = data2
            };
            //Message=SafeHandle cannot be null.

            using (var conn = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                conn.Insert(wallinfo);
            }

            return wallinfo;
        }

        public static Dictionary<string, string> SavePrevWallpaperInfo(IList<string> arcs, IDictionary<string, string> coverStories)
        {
            Dictionary<string, string> retdic = new Dictionary<string, string>();

            IList<Image> arlist = new List<Image>();//所有model image集合
            foreach (var i in arcs)//两组
            {
                IList<Image> arobj = JsonConvert.DeserializeObject<BingArchive>(i).Images;

                foreach (var archive in arobj)//天循环
                {
                    if (coverStories.ContainsKey(archive.Enddate))
                    {
                        BingConverStory coverStory = new BingConverStory();
                        try
                        {
                            coverStory = JsonConvert.DeserializeObject<BingConverStory>(coverStories[archive.Enddate]);
                        }
                        catch { }
                        var info = SaveArchiveCover(archive, coverStory);
                        retdic.Add(info.WallpaperNo, info.PicUrl);
                    }
                }
            }

            return retdic;
        }

        public static WallpaperInfo SaveArchiveCover(Image imginfo, BingConverStory converStory)
        {
            WallpaperInfo winfo = new WallpaperInfo()
            {
                WallpaperNo = imginfo.Enddate,
                Title = converStory?.Title,
                CopyRight = imginfo?.Copyright,
                Description = converStory?.Para1,
                Attribute = converStory?.Attribute,
                Longitude = UwpConverter.ToDouble(converStory?.Longitude),
                Latitude = UwpConverter.ToDouble(converStory?.Latitude),
                PicUrl = imginfo.Url,
                IsFavorite = false,
                OriginData1 = JsonConvert.SerializeObject(imginfo),
                OriginData2 = JsonConvert.SerializeObject(converStory)
            };

            using (var conn = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                conn.Insert(winfo);
            }

            return winfo;
        }


        public static WallpaperInfo GetDaysWallpaperInfo(string dateNo)
        {
            using (var conn = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                var beforeCreate = conn.Execute(@"CREATE TABLE IF NOT EXISTS wallpaperinfo (
                                                id          INTEGER         PRIMARY KEY AUTOINCREMENT,
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

                var afterCreate = conn.Execute(@"UPDATE wallpaperinfo
                                        SET picurl = [replace](picurl, 'http://www.bing.com', ''); ");

                var reswall = conn.Find<WallpaperInfo>(q => q.WallpaperNo == dateNo);

                return reswall;
            }
        }

        public static List<WallpaperInfo> GetDaysWallpaperInfos(IList<string> dates)
        {
            IList<string> querylist = dates.Select(q => string.Format("'{0}'", q)).ToList();
            using (var conn = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                var query = conn.Query<WallpaperInfo>($@"SELECT * FROM wallpaperinfo 
                                                        WHERE wallpaperno in ({string.Join(",", querylist)}) 
                                                        order by wallpaperno desc");
                return query.ToList();
            }
        }

        //public static List<WallpaperInfo> GetDaysWallpaperInfo(IList<string> datelist)
        //{
        //    return GetDaysWallpaperInfo(datelist.ToArray());
        //}

        //public static List<WallpaperInfo> GetDaysWallpaperInfo(string[] dateNos)
        //{
        //    var query = conn.Query<WallpaperInfo>(@"SELECT *
        //                                                FROM wallpaperinfo WHERE wallpaperno in @Nos;", new { Nos = dateNos });

        //    return query.ToList();
        //}

        public static bool CheckIsAlreadySaved()
        {
            using (var conn = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                var query = conn.Query<WallpaperInfo>(@"SELECT wallpaperno
                                                        FROM wallpaperinfo WHERE wallpaperno=?;", DateTime.Now.ToString("yyyyMMdd"));

                return query.ToList().Count > 0;
            }
        }

        public static void AddFaovriteByDayId(string dayid)
        {
            using (var conn = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                var query = conn.Execute(@"UPDATE wallpaperinfo
                                       SET isfavorite = 1
                                     WHERE wallpaperno = ?;", dayid);

            }
        }

        public static void DelFaovriteByDayId(string dayid)
        {
            using (var conn = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                var query = conn.Execute(@"UPDATE wallpaperinfo
                                       SET isfavorite = 0
                                     WHERE wallpaperno = ?;", dayid);
            }
        }

        public static void DeleteImageByDayId(string dayid)
        {
            using (var conn = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                var query = conn.Execute(@"DELETE FROM wallpaperinfo
                                                        WHERE wallpaperno=?;", dayid);
            }
        }
    }
}
