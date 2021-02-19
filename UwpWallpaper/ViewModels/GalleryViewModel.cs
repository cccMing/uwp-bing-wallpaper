using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UwpWallpaper.BingManager;
using UwpWallpaper.Services.Navigation;
using UwpWallpaper.Util;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UwpWallpaper.ViewModels
{
    public class GalleryViewModel
    {
        private INavigationService _navigationService;

        public GalleryViewModel(INavigationService navigationService)
        {
            GalleryList = new ObservableCollection<Photo>();

            _navigationService = navigationService;
        }

        public async Task GetImageListAsync()
        {
            await WallpaperManager.GetWallpaperList(GalleryList, false);
        }

        public async Task GetFavoriteImageListAsync(object sender, RoutedEventArgs e)
        {
            bool isChecked = (bool)(e.OriginalSource as AppBarToggleButton)?.IsChecked;
            await WallpaperManager.GetWallpaperList(GalleryList, isChecked);
        }

        public ObservableCollection<Photo> GalleryList;
    }

    public class Photo
    {
        public string ImageId { get; set; }

        public string ImageUri { get; set; }

        public string HeartSymbol { get; set; }

        public async Task HeartBtn_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string buttonShow = button.Content.ToString();
            try
            {
                if (buttonShow == "\xEB51")
                {
                    button.Content = "\xEB52";
                    DatabaseManager.AddFavorite(button.Name);
                    SqliteManager.SqlQuery.AddFaovriteByDayId(button.Name);
                }
                else
                {
                    button.Content = "\xEB51";
                    DatabaseManager.DelFavorite(button.Name);
                    SqliteManager.SqlQuery.DelFaovriteByDayId(button.Name);
                }

            }
            catch (Exception)
            {
                await new MessageHelper().ShowDialogAsync($"{LangResource.GetString("SaveFailed")},{LangResource.GetString("TryAgainLater")}");

                button.Content = buttonShow;
            }
        }
    }
}
