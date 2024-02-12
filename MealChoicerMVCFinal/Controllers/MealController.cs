using MealChoicerMVCFinal.Models;
using MealChoicerMVCFinal.Services;
using MealChoicerMVCFinal.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MealChoicerMVCFinal.Controllers
{
    public class MealController : Controller
    {
        private readonly IMealService _mealService;

        public MealController(IMealService mealService)
        {
            _mealService = mealService;
        }
        public IActionResult Index()
        {
            MealListViewModel viewModel = new()
            {
                Meals = _mealService.GetAllMeals(),
            };

            return View(viewModel);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(MealAddViewModel mealAddViewModel)
        {
            if (ModelState.IsValid)
            {
                Meal newMeal = new()
                {
                    Name = mealAddViewModel.Meal.Name,
                };

                _mealService.AddMeal(newMeal);
                return RedirectToAction("Index");
            }

            return View(mealAddViewModel);
        }

        public IActionResult Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var selectedMeal = _mealService.GetMealById(new string(id));
            return View(selectedMeal);
        }

        [HttpPost]
        public IActionResult Edit(Meal meal)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _mealService.EditMeal(meal);
                    return RedirectToAction("Index");
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Updating the Meal failed, please try again! Error:{ex.Message}");
            }

            return View(meal);
        }

        public IActionResult Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var selectedMeal = _mealService.GetMealById(new string(id));
            return View(selectedMeal);
        }

        [HttpPost]
        public IActionResult Delete(Meal meal)
        {
            if (meal.Id == null)
            {
                ViewData["ErrorMessage"] = "Deleting the meal failed, invalid ID!";
                return View();
            }

            try
            {
                _mealService.DeleteMeal(meal);
                TempData["MealDeleted"] = "Meal deleted successfully!";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = $"Deleting the meal failed, please try again! Error: {ex.Message}";
            }

            var selectedMeal = _mealService.GetMealById(meal.Id);
            return View(selectedMeal);
        }

        [HttpPost]
        public IActionResult GetRandomMeals(int howManyMeals)
        {

            var getRandomMeals = _mealService.GetRandomMeals(howManyMeals);

            MealListViewModel viewModel = new()
            {
                Meals = getRandomMeals
            };

            return View("Index", viewModel);
        }

    }
}
