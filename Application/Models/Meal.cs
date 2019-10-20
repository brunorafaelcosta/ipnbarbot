namespace ipnbarbot.Application.Models
{
    public class Meal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Constants.MealType MealType { get; set; }
    }
}