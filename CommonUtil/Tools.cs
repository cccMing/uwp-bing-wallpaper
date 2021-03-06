using System;

namespace CommonUtil
{
    /// <summary>
    /// 公共工具
    /// </summary>
    public class Tools
    {
        public static double ToDouble(string dec)
        {
            if (string.IsNullOrEmpty(dec))
            {
                return 0;
            }
            try
            {
                double d = Convert.ToDouble(dec);
                return d;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
