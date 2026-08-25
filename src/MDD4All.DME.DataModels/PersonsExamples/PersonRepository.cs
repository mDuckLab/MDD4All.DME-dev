using System;
using System.Collections.Generic;

namespace MDD4All.DME.DataModels.PersonsExamples
{
    // Complex types only - Person opens into Address, Address opens into City. The list of
    // persons is the one collection here, and it holds objects rather than values.
    public class PersonRepository
    {
        public PersonRepository()
        {
            Persons = new List<Person>
            {
                new Person
                {
                    Name = "Erika Musterfrau",
                    Age = 38,
                    IsFemale = true,
                    DayOfBirth = new DateTime(1987, 4, 12),
                    Address = new Address("Hauptstrasse 12", "64289", new City("Darmstadt", "Deutschland"))
                },
                new Person
                {
                    Name = "Hans Schmidt",
                    Age = 52,
                    IsFemale = false,
                    DayOfBirth = new DateTime(1973, 9, 3),
                    Address = new Address("Bahnhofsweg 3", "60311", new City("Frankfurt", "Deutschland"))
                }
            };
        }

        public List<Person> Persons { get; set; }
    }
}
