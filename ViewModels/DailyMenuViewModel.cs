using ipnbarbot.Application.Constants;
using Newtonsoft.Json;

namespace ipnbarbot.ViewModels
{
    public class DailyMenuViewModel
    {
        [JsonProperty(MealTypeNames.Soup)]
        public string Soup {get;set;}

        [JsonProperty(MealTypeNames.MainDish)]
        public string MainDish {get;set;}

        [JsonProperty(MealTypeNames.VeganDish)]
        public string VeganDish {get;set;}

        public static bool IsValid(DailyMenuViewModel obj)
        {
            return
                (obj != null) &&
                (string.IsNullOrEmpty(obj.Soup) == false) &&
                (string.IsNullOrEmpty(obj.MainDish) == false) &&
                (string.IsNullOrEmpty(obj.VeganDish) == false);
        }
    }
}