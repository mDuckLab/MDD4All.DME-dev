namespace MDD4All.DME.DataModels.PersonsExamples
{
    public class Address
    {
        public Address()
        {
            Street = string.Empty;
            PostalCode = string.Empty;
            City = new City();
        }

        public Address(string street, string postalCode, City city)
        {
            Street = street;
            PostalCode = postalCode;
            City = city;
        }

        public string Street { get; set; }

        public string PostalCode { get; set; }

        public City City { get; set; }

        public override string ToString()
        {
            return Street + ", " + PostalCode + " " + City.Name;
        }
    }
}
