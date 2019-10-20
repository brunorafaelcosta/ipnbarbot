using System;
using System.Text;

namespace ipnbarbot.Application.Models
{
    public class MealSchedule
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int SoupMealId { get; set; }
        public int MainDishMealId { get; set; }
        public int VeganDishMealId { get; set; }

        public virtual Meal SoupMeal { get; set; }
        public virtual Meal MainDishMeal { get; set; }
        public virtual Meal VeganDishMeal { get; set; }

        public string Day => Helpers.DateHelpers.GetDayOfWeekForDailyMenuByDate(this.Date);

        public string ToFormat()
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine(string.Format("{0}: {1}", Constants.MealTypeNames.Soup, this.SoupMeal.Name));
            result.AppendLine(string.Format("{0}: {1}", Constants.MealTypeNames.MainDish, this.MainDishMeal.Name));
            result.AppendLine(string.Format("{0}: {1}", Constants.MealTypeNames.VeganDish, this.VeganDishMeal.Name));
            return result.ToString();
        }
    }
}