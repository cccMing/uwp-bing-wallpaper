using UwpWallpaper.Pages;
using UwpWallpaper.ViewModels;
using Autofac;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace UwpWallpaper.Services.Navigation
{
    public class NavigationService : INavigationService
    {

        private IFrameAdapter _frameAdapter;
        private IComponentContext _autofacIoc;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frameAdapter">显示内容适配器</param>
        /// <param name="iocResolver">autofac ioc</param>
        public NavigationService(IFrameAdapter frameAdapter, IComponentContext iocResolver)
        {
            _frameAdapter = frameAdapter;
            _autofacIoc = iocResolver;

            #region Page Register

            PageViewModelDic = new Dictionary<Type, NavigatedToViewModelDelegate>();
            //所有页面要注册过来
            RegisterPageViewModel<TodayImage, TodayImageViewModel>();
            RegisterPageViewModel<Gallery, GalleryViewModel>();
            RegisterPageViewModel<Setting, SettingViewModel>();

            #endregion

            frameAdapter.Navigated += Frame_Navigated;
        }


        private delegate Task NavigatedToViewModelDelegate(object page, object parameter, NavigationEventArgs navigationArgs);

        /// <summary>
        /// 页面和viewmodel绑定的字典
        /// </summary>
        private Dictionary<Type, NavigatedToViewModelDelegate> PageViewModelDic { get; }


        /// <summary>
        /// 获取frame导航事件时的钩子
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            if(PageViewModelDic.ContainsKey(e.SourcePageType))
            {
                var loadViewModelDelegate = PageViewModelDic[e.SourcePageType];

                //找到注册的字典中的委托并执行
                loadViewModelDelegate(e.Content, e.Parameter, e);
            }
        }

        public event EventHandler<bool> IsNavigatingChanged;
        public event EventHandler Navigated;

        public bool CanGoBack => throw new NotImplementedException();

        public bool IsNavigating => throw new NotImplementedException();

        #region 页面导航 NavigatTo

        public Task NavigateToTodayImageAsync() => NavigateToPage<TodayImage>();

        public Task NavigateToSettingsAsync() => NavigateToPage<Setting>();

        public Task NavigateToGalleryAsync() => NavigateToPage<Gallery>();

        public Task NavigateToTestAsync() => NavigateToPage<Test>();

        #endregion


        private async Task NavigateToPage<TPage>()
        {
            await NavigateToPage<TPage>(null);
        }

        private async Task NavigateToPage<TPage>(object parameter)
        {
            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                _frameAdapter.Navigate(typeof(TPage), parameter);
            });
        }


        private void RegisterPageViewModel<TPage, TViewModel>() where TViewModel : class
        {
            NavigatedToViewModelDelegate navigatedTo = async (page, parameter, navArgs) =>
            {
                if (page is IPageWithViewModel<TViewModel> pageWithVM)
                {
                    //new 相应页面的viewmodel,跟xaml上相应的东西进行绑定
                    pageWithVM.ViewModel = _autofacIoc.Resolve<TViewModel>();//容器去给显示的页面viewmodel赋值

                    if (pageWithVM.ViewModel is INavigableTo navVM)
                    {
                        await navVM.NavigatedTo(navArgs.NavigationMode, parameter);
                    }

                    // Async loading
                    //pageWithVM.UpdateBindings();
                }
            };

            PageViewModelDic[typeof(TPage)] = navigatedTo;
        }
    }
}
