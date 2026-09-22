using System.ComponentModel.DataAnnotations;

namespace MDD4All.DME.DataModels.ValidationExamples
{
    // One property per rule the editor knows how to word, so each can be tried on its own.
    // Everything starts out valid - the point is to break it by typing.
    public class ValidationSample
    {
        public ValidationSample()
        {
            Age = 30;
            Weight = 75.5;
            Name = "Erika";
            ShortCode = "AB";
            Comment = "kurz";
            PostalCode = "64289";
            Untouched = "no rule on this one";
        }

        // Whole numbers with both ends.
        [Range(0, 120)]
        public int Age { get; set; }

        // The same on a floating point value - the bounds are read as doubles here.
        [Range(0.5, 500.0)]
        public double Weight { get; set; }

        // Both ends at once. Two characters too few and twenty too many.
        [StringLength(20, MinimumLength = 2)]
        public string Name { get; set; }

        // Only a lower end.
        [MinLength(2)]
        public string ShortCode { get; set; }

        // Only an upper end.
        [MaxLength(10)]
        public string Comment { get; set; }

        // A shape rather than a length: five digits.
        [RegularExpression("^[0-9]{5}$")]
        public string PostalCode { get; set; }

        // For comparison. Nothing declared, so nothing is ever turned away.
        public string Untouched { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
