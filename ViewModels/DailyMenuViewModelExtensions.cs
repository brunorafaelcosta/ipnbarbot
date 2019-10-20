using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ipnbarbot.Application.Constants;

namespace ipnbarbot.ViewModels
{
    public static class DailyMenuViewModelExtensions
    {
        public static Dictionary<string, ViewModels.DailyMenuViewModel> GetFromText(string menuText)
        {
            if (menuText is null || !(new string[] { "SUGEST", "DO DIA" }.All(s => menuText.ToUpper().Contains(s))))
                throw new Exception("Invalid menu text!");

            // Text clean up
            menuText = Regex.Replace(menuText, @"\s+", " ");
            if (menuText.IndexOf("Valores atualizados") > 0)
                menuText = menuText.Substring(0, menuText.IndexOf("Valores atualizados"));

            Dictionary<string, ViewModels.DailyMenuViewModel> result = new Dictionary<string, ViewModels.DailyMenuViewModel>();

            string dayRegexPattern = $"({string.Join("|", DaysOfWeek.ListOfDays)})([\\s\\S]*?)(?=(?:{string.Join("|", DaysOfWeek.ListOfDays)})|$)";
            foreach (Match dayMatch in Regex.Matches(menuText, dayRegexPattern).Where(m => m.Success && m.Groups.Count == 3))
            {
                string dayName = dayMatch.Groups[1].Value;

                string mealRegexPattern = $"({string.Join("|", MealTypeNames.ListOfMealTypeNames)})([\\s\\S]*?)(?=(?:{string.Join("|", MealTypeNames.ListOfMealTypeNames)})|$)";
                var mealRegexMatches = Regex.Matches(dayMatch.Groups[2].Value, mealRegexPattern);
                if (mealRegexMatches.Count == 3)
                {
                    string soupMeal = mealRegexMatches.Single(m => m.Groups[1].Value == MealTypeNames.Soup).Groups[2].Value;
                    string mainDishMeal = mealRegexMatches.Single(m => m.Groups[1].Value == MealTypeNames.MainDish).Groups[2].Value;
                    string veganDishMeal = mealRegexMatches.Single(m => m.Groups[1].Value == MealTypeNames.VeganDish).Groups[2].Value;

                    if (soupMeal is null || string.IsNullOrEmpty(soupMeal.Trim()))
                        continue;
                    if (mainDishMeal is null || string.IsNullOrEmpty(mainDishMeal.Trim()))
                        continue;
                    if (veganDishMeal is null || string.IsNullOrEmpty(veganDishMeal.Trim()))
                        continue;

                    soupMeal = soupMeal.Trim(new char[] { '.', '*', ':', ' ' });
                    mainDishMeal = mainDishMeal.Trim(new char[] { '.', '*', ':', ' ' });
                    veganDishMeal = veganDishMeal.Trim(new char[] { '.', '*', ':', ' ' });

                    if (string.IsNullOrEmpty(soupMeal.Trim()))
                        continue;
                    if (string.IsNullOrEmpty(mainDishMeal.Trim()))
                        continue;
                    if (string.IsNullOrEmpty(veganDishMeal.Trim()))
                        continue;

                    result.Add(dayName, new ViewModels.DailyMenuViewModel() { Soup = soupMeal, MainDish = mainDishMeal, VeganDish = veganDishMeal });
                }
            }

            return result;
        }
    }
}
