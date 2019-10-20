using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ipnbarbot.Application
{
    public interface IMealSchedulesApp : IDisposable
    {
        Task<Models.MealSchedule> GetForDate(DateTime date);

        Task<Models.MealSchedule> GetForToday();

        Task<IEnumerable<Models.MealSchedule>> GetForWeek(DateTime weekDate);

        Task AddAsync(DateTime date, string soupName, string mainDishName, string veganDishName);
        
        Task<int> RemoveForDateRange(DateTime fromDate, DateTime toDate);
    }
}