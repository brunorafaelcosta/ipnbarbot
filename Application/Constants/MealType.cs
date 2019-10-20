namespace ipnbarbot.Application.Constants
{
    public enum MealType
    {
        Soup,
        MainDish,
        VeganDish
    }

    public static class MealTypeNames
    {
        public const string Soup = "Sopa do dia";
        public const string MainDish = "Sugestão do dia";
        public const string VeganDish = "VeGano";

        public static string[] ListOfMealTypeNames = new string[] { Soup, MainDish, VeganDish };
    }
}