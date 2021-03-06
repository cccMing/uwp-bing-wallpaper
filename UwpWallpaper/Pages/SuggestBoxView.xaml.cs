using CommonUtil;
using SqliteManager.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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

        private ObservableCollection<WallpaperInfoPo> listInfo;

        public SuggestBoxView()
        {
            this.InitializeComponent();

            listInfo = new ObservableCollection<WallpaperInfoPo>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            querykey = e.Parameter as string;
            base.OnNavigatedTo(e);
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            IList<WallpaperInfoPo> walls = SqliteManager.SqlQuery.SuggestQuery(querykey);

            int count = 0;
            foreach (var i in walls)
            {
                if (count++ > 8)
                {
                    break;
                }
                listInfo.Add(new WallpaperInfoPo
                {
                    WallpaperNo = Path.Combine(UwpBing.PicFolderPath, i.WallpaperNo + ".jpg"),
                    Title = i.Title + "(" + i.WallpaperNo + ")",
                    Description = i.Description,
                    CopyRight = i.CopyRight,
                    Attribute = i.Attribute
                });
            }
        }
    }
}
