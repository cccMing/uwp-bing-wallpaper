using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UwpWallpaper
{
    public class DatabaseManager
    {
        /// <summary>
        /// 程序起来时创建数据库及表
        /// </summary>
        public static void InitializeDatabase()
        {
            using (SqliteConnection db = new SqliteConnection("Filename=sqliteFavorite.db"))
            {
                db.Open();

                String tableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS wallpaperfavorite (id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                    "favoriteno NVARCHAR(2048) NULL)";

                SqliteCommand createTable = new SqliteCommand(tableCommand, db);

                createTable.ExecuteReader();
            }
        }

        public static void AddFavorite(string favoriteNo)
        {
            using (SqliteConnection db = new SqliteConnection("Filename=sqliteFavorite.db"))
            {
                db.Open();

                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                // Use parameterized query to prevent SQL injection attacks
                insertCommand.CommandText = "INSERT INTO wallpaperfavorite VALUES (NULL, @Entry);";
                insertCommand.Parameters.AddWithValue("@Entry", favoriteNo);

                insertCommand.ExecuteReader();

                db.Close();
            }

        }

        public static void DelFavorite(string favoriteNo)
        {
            using (SqliteConnection db = new SqliteConnection("Filename=sqliteFavorite.db"))
            {
                db.Open();

                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                // Use parameterized query to prevent SQL injection attacks
                insertCommand.CommandText = "DELETE FROM wallpaperfavorite WHERE favoriteno=@Entry;";
                insertCommand.Parameters.AddWithValue("@Entry", favoriteNo);

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
                    ("SELECT favoriteno from wallpaperfavorite", db);

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
