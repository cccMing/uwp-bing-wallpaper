using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace UwpWallpaper.Util
{
    public class MessageHelper
    {
        /// <summary>
        /// 提示
        /// </summary>
        public async Task ShowDialogAsync(string content)
        {
            await ShowDialogAsync("", content);
        }
        
        public async Task ShowDialogAsync(string title, Exception ex, string ok = "Ok")
        {
            await ShowDialogAsync(title, ex.Message, ok);
        }
        

        public async Task<bool> ShowDialogAsync(string title, string content, string ok = "Ok", string cancel = null)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = content,
                PrimaryButtonText = ok
            };
            if (cancel != null)
            {
                dialog.SecondaryButtonText = cancel;
            }
            var result = await dialog.ShowAsync();
            return result == ContentDialogResult.Primary;
        }
    }
}
