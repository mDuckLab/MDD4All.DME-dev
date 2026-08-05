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
            ReadOnlyAddress = new Address("Konstanterstraße", 5u, 42.0, 54321, "Immerstadt");
            myIntList = new List<int> { 0, 1, 2, 3, 4, 5 };
            myIntArray = new int[] { 10, 20, 30 };

            // 1. Simple-Simple: string -> string
            ContactDetails = new Dictionary<string, string>
            {
                { "Mobile", "0176-1234567" },
                { "Email", "max.mustermann@example.com" },
                { "Slack", "@maxmustermann" }
            };

            // 2. Simple-Complex: string -> Address
            NamedAddresses = new Dictionary<string, Address>
            {
                { "Work", new Address("Business-Park", 10u, 500.0, 12345, "Industriestadt") },
                { "Home-Office", new Address("Musterweg", 1u, 85.5, 12345, "Musterstadt") }
            };
        }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int Age { get; set; }
        public bool IsFemale { get; set; }
        public Address? Address { get; set; }
        public DateTime? DayOfBirth { get; set; } = DateTime.Now;

        public List<int>? myIntList { get; set; }
        public int[]? myIntArray { get; set; }

        public Dictionary<string, string>? ContactDetails { get; set; }
        public Dictionary<string, Address>? NamedAddresses { get; set; }

        // Read-only test properties, no setter at all - used to verify the
        // editor correctly disables editing/Create instead of throwing.
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        public Address? ReadOnlyAddress { get; }

        public override string ToString()
        {
            string gender = IsFemale ? "Weiblich" : "Männlich";
            return $"{FirstName} {LastName} ({Age}, {gender}) - {Address}";
        }
    }
}