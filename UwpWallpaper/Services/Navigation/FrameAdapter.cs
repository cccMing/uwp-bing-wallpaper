using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace UwpWallpaper.Services.Navigation
{
    /// <summary>
    /// 导航frame的相关动作，主要是页面跳转用
    /// </summary>
    public class FrameAdapter : IFrameAdapter
    {
        private Frame _pageFrame;//这个就是rootpage中显示内容的frame，内容的展示都是基于此
        public FrameAdapter(Frame pageFrame)
        {
            _pageFrame = pageFrame;
        }

        //导航事件，实际上绑到了页面窗口
        public event NavigatedEventHandler Navigated
        {
            add { _pageFrame.Navigated += value; }
            remove { _pageFrame.Navigated -= value; }
        }

        public object Content => _pageFrame.Content;

        public bool CanGoBack => _pageFrame.CanGoBack;

        public void GoBack() => _pageFrame.GoBack();

        public bool Navigate(Type page, object parameter)
        {
            return _pageFrame.Navigate(page, parameter);
        }
    }
}
