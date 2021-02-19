using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DapperSqlite.Model;
using Microsoft.Data.Sqlite;

namespace DapperSqlite
{

    public class SqLiteBaseRepository
    {
        static IDbConnection db = new SqliteConnection("Filename=sqliteFavorite.db");

        public static IList<WallpaperInfo> WhatTheFuck()
        {
            String query = "select * from wallpaperinfo where id in @ids";
            IEnumerable<WallpaperInfo> ws = db.Query<WallpaperInfo>(query, new { ids = new[] { 36, 37, 38 } });
            return ws.ToList();
        }

        public static IList<Favorite> What()
        {
            String query = "insert into wallpaperfavorite(favoriteno) values(@FavoriteNo)";
            IEnumerable<Favorite> ws = db.Query<Favorite>(query, new Favorite { FavoriteNo = "1234df5" });
            return ws.ToList();
        }

        public class Favorite
        {
            public long Id { get; set; }
            public string FavoriteNo { get; set; }
        }
    }
}
