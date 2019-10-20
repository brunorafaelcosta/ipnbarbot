using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ipnbarbot.Application
{
    public class MealSchedulesApp : IMealSchedulesApp
    {
        #region IDisposable
        public void Dispose()
        {
        }
        #endregion

        private readonly IConfiguration _configuration;
        private readonly Data.ApplicationDbContext _dbContext;

        public MealSchedulesApp(IConfiguration configuration, Data.ApplicationDbContext dbContext)
        {
            this._configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this._dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<Models.MealSchedule> GetForDate(DateTime date)
        {
            Models.MealSchedule mealSchedule = await this._dbContext.MealSchedules.AsNoTracking()
                .Include(e => e.SoupMeal)
                .Include(e => e.MainDishMeal)
                .Include(e => e.VeganDishMeal)
                .FirstOrDefaultAsync(ms => ms.Date.Date == date.Date);

            return mealSchedule;
        }

        public async Task<Models.MealSchedule> GetForToday()
        {
            return await GetForDate(DateTime.Today);
        }

        public async Task<IEnumerable<Models.MealSchedule>> GetForWeek(DateTime weekDate)
        {
            DateTime weekStartDate = weekDate.AddDays(-(int)weekDate.DayOfWeek).AddDays(1).Date;
            DateTime weekEndDay = weekStartDate.AddDays(4).Date;

            var weekMealSchedules = await this._dbContext.MealSchedules.AsNoTracking()
                .Where(ms => ms.Date.Date >= weekStartDate.Date && ms.Date.Date <= weekEndDay.Date)
                .Include(e => e.SoupMeal)
                .Include(e => e.MainDishMeal)
                .Include(e => e.VeganDishMeal)
                .OrderBy(o => o.Date)
                .ToListAsync();

            return weekMealSchedules;
        }

        public async Task AddAsync(DateTime date, string soupName, string mainDishName, string veganDishName)
        {
            if (string.IsNullOrEmpty(soupName))
                throw new ArgumentNullException(nameof(soupName));
            else if (string.IsNullOrEmpty(mainDishName))
                throw new ArgumentNullException(nameof(mainDishName));
            else if (string.IsNullOrEmpty(veganDishName))
                throw new ArgumentNullException(nameof(veganDishName));
            else if (this._dbContext.MealSchedules.Any(m => m.Date.Year == date.Year && m.Date.Month == date.Month && m.Date.Day == date.Day))
                throw new Exceptions.ApplicationHandledException($"Menu already set for {date.ToShortDateString()}");
            
            Models.Meal soup;
            if (this._dbContext.Meals.AnyAsync(m => m.MealType == Constants.MealType.Soup && m.Name.ToUpper().Equals(soupName.ToUpper())).Result == false)
            {
                Models.Meal newSoupMeal = new Models.Meal
                {
                    MealType = Constants.MealType.Soup,
                    Name = soupName
                };
                await this._dbContext.Meals.AddAsync(newSoupMeal);
                soup = newSoupMeal;
            }
            else
            {
                soup = this._dbContext.Meals.FirstAsync(m => m.MealType == Constants.MealType.Soup && m.Name.ToUpper().Equals(soupName.ToUpper())).Result;
            }

            Models.Meal mainDish;
            if (this._dbContext.Meals.AnyAsync(m => m.MealType == Constants.MealType.MainDish && m.Name.ToUpper().Equals(mainDishName.ToUpper())).Result == false)
            {
                Models.Meal newMainDishMeal = new Models.Meal
                {
                    MealType = Constants.MealType.MainDish,
                    Name = mainDishName
                };
                await this._dbContext.Meals.AddAsync(newMainDishMeal);
                mainDish = newMainDishMeal;
            }
            else
            {
                mainDish = this._dbContext.Meals.FirstAsync(m => m.MealType == Constants.MealType.MainDish && m.Name.ToUpper().Equals(mainDishName.ToUpper())).Result;
            }

            Models.Meal veganDish;
            if (this._dbContext.Meals.AnyAsync(m => m.MealType == Constants.MealType.VeganDish && m.Name.ToUpper().Equals(veganDishName.ToUpper())).Result == false)
            {
                Models.Meal newVeganDishMeal = new Models.Meal
                {
                    MealType = Constants.MealType.VeganDish,
                    Name = veganDishName
                };
                await this._dbContext.Meals.AddAsync(newVeganDishMeal);
                veganDish = newVeganDishMeal;
            }
            else
            {
                veganDish = this._dbContext.Meals.FirstAsync(m => m.MealType == Constants.MealType.VeganDish && m.Name.ToUpper().Equals(veganDishName.ToUpper())).Result;
            }

            Models.MealSchedule newMealSchedule = new Models.MealSchedule()
            {
                Date = date,
                SoupMealId = soup.Id,
                MainDishMealId = mainDish.Id,
                VeganDishMealId = veganDish.Id
            };
            await this._dbContext.MealSchedules.AddAsync(newMealSchedule);

            await this._dbContext.SaveChangesAsync();
        }
    
        public async Task<int> RemoveForDateRange(DateTime fromDate, DateTime toDate)
        {
            int removedMenusCount = 0;

            var mealSchedules = await this._dbContext.MealSchedules
                    .Where(ms => ms.Date.Date >= fromDate.Date && ms.Date.Date <= toDate.Date)
                    .ToListAsync();

            if (mealSchedules != null && mealSchedules.Count > 0)
            {
                this._dbContext.MealSchedules.RemoveRange(mealSchedules);

                removedMenusCount = await this._dbContext.SaveChangesAsync();
            }

            return removedMenusCount;
        }
    }
}