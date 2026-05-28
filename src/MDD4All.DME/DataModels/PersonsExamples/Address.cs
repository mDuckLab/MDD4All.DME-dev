namespace MDD4All.DME.DataModels.PersonsExamples
{
    public class Address
    {
        public Address()
        {
            Street = string.Empty;
            City = null; // new City();
        }

        public Address(string Street, uint HouseNumber, double Size, int PostCode, string CityName)
        {
            this.Street = Street;
            this.HouseNumber = HouseNumber;
            this.Size = Size;
            this.City = null;//new City(PostCode, CityName);
        }

        public string Street { get; set; }

        public uint HouseNumber { get; set; }    

        public double Size { get; set; }

        public  City? City { get; set; }

        public override string ToString()
        {
            return $"{Street} {HouseNumber}, {City}";
        }

    }
}
