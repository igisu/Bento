using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace com.emukikaku.CtiServiceAlphaCTI
{
    static class Utility
    {
        #region Cast
        public static int toInt(object value)
        {
            if (value == null) return 0;
            string stringValue = value.ToString();
            if (stringValue.Equals(string.Empty)) return 0;
            int castedValue = 0;
            if (!int.TryParse(stringValue, out castedValue)) return 0;
            return castedValue;
        }

        public static bool toBool(object value)
        {
            if (value == null) return false;
            string stringValue = value.ToString();
            if (stringValue.Equals(string.Empty)) return false;
            bool castedValue = false;
            if (!bool.TryParse(stringValue, out castedValue)) return false;
            return castedValue;
        }
        #endregion

        #region LogWriter
        public static void writeLog(string tag,  string message)
        {
            try
            {
                Encoding enc = Encoding.GetEncoding("UTF-8");
                string path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "CtiService.log");
                using (StreamWriter sw = new StreamWriter(path, true, enc))
                {
                    sw.WriteLine(string.Format("{0}\t{1}\t{2}", DateTime.Now.ToString(), tag, message));
                }
            }
            catch (Exception)
            {

            }
        }
        #endregion
    }
}
