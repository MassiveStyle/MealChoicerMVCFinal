using MealChoicerMVCFinal.Models;

namespace MealChoicerMVCFinal.Services
{
    public interface IMealService
    {
        IEnumerable<Meal> GetAllMeals();
        Meal? GetMealById(string id);

        void AddMeal(Meal newMeal);
        void EditMeal(Meal editMeal);
        void DeleteMeal(Meal mealToDelete);
        IEnumerable<Meal> GetRandomMeals(Meal meal, int howManyMeals);
    }
}
