using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UwpWallpaper.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Debug : Page
    {
        public Debug()
        {
            this.InitializeComponent();
        }

        private void ConfigBtn_Click(object sender, RoutedEventArgs e)
        {
            var key = this.LocalSettingText.Text;
            if (!string.IsNullOrEmpty(key))
            {
                var LocalSettings = ApplicationData.Current.LocalSettings;
                if (LocalSettings.Values.ContainsKey(key))
                {
                    LocalSettings.Values[key] = this.LocalSettingConfigText.Text;
                } 
            }
        }

        private void ShowConfig_Click(object sender, RoutedEventArgs e)
        {
            var key = this.LocalSettingText.Text;
            if(!string.IsNullOrEmpty(key))
            {
                var LocalSettings = ApplicationData.Current.LocalSettings;
                if (LocalSettings.Values.ContainsKey(key))
                {
                    this.Config.Text = (string)LocalSettings.Values[key];
                }
            }
        }
    }
}
