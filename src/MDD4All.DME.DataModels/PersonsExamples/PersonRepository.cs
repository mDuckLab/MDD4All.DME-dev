using System;
using System.Collections.Generic;

namespace MDD4All.DME.DataModels.PersonsExamples
{
    // Complex types only - Person opens into Address, Address opens into City. Both collections
    // hold objects rather than values, and both branch over the index: the list grows and
    // shrinks, the array keeps the length it was created with.
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

            Archived = new Person[]
            {
                new Person
                {
                    Name = "Peter Klein",
                    Age = 67,
                    IsFemale = false,
                    DayOfBirth = new DateTime(1958, 2, 24),
                    Status = MaritalStatus.Widowed,
                    Address = new Address("Feldweg 8", "34117", new City("Kassel", "Deutschland"))
                },
                new Person
                {
                    Name = "Maria Gruber",
                    Age = 45,
                    IsFemale = true,
                    DayOfBirth = new DateTime(1980, 11, 7),
                    Status = MaritalStatus.Divorced,
                    Address = new Address("Seestrasse 4", "88045", new City("Friedrichshafen", "Deutschland"))
                },
                new Person
                {
                    Name = "Jonas Weber",
                    Age = 29,
                    IsFemale = false,
                    DayOfBirth = new DateTime(1996, 6, 30),
                    Address = new Address("Am Markt 15", "04109", new City("Leipzig", "Deutschland"))
                }
            };
        }

        public List<Person> Persons { get; set; }

        public Person[] Archived { get; set; }
    }
}
