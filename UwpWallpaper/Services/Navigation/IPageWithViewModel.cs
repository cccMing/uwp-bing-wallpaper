using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UwpWallpaper.Services.Navigation
{
    /// <summary>
    /// 主要是使页面cs实现这个接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPageWithViewModel<T>
        where T : class
    {
        T ViewModel { get; set; }

        //void UpdateBindings();
    }
}
