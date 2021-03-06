using UwpWallpaper.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UwpWallpaper;
using UwpWallpaper.ViewModels;
using CommonUtil;
using Windows.Storage;

namespace UwpWallpaper.BingManager
{
    public class WallpaperManager
    {
        public static void GetWallpaperList(ObservableCollection<Photo> wallpapers, bool isFavorite)
        {
            wallpapers.Clear();
            List<string> files = GetAllFileInFolder("jpg");

            IList<string> favorites = DatabaseManager.GetFavoriteData();

            foreach (var q in files.OrderByDescending(q => q))
            {
                if (isFavorite && !favorites.Contains(Path.GetFileNameWithoutExtension(q)))
                {
                    continue;
                }
                wallpapers.Add(new Photo
                {
                    ImageId = Path.GetFileNameWithoutExtension(q),
                    ImageUri = q,
                    HeartSymbol = (favorites.Contains(Path.GetFileNameWithoutExtension(q))) ? "\xEB52" : "\xEB51"//喜欢，不喜欢
                });
            }
        }

        public static List<string> GetAllFileInFolder(string suffix)
        {
            List<string> filenames = new List<string>();
            foreach (string i in Directory.GetFiles(UwpBing.PicFolderPath))
            {
                if (i.EndsWith(suffix))
                {
                    filenames.Add(i);
                }
            }
            return filenames;
        }
    }
}
