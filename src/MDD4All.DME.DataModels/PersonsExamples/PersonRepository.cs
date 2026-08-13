using System.Collections.Generic;

namespace MDD4All.DME.DataModels.PersonsExamples
{
    public class PersonRepository
    {
        private List<Person> _objects;

        private Person[] _persons = new Person[2];

        private Dictionary<Address, Person> _residents = new Dictionary<Address, Person>();

        public PersonRepository()
        {
            _objects = new List<Person>();

            _persons[0] = new Person { FirstName = "Erika", LastName = "Musterfrau", Age = 38, IsFemale = true };
            _persons[1] = new Person { FirstName = "Hans", LastName = "Schmidt", Age = 52, IsFemale = false };

            _residents.Add(new Address("Hauptstrasse", 12, 85.5, 64289, "Darmstadt"), _persons[0]);
            _residents.Add(new Address("Bahnhofsweg", 3, 120.0, 60311, "Frankfurt"), _persons[1]);
        }


        public List<Person> Persons
        {
            get
            {
                return _objects;
            }
            set
            {
                _objects = value;
            }

        }

        public Person[] PersonArray
        {
            get { return _persons; }
            set { _persons = value; }
        }

        // Both key and value are classes. JSON property names can only be strings, so this cannot
        // be written as a plain object - it needs the converter that handles complex keys.
        public Dictionary<Address, Person> Residents
        {
            get { return _residents; }
            set { _residents = value; }
        }
    }
}