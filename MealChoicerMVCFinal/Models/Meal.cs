using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace MealChoicerMVCFinal.Models
{
    public class Meal : IEquatable<Meal>
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required(ErrorMessage = "Gib bitte den Namen des Gerichts an")]
        [Display(Name = "Name")]
        public string? Name { get; set; }

        public string? Recipe { get; set; }

        public string? Ingredients { get; set; }

        public bool pickedInLast7Meals { get; set; }

        public bool Equals(Meal other)
        {
            if (other == null)
                return false;

            return this.Id == other.Id && this.Name == other.Name;
        }

        public override int GetHashCode()
        {
            return (Id?.GetHashCode() ?? 0) ^ (Name?.GetHashCode() ?? 0);
        }
    }
}
