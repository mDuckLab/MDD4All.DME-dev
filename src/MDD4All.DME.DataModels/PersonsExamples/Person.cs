using System;
using System.ComponentModel.DataAnnotations;

namespace MDD4All.DME.DataModels.PersonsExamples
{
    // Four simple values, each of a different type, and one property that opens a branch of its
    // own. Name and Size carry rules, so the editor has something to enforce.
    public class Person
    {
        public Person()
        {
            Name = string.Empty;
            DayOfBirth = new DateTime(1990, 1, 1);
            Address = new Address();
        }

        [StringLength(20, MinimumLength = 2)]
        public string Name { get; set; }

        public int Age { get; set; }

        // In centimetres.
        [Range(50, 250)]
        public int Size { get; set; }

        public bool IsFemale { get; set; }

        public DateTime DayOfBirth { get; set; }

        public Address Address { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
