using System;

namespace MDD4All.DME.DataModels.PersonsExamples
{
    // Four simple values, each of a different type, and one property that opens a branch of its own.
    public class Person
    {
        public Person()
        {
            Name = string.Empty;
            DayOfBirth = new DateTime(1990, 1, 1);
            Address = new Address();
        }

        public string Name { get; set; }

        public int Age { get; set; }

        public bool IsFemale { get; set; }

        public DateTime DayOfBirth { get; set; }

        public Address Address { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
