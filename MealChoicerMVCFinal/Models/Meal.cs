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

        public override bool Equals(object? obj)
        {
            return Equals(obj as Meal);
        }

        public bool Equals(Meal other)
        {
            return other != null && Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
