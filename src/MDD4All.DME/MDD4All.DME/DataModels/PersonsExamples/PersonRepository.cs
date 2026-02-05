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

            Person p1 = new Person("Anna", "Beispiel", 30, false, "Musterweg", 1, 85.5, 12345, "Musterstadt");
            _objects.Add(p1);

            Person p2 = new Person("Max", "Mustermann", 45, false, "Hauptstraße", 10, 75.0, 10115, "Berlin");
            _objects.Add(p2);

            _persons[0] = new Person("Erika", "Musterfrau", 38, true, "Lindenstraße", 42, 65.2, 20095, "Hamburg");
            _persons[1] = new Person("Hans", "Schmidt", 52, false, "Goethestraße", 5, 90.0, 80331, "München");
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