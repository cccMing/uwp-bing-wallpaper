using CommonUtil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace UwpWallpaper.Util
{
    public class StorageHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Mb</returns>
        public async Task<double> GetAppDataStorageSize()
        {
            try
            {
                var mainpath = UwpBing.Folder.Path;
                folderList.Add(mainpath);//当前目录
                await GetFoldersPath(mainpath);//当前目录下文件夹

                var sizes = folderList.Select(async path => await GetFolderSize(path));

                var allsizes = await Task.WhenAll(sizes);

                return allsizes.Sum() / (1024 * 1024.0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return -1;
            }
        }

        IList<string> folderList = new List<string>();
        private async Task GetFoldersPath(string parentFolder)
        {
            var folderQuerys = (await StorageFolder.GetFolderFromPathAsync(parentFolder)).CreateFolderQuery();
            var folders = await folderQuerys.GetFoldersAsync();
            if (folders.Count == 0)
            {
                return;
            }

            foreach (var path in folders.Select(q => q.Path))
            {
                folderList.Add(path);
                await GetFoldersPath(path);
            }
        }

        /// <summary>
        /// 计算LocalState文件夹中文件的大小，不包括子文件夹
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        private async Task<long> GetFolderSize(string folderPath)
        {

            // Query all files in the folder. Make sure to add the CommonFileQuery
            // So that it goes through all sub-folders as well
            //var folders = ApplicationData.Current.LocalFolder.CreateFileQuery();
            var folders = (await StorageFolder.GetFolderFromPathAsync(folderPath)).CreateFileQuery();

            // Await the query, then for each file create a new Task which gets the size
            var fileSizeTasks = (await folders.GetFilesAsync()).Select(async file => (await file.GetBasicPropertiesAsync()).Size);

            // Wait for all of these tasks to complete. WhenAll thankfully returns each result
            // as a whole list
            var sizes = await Task.WhenAll(fileSizeTasks);

            // Sum all of them up. You have to convert it to a long because Sum does not accept ulong.
            var size = sizes.Sum(l => (long)l);

            return size;
        }

        /// <summary>
        /// 删除localstate下面的所有数据,目前只删除图片
        /// </summary>
        /// <returns></returns>
        public async Task DeleteAppCache()
        {
            //var statePath = UwpBing.Folder.Path;
            await (await StorageFolder.GetFolderFromPathAsync(UwpBing.CurrentStorgePath)).DeleteAsync();
            //await (await StorageFile.GetFileFromPathAsync(statePath)).DeleteAsync();
        }
    }
}
