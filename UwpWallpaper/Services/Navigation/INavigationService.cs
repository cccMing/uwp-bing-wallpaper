using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UwpWallpaper.Services.Navigation
{
    public interface INavigationService
    {
        event EventHandler<bool> IsNavigatingChanged;

        event EventHandler Navigated;

        bool CanGoBack { get; }

        bool IsNavigating { get; }

        Task NavigateToTodayImageAsync();

        Task NavigateToSettingsAsync();

        Task NavigateToGalleryAsync();

        Task NavigateToTestAsync();
    }
}
