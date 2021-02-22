using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtil
{
    public class DateHelper
    {
        /// <summary>
        /// 当前时间yyyyMMdd
        /// </summary>
        public static string CurrentDateStr 
            => GetDateStr(0);

        /// <summary>
        /// 获取相应时间yyyyMMdd
        /// </summary>
        /// <param name="dayOffset"></param>
        /// <returns></returns>
        public static string GetDateStr(int dayOffset = 0) 
            => DateTime.Now.AddDays(dayOffset).ToString("yyyyMMdd");
    }
}
