using System;
using System.Linq;

namespace Luis
{
    // Extends the partial IPNBarMenuCognitiveModel class with methods and properties that simplify accessing entities in the luis results
    public partial class IPNBarMenuCognitiveModel
    {
        public DateTime? MenuDateAsDateTime
        {
            get
            {
                string menuDate = Entities?.MenuDate?.FirstOrDefault()?.FirstOrDefault();

                if (menuDate == null)
                    return null;
                else if (menuDate.ToUpper() == ipnbarbot.Application.Constants.DaysOfWeek.Today.ToUpper())
                    return ipnbarbot.Application.Helpers.DateHelpers.GetDateForDailyMenuByDayOfWeek(ipnbarbot.Application.Constants.DaysOfWeek.Today);
                else if (menuDate.ToUpper() == ipnbarbot.Application.Constants.DaysOfWeek.Tomorrow.ToUpper())
                    return ipnbarbot.Application.Helpers.DateHelpers.GetDateForDailyMenuByDayOfWeek(ipnbarbot.Application.Constants.DaysOfWeek.Tomorrow);
                else if (menuDate.ToUpper() == ipnbarbot.Application.Constants.DaysOfWeek.Monday.ToUpper())
                    return ipnbarbot.Application.Helpers.DateHelpers.GetDateForDailyMenuByDayOfWeek(ipnbarbot.Application.Constants.DaysOfWeek.Monday);
                else if (menuDate.ToUpper() == ipnbarbot.Application.Constants.DaysOfWeek.Tuesday.ToUpper())
                    return ipnbarbot.Application.Helpers.DateHelpers.GetDateForDailyMenuByDayOfWeek(ipnbarbot.Application.Constants.DaysOfWeek.Tuesday);
                else if (menuDate.ToUpper() == ipnbarbot.Application.Constants.DaysOfWeek.Wednesday.ToUpper())
                    return ipnbarbot.Application.Helpers.DateHelpers.GetDateForDailyMenuByDayOfWeek(ipnbarbot.Application.Constants.DaysOfWeek.Wednesday);
                else if (menuDate.ToUpper() == ipnbarbot.Application.Constants.DaysOfWeek.Thursday.ToUpper())
                    return ipnbarbot.Application.Helpers.DateHelpers.GetDateForDailyMenuByDayOfWeek(ipnbarbot.Application.Constants.DaysOfWeek.Thursday);
                else if (menuDate.ToUpper() == ipnbarbot.Application.Constants.DaysOfWeek.Friday.ToUpper())
                    return ipnbarbot.Application.Helpers.DateHelpers.GetDateForDailyMenuByDayOfWeek(ipnbarbot.Application.Constants.DaysOfWeek.Friday);
                else
                    return null;
            }
        }
    }
}