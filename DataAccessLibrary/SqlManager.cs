using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace DataAccessLibrary
{
    public class SqlManager
    {

        //public static void InitializeDatabase()
        //{
        //    using (SqliteConnection db = new SqliteConnection("Filename=sqliteFavorite.db"))
        //    {
        //        db.Open();

        //        String tableCommand = "CREATE TABLE IF NOT " +
        //            "EXISTS wallpaperfavorite (id INTEGER PRIMARY KEY AUTOINCREMENT, " +
        //            "favoriteno NVARCHAR(2048) NULL)";

        //        SqliteCommand createTable = new SqliteCommand(tableCommand, db);

        //        createTable.ExecuteReader();
        //    }
        //}

        public static void AddFavorite(string wallpaperno)
        {
            using (SqliteConnection db = new SqliteConnection("Filename=sqliteFavorite.db"))
            {
                db.Open();

                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                // Use parameterized query to prevent SQL injection attacks
                insertCommand.CommandText = @"UPDATE wallpaperinfo
                                                SET isfavorite = 1
                                             WHERE wallpaperno = @wallpaperno;";
                insertCommand.Parameters.AddWithValue("@wallpaperno", wallpaperno);

                insertCommand.ExecuteReader();

                db.Close();
            }

        }

        public static void DelFavorite(string wallpaperno)
        {
            using (SqliteConnection db = new SqliteConnection("Filename=sqliteFavorite.db"))
            {
                db.Open();

                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                // Use parameterized query to prevent SQL injection attacks
                insertCommand.CommandText = @"UPDATE wallpaperinfo
                                                SET isfavorite = 0
                                             WHERE wallpaperno = @wallpaperno;";
                insertCommand.Parameters.AddWithValue("@wallpaperno", wallpaperno);

                insertCommand.ExecuteReader();

                db.Close();
            }

        }

        public static List<String> GetFavoriteData()
        {
            List<String> entries = new List<string>();

            using (SqliteConnection db =
                new SqliteConnection("Filename=sqliteFavorite.db"))
            {
                db.Open();

                SqliteCommand selectCommand = new SqliteCommand
                    ("SELECT wallpaperno from wallpaperinfo where isfavorite = 1", db);

                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    entries.Add(query.GetString(0));
                }

                db.Close();
            }

            return entries;
        }
    }
}
