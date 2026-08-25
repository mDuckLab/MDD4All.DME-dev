using System;
using System.Collections.Generic;
using MDD4All.DME.DataModels.PersonsExamples;

namespace MDD4All.DME.DataModels.AddressBookExamples
{
    // Three dictionaries, each one step further: simple values, objects as values, and finally
    // an object as the key as well.
    public class AddressBook
    {
        public AddressBook()
        {
            PhoneNumbers = new Dictionary<string, string>
            {
                { "Erika", "0176-1234567" },
                { "Hans", "0170-7654321" }
            };

            Addresses = new Dictionary<string, Address>
            {
                { "Erika", new Address("Hauptstrasse 12", "64289", new City("Darmstadt", "Deutschland")) },
                { "Hans", new Address("Bahnhofsweg 3", "60311", new City("Frankfurt", "Deutschland")) }
            };

            Occupants = new Dictionary<Address, Person>();

            foreach (KeyValuePair<string, Address> eintrag in Addresses)
            {
                Occupants.Add(eintrag.Value, new Person
                {
                    Name = eintrag.Key,
                    Age = 40,
                    DayOfBirth = new DateTime(1985, 6, 1),
                    Address = eintrag.Value
                });
            }
        }

        public Dictionary<string, string> PhoneNumbers { get; set; }

        public Dictionary<string, Address> Addresses { get; set; }

        // The key is an object too, so it cannot be written as a plain JSON property name.
        public Dictionary<Address, Person> Occupants { get; set; }
    }
}
