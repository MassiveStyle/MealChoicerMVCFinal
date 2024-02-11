using MealChoicerMVCFinal.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;


namespace MealChoicerMVCFinal.Services
{
    public class MongoDBService
    {
        public IMongoCollection<Meal> _mealCollection { get; set; }

        public MongoDBService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.MongoDBURI);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _mealCollection = database.GetCollection<Meal>(mongoDBSettings.Value.CollectionName);
        }
    }
}
