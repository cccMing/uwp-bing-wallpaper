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

namespace UwpWallpaper.BingManager
{
    public class WallpaperManager
    {
        public async static Task GetWallpaperList(ObservableCollection<Photo> wallpapers, bool isFavorite)
        {
            string path = await FileManager.GetStorePath();
            wallpapers.Clear();
            List<string> files = await FileManager.GetAllFileInFolder("jpg");

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
    }
}
