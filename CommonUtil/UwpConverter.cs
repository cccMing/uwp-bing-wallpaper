using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtil
{
    public class UwpConverter
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
