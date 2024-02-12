using MealChoicerMVCFinal.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Linq;

namespace MealChoicerMVCFinal.Services
{
    public class MealService : IMealService
    {
        private readonly MongoDBService _mongoDBService;

        public MealService(MongoDBService mongoDBservice)
        {
            _mongoDBService = mongoDBservice;
        }

        public IEnumerable<Meal> GetAllMeals()
        {
            return _mongoDBService._mealCollection.Find(new BsonDocument()).SortBy(c => c.Id).ToList();
        }

        public Meal? GetMealById(string id)
        {
            return _mongoDBService._mealCollection.Find(c => c.Id == id).FirstOrDefault();
        }

        public void AddMeal(Meal meal)
        {
            _mongoDBService._mealCollection.InsertOne(meal);

        }

        public void EditMeal(Meal meal)
        {
            var filter = Builders<Meal>.Filter.Eq("Id", meal.Id);
            var update = Builders<Meal>.Update.Set("Name", meal.Name);
            var result = _mongoDBService._mealCollection.UpdateOne(filter, update);

            if (result.ModifiedCount == 0)
            {
                throw new ArgumentException("Das zu editierende Gericht konnte nicht gefunden werden.");
            }

        }

        public void DeleteMeal(Meal meal)
        {
            var filter = Builders<Meal>.Filter.Eq("Id", meal.Id);
            var result = _mongoDBService._mealCollection.DeleteOne(filter);

            if (result.DeletedCount == 0)
            {
                throw new ArgumentException("Das zu löschende Gericht konnte nicht gefunden werden");
            }

        }

        public IEnumerable<Meal> GetRandomMeals(int howManyMeals)
        {
            int mealsPerGroup = 7;
            List<Meal> randomMeals = new List<Meal>();
            List<Meal> mealsToAdd = new List<Meal>();
            List<string> updatedMealIds = new List<string>();
            int remainingMealsToGenerate = howManyMeals;

            while (remainingMealsToGenerate > 0)
            {
                int mealsToGenerate = Math.Min(mealsPerGroup, mealsPerGroup - mealsToAdd.Count);

                var pipeline = new List<BsonDocument>
                {
                    new BsonDocument("$sample", new BsonDocument("size", mealsToGenerate)),
                    new BsonDocument("$match", new BsonDocument("pickedInLast7Meals", false))
                };

                var randomMealDocument = _mongoDBService._mealCollection.Aggregate<BsonDocument>(pipeline).ToList();
                IEnumerable<Meal> groupMeals = randomMealDocument.Select(bsonDocument => BsonSerializer.Deserialize<Meal>(bsonDocument));

                foreach (Meal meal in groupMeals)
                {
                    var filter = Builders<Meal>.Filter.Eq("Id", meal.Id);
                    var update = Builders<Meal>.Update.Set("pickedInLast7Meals", true);

                    _mongoDBService._mealCollection.UpdateOne(filter, update);

                    updatedMealIds.Add(meal.Id);
                }

                var generatedMeals = groupMeals.ToList();
                mealsToAdd.AddRange(generatedMeals);

                if (mealsToAdd.Count == mealsPerGroup)
                {
                    randomMeals.AddRange(mealsToAdd);
                    updatedMealIds = updatedMealIds.TakeLast(mealsPerGroup).ToList();
                    mealsToAdd.Clear();
                }

                remainingMealsToGenerate = howManyMeals - randomMeals.Count;

                var filter2 = Builders<Meal>.Filter.And(
                              Builders<Meal>.Filter.Nin("Id", updatedMealIds),
                              Builders<Meal>.Filter.Eq("pickedInLast7Meals", true));

                var update2 = Builders<Meal>.Update.Set("pickedInLast7Meals", false);

                _mongoDBService._mealCollection.UpdateMany(filter2, update2);
            }
            return randomMeals.Take(howManyMeals).ToList();
        }
    }
}
