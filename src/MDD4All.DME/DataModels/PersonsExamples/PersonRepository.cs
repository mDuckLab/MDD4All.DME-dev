using System.Collections.Generic;

namespace MDD4All.DME.DataModels.PersonsExamples
{
    public class PersonRepository
    {
        private List<Person> _objects;

        private Person[] _persons = new Person[2];

        public PersonRepository()
        {
            _objects = new List<Person>();

            _persons[0] = new Person { FirstName = "Erika", LastName = "Musterfrau", Age = 38, IsFemale = true };
            _persons[1] = new Person { FirstName = "Hans", LastName = "Schmidt", Age = 52, IsFemale = false };
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
    }
}