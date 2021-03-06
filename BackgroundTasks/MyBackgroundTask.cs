using CommonUtil;
using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using SqliteManager;
using SqliteManager.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Resources;
using Windows.UI.Notifications;

namespace BackgroundTasks
{
    /// <summary>
    /// 后台任务，主要进行下载这几天缺失的壁纸信息以及windows通知
    /// </summary>
    public sealed class MyBackgroundTask : IBackgroundTask
    {
        BackgroundTaskDeferral _deferral = null; // Note: defined at class scope so we can mark it complete inside the OnCancel() callback if we choose to support cancellation

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();

            try
            {
                WallpaperInfoPo wallpaperInfo = await DownloadTodayWallpaperIfNotExist();
                ToastNotification(wallpaperInfo);
                await SetDestopWallpaperAsync();
                UpdateTile();
                await DownloadPrevMissingWallpaperInfoAsync();
                await DownloadPrevMissingWallpaperAsync();
            }
            catch (Exception ex)
            {
                //异常不能 记录到日志，用nlog后已经解决，可能是配置路径问题
                ULogger.Current.LogError("MyBackgroundTask Run", ex);
            }

            _deferral.Complete();
        }

        //写为public会导致:严重性	代码	说明	项目	文件	行	禁止显示状态
        //错误 Method 'BackgroundTasks.MyBackgroundTask.UpdateTileAsync()' has a parameter of type 'System.Threading.Tasks.Task' in its signature.Although this type is not a valid Windows Runtime type, it implements interfaces that are valid Windows Runtime types.Consider changing the method signature to use one of the following types instead: ''.	BackgroundTasks C:\Users\John\source\repos\UwpWallpaper\BackgroundTasks\MyBackgroundTask.cs 38	
        //磁贴图片轮播
        private void UpdateTile()
        {
            var random = new Random();
            string filepath;
            var selectedPicName = "111.abc";
            int trytimes = 0;

            while (!UwpBing.IsPicExist(selectedPicName, out filepath))
            {
                if (trytimes++ > 6)
                {
                    return;
                }
                selectedPicName = DateHelper.GetDateStr((-1) * random.Next(30));
            }

            TileContent content = new TileContent
            {
                Visual = new TileVisual
                {
                    Branding = TileBranding.Name,
                    DisplayName = "BingWallpaper",
                    TileMedium = new TileBinding
                    {
                        Content = new TileBindingContentAdaptive
                        {
                            BackgroundImage = new TileBackgroundImage
                            {
                                HintCrop = TileBackgroundImageCrop.Default,
                                Source = filepath
                            }
                        }
                    },
                    TileWide = new TileBinding
                    {
                        Content = new TileBindingContentAdaptive
                        {
                            BackgroundImage = new TileBackgroundImage
                            {
                                HintCrop = TileBackgroundImageCrop.Default,
                                Source = filepath
                            }
                        }
                    }
                }
            };

            //创建一个磁贴类，将xml数据填充到磁贴中
            var tileNotification = new TileNotification(content.GetXml());
            //tileNotification.ExpirationTime = DateTimeOffset.UtcNow.AddSeconds(10);
            //向磁贴更新
            //TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
        }

        /// <summary>
        /// windows通知
        /// </summary>
        /// <param name="wallpaperInfo"></param>
        private void ToastNotification(WallpaperInfoPo wallpaperInfo)
        {
            if (wallpaperInfo == null)
            {
                return;
            }

            var isEnableToast = AppSettings.Current.IsEnableToast;
            var lastOpenDate = AppSettings.Current.LastOpenAppDate;
            var toastDate = AppSettings.Current.ToastDate;

            ULogger.Current.Log($"{nameof(MyBackgroundTask)} ToastNotification [{isEnableToast}] [{lastOpenDate}] [{toastDate}]");

            string today = DateHelper.CurrentDateStr;
            if (!isEnableToast)
            {
                return;
            }
            if (lastOpenDate?.ToString() == today || toastDate?.ToString() == today)
            {
                return;
            }
            if (new Random().Next(5) != 1)
            {
                return;
            }

            AppSettings.Current.ToastDate = today;

            var toastContent = new ToastContent
            {
                Visual = new ToastVisual
                {
                    BindingGeneric = new ToastBindingGeneric
                    {
                        Children =
                        {
                            new AdaptiveText
                            {
                                Text = new ResourceLoader().GetString("TodayImageUpdated") //"今日美图更新啦♪(^∇^*)"
                            },
                            new AdaptiveText
                            {
                                Text = wallpaperInfo?.CopyRight
                            },
                            new AdaptiveImage
                            {
                                Source = wallpaperInfo?.PicUrl.GetFullDownloadPicUrl()
                            }
                        }
                    }
                },
                Actions = new ToastActionsCustom
                {
                    Buttons = {//"action=setDesktopBackground&ImgId=123"
                        new ToastButton(
                            new ResourceLoader().GetString("SetImgAsBackground"),
                            new QueryString{
                                {"action","SetDesktopBackground" },
                                {"ImgId", wallpaperInfo.WallpaperNo.ToString()}
                            }.ToString())
                        {
                            ActivationType=ToastActivationType.Background
                        },
                        new ToastButtonDismiss()
                    }
                }
            };

            // Create the toast notification
            var toastNotif = new ToastNotification(toastContent.GetXml());

            // 清空在通知中心的时间，就是一天消息没被点过就在通知中心消失

            var todaySpan = DateTime.Parse(DateTime.Now.AddDays(1).ToShortDateString()) - DateTime.Now;
            double todayRemainingHours = Math.Ceiling(todaySpan.TotalHours);//计算今天剩余时间，为了使通知明天就消失

            toastNotif.ExpirationTime = DateTime.Now.AddHours(todayRemainingHours);

            // And send the notification
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
        }

        private async Task<WallpaperInfoPo> DownloadTodayWallpaperIfNotExist()
        {
            //以下和主程序中第一次下载一样，有时间下载放一个程序集给两边调用

            string todayno = DateHelper.CurrentDateStr;
            //先去数据库中查今天的数据是否已经存入
            var wallinfo = SqlQuery.GetDaysWallpaperInfo(todayno);
            if (wallinfo != null)
                return wallinfo;

            WallpaperInfoPo allinfo;
            try
            {
                //没有的话网络请求
                string archive = await DownloadHelper.GetTodayWallpaperAsync();
#if FALSE
                string coverstory = await DownloadHelper.GetCoverstoryAsync();
#endif
                allinfo = SqlQuery.SaveBingWallpaperInfo(archive, null);
            }
            catch (Exception)
            {
                return null;
            }

            if (UwpBing.IsPicExist(todayno, out var _))
            {
                return allinfo;
            }

            UwpBing ub = new UwpBing();
            bool result = await ub.SavePicByBuffer(todayno, allinfo.PicUrl);

            return result ? allinfo : null;
        }

        private async Task<bool> SetDestopWallpaperAsync()
        {
            if (AppSettings.Current.WallpaperSetDate == DateHelper.CurrentDateStr)
            {
                return true;
            }
            if (!AppSettings.Current.IsAutoSetDestopWallpaper)
            {
                return true;
            }

            AppSettings.Current.WallpaperSetDate = DateHelper.CurrentDateStr;
            return await WallpaperSetting.SetWallpaperAsync(DateHelper.CurrentDateStr, BackgroundEnum.Destop);
        }

        /// <summary>
        /// 下载之前缺失的图片数据库信息 最多14天
        /// </summary>
        /// <returns></returns>
        private async Task<int> DownloadPrevMissingWallpaperInfoAsync()
        {
            IList<string> dayMissings = getMissingDays();
            if (dayMissings.Count == 0)
            {
                return 0;
            }

            try
            {
                //取第7天前（包含第7天）的前8张数据
                string urla = "http://www.bing.com/HPImageArchive.aspx?format=js&idx=7&n=8";

                //取之前6天（昨天开始）的数据
                string urlb = "http://www.bing.com/HPImageArchive.aspx?format=js&idx=1&n=6";

                //下载数据
                string archiveA = await DownloadHelper.HttpGetStringAsync(urla);
                string archiveB = await DownloadHelper.HttpGetStringAsync(urlb);

                Dictionary<string, string> coverStoryDic = new Dictionary<string, string>();
                foreach (string day in dayMissings)
                {
                    string coverUrl = $"http://cn.bing.com/cnhp/coverstory?d={day}";
                    coverStoryDic.Add(day, await DownloadHelper.HttpGetStringAsync(coverUrl));
                }

                SqlQuery.SavePrevWallpaperInfo(new List<string> { archiveA, archiveB }, coverStoryDic);
            }
            catch (Exception ex)
            {
                ULogger.Current.LogError("MyBackgroundTask DownLoadPrevMissiingWallpaperInfo", ex);
                return 0;
            }

            return 1;
        }

        private async Task DownloadPrevMissingWallpaperAsync()
        {
            for (var i = 0; i <= 14; i++)
            {
                var day = DateHelper.GetDateStr(-1 * i);

                if (UwpBing.IsPicExist(day, out var _))
                {
                    continue;
                }

                //图片不存在的情况下
                #region Picture Save
                WallpaperInfoPo winfo = SqlQuery.GetDaysWallpaperInfo(day);
                if (!string.IsNullOrEmpty(winfo?.PicUrl))
                {
                    UwpBing ub = new UwpBing();
                    bool result = await ub.SavePicByBuffer(day, winfo.PicUrl);
                }
#endregion
            }
        }

        /// <summary>
        /// 获取缺失时间集合
        /// </summary>
        /// <returns></returns>
        private IList<string> getMissingDays()
        {
            IList<string> dayMissings = new List<string>();
            for (var i = 1; i <= 14; i++)
            {
                var day = DateHelper.GetDateStr(-1 * i);
                //先去数据库中查这天的数据是否已经存入
                var wallinfo = SqlQuery.GetDaysWallpaperInfo(day);
                if (wallinfo == null)
                {
                    dayMissings.Add(day);
                }
            }
            return dayMissings;
        }
    }
}
