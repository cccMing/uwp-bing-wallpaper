using CommonUtil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UwpWallpaper.BingManager;
using UwpWallpaper.Pages.UserControls;
using UwpWallpaper.Services.Navigation;
using UwpWallpaper.Util;
using UwpWallpaper.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace UwpWallpaper.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Gallery : Page, IPageWithViewModel<GalleryViewModel>
    {
        private int _animationDuration = 400;
        private Compositor _compositor;

        public GalleryViewModel ViewModel { get; set; }

        public Gallery()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.GetImageListAsync();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _compositor = ElementCompositionPreview.GetElementVisual(this)?.Compositor;
            if (e.Parameter != null)
                int.TryParse(e.Parameter.ToString(), out _animationDuration);

            base.OnNavigatedTo(e);
        }

        #region 关于图片动画
        private void PhotoCollectionViewer_ChoosingItemContainer(ListViewBase sender, ChoosingItemContainerEventArgs args)
        {
            args.ItemContainer = args.ItemContainer ?? new GridViewItem();

            var fadeIn = _compositor.CreateScalarKeyFrameAnimation();
            fadeIn.Target = "Opacity";
            fadeIn.Duration = TimeSpan.FromMilliseconds(_animationDuration);
            fadeIn.InsertKeyFrame(0, 0);
            fadeIn.InsertKeyFrame(1, 1);
            fadeIn.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;
            fadeIn.DelayTime = TimeSpan.FromMilliseconds(_animationDuration * 0.125 * args.ItemIndex);

            var fadeOut = _compositor.CreateScalarKeyFrameAnimation();
            fadeOut.Target = "Opacity";
            fadeOut.Duration = TimeSpan.FromMilliseconds(_animationDuration);
            fadeOut.InsertKeyFrame(1, 0);

            var scaleIn = _compositor.CreateVector3KeyFrameAnimation();
            scaleIn.Target = "Scale";
            scaleIn.Duration = TimeSpan.FromMilliseconds(_animationDuration);
            scaleIn.InsertKeyFrame(0f, new Vector3(1.2f, 1.2f, 1.2f));
            scaleIn.InsertKeyFrame(1f, new Vector3(1f, 1f, 1f));
            scaleIn.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;
            scaleIn.DelayTime = TimeSpan.FromMilliseconds(_animationDuration * 0.125 * args.ItemIndex);

            // animations set to run at the same time are grouped
            var animationFadeInGroup = _compositor.CreateAnimationGroup();
            animationFadeInGroup.Add(fadeIn);
            animationFadeInGroup.Add(scaleIn);

            // Set up show and hide animations for this item container before the element is added to the tree.
            // These fire when items are added/removed from the visual tree, including when you set Visibilty
            ElementCompositionPreview.SetImplicitShowAnimation(args.ItemContainer, animationFadeInGroup);
            ElementCompositionPreview.SetImplicitHideAnimation(args.ItemContainer, fadeOut);
        }

        private void GalleryItem_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            ScaleImage(frameworkElement, 1.1f);
        }

        private void GalleryItem_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            ScaleImage(frameworkElement, 1f);
        }

        private void ScaleImage(FrameworkElement frameworkElement, float scaleAmout)
        {
            var element = ElementCompositionPreview.GetElementVisual(frameworkElement);

            var scaleAnimation = _compositor.CreateVector3KeyFrameAnimation();
            scaleAnimation.Duration = TimeSpan.FromMilliseconds(500);
            scaleAnimation.InsertKeyFrame(1f, new Vector3(scaleAmout, scaleAmout, scaleAmout));

            element.CenterPoint = new Vector3((float)frameworkElement.ActualHeight / 2,
                (float)frameworkElement.ActualWidth / 2, 275f / 2);
            element.StartAnimation("Scale", scaleAnimation);

            if (scaleAmout > 1)
            {
                var shadow = _compositor.CreateDropShadow();
                shadow.Offset = new Vector3(15, 15, -10);
                shadow.BlurRadius = 5;
                shadow.Color = Colors.DarkGray;

                var sprite = _compositor.CreateSpriteVisual();
                sprite.Size = new Vector2((float)frameworkElement.ActualWidth - 20,
                    (float)frameworkElement.ActualHeight - 20);
                sprite.Shadow = shadow;

                ElementCompositionPreview.SetElementChildVisual((UIElement)frameworkElement.FindName("Shadow"), sprite);
            }
            else
            {
                ElementCompositionPreview.SetElementChildVisual((UIElement)frameworkElement.FindName("Shadow"), null);
            }
        }

        #endregion

        /// <summary>
        /// 图片右击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
            MenuFlyout flyout = (MenuFlyout)MenuFlyout.GetAttachedFlyout((FrameworkElement)sender);

            #region 将15天内的删除按钮屏蔽

            try
            {
                var imgId = ((e.OriginalSource as Windows.UI.Xaml.Controls.Image)?.DataContext as UwpWallpaper.ViewModels.Photo)?.ImageId;
                if (imgId != null && int.Parse(imgId) > int.Parse(DateHelper.GetDateStr(-1 * 15)))
                {
                    this.DelFlyout.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.DelFlyout.Visibility = Visibility.Visible;
                }
            }
            catch (Exception)
            {

            }
            #endregion

            flyout.ShowAt(sender as UIElement, e.GetPosition(sender as UIElement));
        }

        private async void Button_UsePhotoOpen(object sender, RoutedEventArgs e)
        {
            var flyoutItem = sender as Windows.UI.Xaml.Controls.MenuFlyoutItem;
            string imgUri = (flyoutItem.DataContext as Photo).ImageUri;
            var retflg = await OpenSysPhotoView(imgUri);
            if (!retflg)//如果调用失败
            {
            }
        }

        private async Task<bool> OpenSysPhotoView(string imguri)
        {
            var imageFile = await Windows.Storage.StorageFile.GetFileFromPathAsync(imguri);
            return await Windows.System.Launcher.LaunchFileAsync(imageFile);
        }

        private async void Button_DelPicture(object sender, RoutedEventArgs e)
        {
            var flyoutItem = sender as Windows.UI.Xaml.Controls.MenuFlyoutItem;

            string imgId = (flyoutItem.DataContext as Photo).ImageId;
            string imgUri = (flyoutItem.DataContext as Photo).ImageUri;
            var imageFile = await Windows.Storage.StorageFile.GetFileFromPathAsync(imgUri);

            //dialog window
            var dialog = new ContentDialog()
            {
                Title = LangResource.GetString("Tips"), //"消息提示",
                Content = LangResource.GetString("DelTipsDialog"), //"仅可删除非当天数据，且删除后不可找回，确认要删除吗?",
                PrimaryButtonText = LangResource.GetString("Confirm"), //"确定",
                SecondaryButtonText = LangResource.GetString("Cancel"), //"取消",
                FullSizeDesired = false,
            };
            dialog.PrimaryButtonClick += async (_s, _e) =>
            {
                try
                {
                    SqliteManager.SqlQuery.DeleteImageByDayId(imgId);
                    await imageFile.DeleteAsync();
                    //_showWallpapers.Remove(_showWallpapers.First(q => q.ImageId == imgId));
                    var model = ViewModel.GalleryList.FirstOrDefault(q => q.ImageId == imgId);
                    ViewModel.GalleryList.Remove(model);
                }
                catch (Exception ex)
                {
                    await new ContentDialog
                    {
                        Title = "Error",
                        Content = ex.Message,
                        CloseButtonText = "Ok"
                    }.ShowAsync();
                }
            };
            await dialog.ShowAsync();
        }

        /// <summary>
        /// 设为桌面背景
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_SetScreenBackground(object sender, RoutedEventArgs e)
        {
            var flyoutItem = sender as Windows.UI.Xaml.Controls.MenuFlyoutItem;
            string imgUri = (flyoutItem.DataContext as Photo).ImageUri;

            if (!Windows.System.UserProfile.UserProfilePersonalizationSettings.IsSupported())
            {
                ContentDialog noSaveDialog = new ContentDialog
                {
                    Title = "Oops……",
                    Content = LangResource.GetString("NoLimitedToSetDeskPic"),
                    CloseButtonText = "Ok"
                };

                ContentDialogResult result = await noSaveDialog.ShowAsync();
                return;
            }

            //获取文件
            Windows.Storage.StorageFile file = await Windows.Storage.StorageFile.GetFileFromPathAsync(imgUri);
            //设置背景
            Windows.System.UserProfile.UserProfilePersonalizationSettings setting = Windows.System.UserProfile.UserProfilePersonalizationSettings.Current;
            bool b = await setting.TrySetWallpaperImageAsync(file);
            if (b)
            {
                ContentDialog noSaveDialog = new ContentDialog
                {
                    Title = "OK",
                    Content = LangResource.GetString("SetSuccess"),
                    CloseButtonText = "Ok"
                };

                ContentDialogResult result = await noSaveDialog.ShowAsync();
                return;
            }
            else
            {
                ContentDialog noSaveDialog = new ContentDialog
                {
                    Title = "OK",
                    Content = LangResource.GetString("SetFailed"),
                    CloseButtonText = "Ok"
                };

                ContentDialogResult result = await noSaveDialog.ShowAsync();
                return;
            }
        }

        private async void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var imageid = ((sender as Windows.UI.Xaml.Controls.Image)?.DataContext as Photo)?.ImageId;

            var imgDialog = new ImgContentDialog
            {
                ImgId = imageid,
                Width = Window.Current.Bounds.Width * 0.8,
                Height = Window.Current.Bounds.Height * 0.8
            };

            await imgDialog.ShowAsync();
        }

        private async void Button_4kDownload(object sender, RoutedEventArgs e)
        {
            try
            {
                var flyoutItem = sender as Windows.UI.Xaml.Controls.MenuFlyoutItem;
                string imgUri = (flyoutItem.DataContext as Photo).ImageUri;

                var imgId = Path.GetFileNameWithoutExtension(imgUri);

                var wallpaperInfo = await HttpManager.GetWallpaperInfoAsync(imgId);
                var downloadUrl = wallpaperInfo.PicUrl.GetFullDownloadPicUrl();
                var UHDUrl = downloadUrl.Replace("1920x1080", "UHD");

                Windows.System.Launcher.LaunchUriAsync(new Uri(UHDUrl));
            }
            catch (Exception ex)
            {
                ULogger.Current.LogError("Button_4kDownload", ex);
            }
        }
    }
}
