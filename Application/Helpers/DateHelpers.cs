using System;
using ipnbarbot.Application.Constants;

namespace ipnbarbot.Application.Helpers
{
    public static class DateHelpers
    {
        public static DateTime Today
        {
            get
            {
                DateTime result = DateTime.Today;

                if (result.DayOfWeek == DayOfWeek.Saturday || result.DayOfWeek == DayOfWeek.Sunday)
                {
                    int daysToAdd = ((int)DayOfWeek.Monday - (int)result.DayOfWeek + 7) % 7;
                    result = result.AddDays(daysToAdd);
                }

                return result;
            }
        }

        public static DateTime GetDateForDailyMenuByDayOfWeek(string dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DaysOfWeek.Monday:
                    return Today.AddDays(((int)DayOfWeek.Monday - (int)Today.DayOfWeek + 7) % 7);
                case DaysOfWeek.Tuesday:
                    return Today.AddDays(((int)DayOfWeek.Tuesday - (int)Today.DayOfWeek + 7) % 7);
                case DaysOfWeek.Wednesday:
                    return Today.AddDays(((int)DayOfWeek.Wednesday - (int)Today.DayOfWeek + 7) % 7);
                case DaysOfWeek.Thursday:
                    return Today.AddDays(((int)DayOfWeek.Thursday - (int)Today.DayOfWeek + 7) % 7);
                case DaysOfWeek.Friday:
                    return Today.AddDays(((int)DayOfWeek.Friday - (int)Today.DayOfWeek + 7) % 7);
                case DaysOfWeek.Today:
                    return DateTime.Today;
                case DaysOfWeek.Tomorrow:
                    return DateTime.Today.AddDays(1);
                default:
                    throw new Exception("Invalid day of the week");
            }
        }

        public static string GetDayOfWeekForDailyMenuByDate(DateTime date)
        {
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return DaysOfWeek.Monday;
                case DayOfWeek.Tuesday:
                    return DaysOfWeek.Tuesday;
                case DayOfWeek.Wednesday:
                    return DaysOfWeek.Wednesday;
                case DayOfWeek.Thursday:
                    return DaysOfWeek.Thursday;
                case DayOfWeek.Friday:
                    return DaysOfWeek.Friday;
                default:
                    throw new Exception("Invalid date");
            }
        }
    }
}