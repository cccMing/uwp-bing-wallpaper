using CommonUtil;
using Microsoft.QueryStringDotNET;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;

namespace NotificationBackgroundTasks
{
    public sealed class NotifyTask: IBackgroundTask
    {
        BackgroundTaskDeferral _deferral = null; // Note: defined at class scope so we can mark it complete inside the OnCancel() callback if we choose to support cancellation
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();
            //
            // Associate a cancellation handler with the background task.
            //
            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

            var details = taskInstance.TriggerDetails as ToastNotificationActionTriggerDetail;

            if (details != null)
            {
                string arguments = details.Argument;
                // Perform tasks
                QueryString args = QueryString.Parse(arguments);
                try
                {
                    string action = args["action"] ?? "";
                    switch (action.ToLower())
                    {
                        case "setdesktopbackground":
                            var imgId = args["ImgId"] ?? "";
                            if (!string.IsNullOrEmpty(imgId))
                            {
                                await SetDesktopWallpaper(imgId);
                            }
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ULogger.Current.LogError("NotifyTask Run", ex);
                }
            }

            _deferral.Complete();
        }

        volatile bool _cancelRequested = false;
        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            //
            // Indicate that the background task is canceled.
            //

            _cancelRequested = true;

            ULogger.Current.Log($"{nameof(NotifyTask)} OnCanceled clicked");

            //Debug.WriteLine("Background " + sender.Task.Name + " Cancel Requested...");
        }

        private async Task SetDesktopWallpaper(string imgId)
        {
            //获取文件
            Windows.Storage.StorageFile file = await Windows.Storage.StorageFile.GetFileFromPathAsync(Path.Combine(UwpBing.PicFolderPath, $"{imgId}.jpg"));
            //设置背景
            Windows.System.UserProfile.UserProfilePersonalizationSettings setting = Windows.System.UserProfile.UserProfilePersonalizationSettings.Current;
            bool b = await setting.TrySetWallpaperImageAsync(file);
            ULogger.Current.Log($"NotifyTask SetDesktopWallpaper:{b}");
        }
    }
}
