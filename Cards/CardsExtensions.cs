using System;
using System.Collections.Generic;
using System.Text;
using ipnbarbot.Application.Models;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace ipnbarbot.Cards
{
    public static class CardsExtensions
    {
        public static string GetDailyMenuCardAsString(MealSchedule mealSchedule)
        {
            if (mealSchedule is null)
                throw new ArgumentNullException(nameof(mealSchedule));

            StringBuilder result = new StringBuilder();

            result.Append(string.Format("**{0}**", mealSchedule.Day));
            result.Append(" \n ");
            result.Append(string.Format("* **{0}**: {1}", ipnbarbot.Application.Constants.MealTypeNames.Soup, mealSchedule.SoupMeal.Name));
            result.Append(" \n ");
            result.Append(string.Format("* **{0}**: {1}", ipnbarbot.Application.Constants.MealTypeNames.MainDish, mealSchedule.MainDishMeal.Name));
            result.Append(" \n ");
            result.Append(string.Format("* **{0}**: {1}", ipnbarbot.Application.Constants.MealTypeNames.VeganDish, mealSchedule.VeganDishMeal.Name));

            return result.ToString();
        }

        public static Attachment GetDailyMenuCard(MealSchedule mealSchedule)
        {
            if (mealSchedule is null)
                throw new ArgumentNullException(nameof(mealSchedule));
            
            StringBuilder content = new StringBuilder();
            
            content.Append("{");
            
            content.Append("\"$schema\": \"http://adaptivecards.io/schemas/adaptive-card.json\",");
            content.Append("\"type\": \"AdaptiveCard\",");
            content.Append("\"version\": \"1.0\",");

            content.Append("\"body\": [");

            content.Append("{");
            content.Append("\"type\": \"TextBlock\",");
            content.AppendFormat("\"text\": \"{0}\",", mealSchedule.Day);
            content.Append("\"weight\": \"Bolder\"");
            content.Append("},");

            content.Append("{");

            content.Append("\"type\": \"FactSet\",");
            content.Append("\"facts\": [");

            content.Append("{");
            content.AppendFormat("\"title\": \"{0}\",", ipnbarbot.Application.Constants.MealTypeNames.Soup);
            content.AppendFormat("\"value\": \"{0}\"", mealSchedule.SoupMeal.Name);
            content.Append("},");

            content.Append("{");
            content.AppendFormat("\"title\": \"{0}\",", ipnbarbot.Application.Constants.MealTypeNames.MainDish);
            content.AppendFormat("\"value\": \"{0}\"", mealSchedule.MainDishMeal.Name);
            content.Append("},");

            content.Append("{");
            content.AppendFormat("\"title\": \"{0}\",", ipnbarbot.Application.Constants.MealTypeNames.VeganDish);
            content.AppendFormat("\"value\": \"{0}\"", mealSchedule.VeganDishMeal.Name);
            content.Append("}");

            content.Append("]");

            content.Append("}");

            content.Append("]");

            content.Append("}");

            return new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(content.ToString()),
            };
        }

        public static string GetWeeklyMenuCardAsString(IEnumerable<MealSchedule> mealSchedules)
        {
            if (mealSchedules is null)
                throw new ArgumentNullException(nameof(mealSchedules));

            StringBuilder result = new StringBuilder();

            bool isFirstCard = true;
            foreach (var mealSchedule in mealSchedules)
            {
                if (isFirstCard == false)
                {
                    result.Append(" \n ");
                    result.Append(" \n ");
                }
                
                result.Append(string.Format("**{0}**", mealSchedule.Day));
                result.Append(" \n ");
                result.Append(string.Format("* **{0}**: {1}", ipnbarbot.Application.Constants.MealTypeNames.Soup, mealSchedule.SoupMeal.Name));
                result.Append(" \n ");
                result.Append(string.Format("* **{0}**: {1}", ipnbarbot.Application.Constants.MealTypeNames.MainDish, mealSchedule.MainDishMeal.Name));
                result.Append(" \n ");
                result.Append(string.Format("* **{0}**: {1}", ipnbarbot.Application.Constants.MealTypeNames.VeganDish, mealSchedule.VeganDishMeal.Name));
                
                isFirstCard = false;
            }
            
            return result.ToString();
        }

        public static Attachment GetWeeklyMenuCard(IEnumerable<MealSchedule> mealSchedules)
        {
            if (mealSchedules is null)
                throw new ArgumentNullException(nameof(mealSchedules));
            
            StringBuilder content = new StringBuilder();
            
            content.Append("{");
            
            content.Append("\"$schema\": \"http://adaptivecards.io/schemas/adaptive-card.json\",");
            content.Append("\"type\": \"AdaptiveCard\",");
            content.Append("\"version\": \"1.0\",");

            content.Append("\"body\": [");

            bool isFirstCard = true;
            foreach (var mealSchedule in mealSchedules)
            {
                if (isFirstCard == false)
                    content.Append(",");

                content.Append("{");
                content.Append("\"type\": \"TextBlock\",");
                content.AppendFormat("\"text\": \"{0}\",", mealSchedule.Day);
                content.Append("\"weight\": \"Bolder\"");
                if (isFirstCard == false)
                    content.Append(",\"separator\": true");
                content.Append("},");
                
                content.Append("{");

                content.Append("\"type\": \"FactSet\",");
                content.Append("\"facts\": [");

                content.Append("{");
                content.AppendFormat("\"title\": \"{0}\",", ipnbarbot.Application.Constants.MealTypeNames.Soup);
                content.AppendFormat("\"value\": \"{0}\"", mealSchedule.SoupMeal.Name);
                content.Append("},");

                content.Append("{");
                content.AppendFormat("\"title\": \"{0}\",", ipnbarbot.Application.Constants.MealTypeNames.MainDish);
                content.AppendFormat("\"value\": \"{0}\"", mealSchedule.MainDishMeal.Name);
                content.Append("},");

                content.Append("{");
                content.AppendFormat("\"title\": \"{0}\",", ipnbarbot.Application.Constants.MealTypeNames.VeganDish);
                content.AppendFormat("\"value\": \"{0}\"", mealSchedule.VeganDishMeal.Name);
                content.Append("}");

                content.Append("]");

                content.Append("}");

                isFirstCard = false;
            }

            content.Append("]");

            content.Append("}");

            return new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(content.ToString()),
            };
        }
    }
}