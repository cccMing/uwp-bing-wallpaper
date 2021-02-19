using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using CommonUtil;

namespace UwpWallpaper.BingManager
{
    public class FileManager
    {
        public async static Task<string> GetStorePath()
        {
            StorageFolder saveFolder = await UwpBing.Folder.CreateBingdataFolderIfNotExist();
            return saveFolder.Path;
        }

        public async static Task<List<string>> GetAllFileInFolder(string suffix)
        {
            var saveFolder = await UwpBing.Folder.CreateBingdataFolderIfNotExist();
            List<string> filenames = new List<string>();
            foreach(string i in Directory.GetFiles(saveFolder.Path))
            {
                if (i.EndsWith(suffix))
                {
                    filenames.Add(i);
                }
            }
            return filenames;
        }
    }
}
