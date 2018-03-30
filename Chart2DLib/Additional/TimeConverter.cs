using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Readearth.Chart2D.Addtional
{
    public static class TimeConverter
    {
        /// <summary>  
        /// 时间戳转为C#格式时间（默认起始时间为UTC1970.01.01）
        /// </summary>  
        /// <param name="timeStamp">Unix时间戳格式(单位为秒)</param>  
        /// <returns>C#格式时间</returns>  
        public static DateTime GetTime(long timeStamp)
        {
            //DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            DateTime dtStart = new DateTime(1970, 1, 1);
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }
        /// <summary>  
        /// 时间戳转为C#格式时间  
        /// </summary>  
        /// <param name="timeStamp">Unix时间戳格式(单位为秒)</param>  
        /// <param name="startTime">起始时间</param> 
        /// <returns>C#格式时间</returns>  
        public static DateTime GetTime(long TimeStamp, DateTime startTime)
        {
            long lTime = long.Parse(TimeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return startTime.Add(toNow);
            //TimeSpan刻度为100毫微秒 = 10^-7 秒。
            //1 毫秒 = 10^-3 秒，
            //1 微秒 = 10^-6 秒，
            //1 毫微秒 = 10^-9 秒，
        }

        /// <summary>  
        /// DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time"> DateTime时间格式</param>  
        /// <returns>Unix时间戳格式</returns>  
        public static long ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (long)(time - startTime).TotalSeconds;
        }
        /// <summary>  
        /// DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time"> DateTime时间格式</param>  
        /// <returns>Unix时间戳格式</returns>  
        public static long ConvertDateTimeInt(System.DateTime time, DateTime startTime)
        {
            return (long)(time - startTime).TotalSeconds;
        }
    }
}
