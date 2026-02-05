using System;
using System.Collections.Generic;

namespace MDD4All.DME.DataModels.PersonsExamples
{
    public class Person
    {
        public Person()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Address = new Address();
            dateTime = DateTime.Now;
            myIntList = new List<int> { 0, 1, 2, 3, 4, 5 };

            //Initialisierung für den leeren Fall
           ContactDetails = new Dictionary<string, string>();
            NamedAddresses = new Dictionary<string, Address>();
            RouteMap = new Dictionary<Address, Address>();
        }

        // Konstruktor mit 9 Argumenten für das PersonRepository
        public Person(string firstName, string lastName, int age, bool isFemale,
                      string street, uint houseNumber, double size, int postCode, string cityName)
        {
            FirstName = firstName;
            LastName = lastName;
            Age = age;
            IsFemale = isFemale;
            Address = new Address(street, houseNumber, size, postCode, cityName);
            dateTime = DateTime.Now;
            myIntList = new List<int> { 0, 1, 2 };

            // 1. Simple-Simple: string -> string
            ContactDetails = new Dictionary<string, string>
            {
                { "Mobile", "0176-1234567" },
                { "Email", firstName.ToLower() + "." + lastName.ToLower() + "@example.com" },
                { "Slack", "@" + firstName.ToLower() }
            };

            // 2. Simple-Complex: string -> Address
            NamedAddresses = new Dictionary<string, Address>
            {
                { "Work", new Address("Business-Park", 10u, 500.0, 12345, "Industriestadt") },
                { "Home-Office", new Address(street, houseNumber, size, postCode, cityName) }
            };

            // 3. Complex-Complex: Address -> Address
            // Hier nutzen wir zwei Address-Objekte als Key und Value
            Address startPoint = new Address(street, houseNumber, size, postCode, cityName);
            Address endPoint = new Address("Zielweg", 99u, 15.5, 54321, "Zielstadt");

            RouteMap = new Dictionary<Address, Address>
            {
                { startPoint, endPoint }
            };
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public bool IsFemale { get; set; }
        public Address Address { get; set; }
        public DateTime dateTime { get; set; } = DateTime.Now;
        public List<int> myIntList { get; set; }
        public int[]? myIntArray { get; set; } = null;
        public Address? WorkAddress { get; set; } = null;

        public Dictionary<string, string> ContactDetails { get; set; }
        public Dictionary<string, Address> NamedAddresses { get; set; }
        public Dictionary<Address, Address> RouteMap { get; set; }

        public override string ToString()
        {
            string gender = IsFemale ? "Weiblich" : "Männlich";
            return $"{FirstName} {LastName} ({Age}, {gender}) - {Address}";
        }
    }
}