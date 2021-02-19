using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace UwpWallpaper
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SuggestBoxView : Page
    {
        string querykey;

        private ObservableCollection<SqliteManager.Models.WallpaperInfo> listInfo;

        public SuggestBoxView()
        {
            this.InitializeComponent();

            listInfo = new ObservableCollection<SqliteManager.Models.WallpaperInfo>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            querykey = e.Parameter as string;
            base.OnNavigatedTo(e);
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            string path = await BingManager.FileManager.GetStorePath();
            IList<SqliteManager.Models.WallpaperInfo> walls = SqliteManager.SqlQuery.SuggestQuery(querykey);

            int count = 0;
            foreach (var i in walls)
            {
                if (count++ > 8)
                {
                    break;
                }
                listInfo.Add(new SqliteManager.Models.WallpaperInfo
                {
                    WallpaperNo = Path.Combine(path, i.WallpaperNo + ".jpg"),
                    Title = i.Title + "(" + i.WallpaperNo + ")",
                    Description = i.Description,
                    CopyRight = i.CopyRight,
                    Attribute = i.Attribute
                });
            }
        }
    }
}
