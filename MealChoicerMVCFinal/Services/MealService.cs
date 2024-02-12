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
            List<Meal> mealsToAddIncomplete = new List<Meal>();
            int remainingMealsToGenerate = howManyMeals;

            while (remainingMealsToGenerate > 0)
            {
                int mealsToGenerate = Math.Min(mealsPerGroup, mealsPerGroup - mealsToAddIncomplete.Count);

                var pipeline = new List<BsonDocument>
                {
                    new BsonDocument("$sample", new BsonDocument("size", mealsToGenerate)),
                };

                var randomMealDocument = _mongoDBService._mealCollection.Aggregate<BsonDocument>(pipeline).ToList();
                IEnumerable<Meal> groupMeals = randomMealDocument.Select(bsonDocument => BsonSerializer.Deserialize<Meal>(bsonDocument));

                var excludeMeals = randomMeals.TakeLast(mealsPerGroup).Concat(mealsToAddIncomplete).ToList();
                var mealsToAdd = groupMeals.Except(excludeMeals).ToList();

                mealsToAddIncomplete.AddRange(mealsToAdd);

                if (mealsToAddIncomplete.Count == mealsPerGroup)
                {
                    randomMeals.AddRange(mealsToAddIncomplete);
                    mealsToAddIncomplete.Clear();
                }

                remainingMealsToGenerate = howManyMeals - randomMeals.Count;
            }

            return randomMeals.Take(howManyMeals);
        }

    }
}
