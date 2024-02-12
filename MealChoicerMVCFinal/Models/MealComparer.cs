namespace MealChoicerMVCFinal.Models
{
    public class MealComparer : IEqualityComparer<Meal>
    {
        public bool Equals(Meal x, Meal y)
        {
            if (object.ReferenceEquals(x, y))
            {
                return true;
            }

            if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null))
            {
                return false;
            }

            return x.Id == y.Id && x.Name == y.Name;
        }

        public int GetHashCode(Meal obj)
        {
            if (obj == null)
            {
                return 0;
            }

            int IdHashCode = obj.Id.GetHashCode();

            int NameHashCode = obj.Name == null ? 0 : obj.Name.GetHashCode();

            return IdHashCode ^ NameHashCode;
        }
    }
}
