using System;

namespace DBO.Common.Helpers
{
    public static class DateTimeHelper
    {
        public static DateTime GetCurrentDateTimeByTimeZone(string timeZoneId)
        {
            try
            {
                if (string.IsNullOrEmpty(timeZoneId)) throw new InvalidTimeZoneException();

                DateTime nowDateTimeUtc = DateTime.Now.ToUniversalTime();
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                return TimeZoneInfo.ConvertTimeFromUtc(nowDateTimeUtc, timeZone);
            }
            catch (Exception ex)
            {
                throw new InvalidTimeZoneException("The time zone id parameter is incorrect, or empty.");
            }

        }

        public static int GetDaysCountFromDateToDate(DateTime now, DateTime processedNotificationTime)
        {
            return (now - processedNotificationTime).Days;
        }
    }
}
