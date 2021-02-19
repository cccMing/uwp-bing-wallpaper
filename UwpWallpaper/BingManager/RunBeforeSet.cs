using CommonUtil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UwpWallpaper.Util;
using Windows.Storage;

namespace UwpWallpaper.BingManager
{
    public class RunBeforeSet
    {

        public static void Setting()
        {
            //CheckFolderCreatedAsync().Wait(); 弃用，因为在release模式下createfolder这句话不能执行通过，
            /*
             * 项目右键-属性-生成-release模式下取消勾选优化代码
             * 勾选使用.net本机工具链编译
             * */

            AppSettings.Current.LastOpenAppDate = "1";//更新最后一次打开时间，实际赋值是在属性set里面
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static async Task CheckFolderCreatedAsync()
        {
            var path = Path.Combine(UwpBing.Folder.Path, ConstantObj.BINGFOLDER);
            if (!Directory.Exists(path))
            {
                await UwpBing.Folder.CreateBingdataFolderIfNotExist();
            }
        }
    }
}
