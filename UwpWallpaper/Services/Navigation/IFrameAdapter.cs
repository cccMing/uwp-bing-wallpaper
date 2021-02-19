using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace UwpWallpaper.Services.Navigation
{
    public interface IFrameAdapter
    {
        /// <summary>
        /// 导航跳转事件
        /// </summary>
        event NavigatedEventHandler Navigated;

        object Content { get; }

        bool CanGoBack { get; }

        void GoBack();

        bool Navigate(Type page, object parameter);
    }
}
