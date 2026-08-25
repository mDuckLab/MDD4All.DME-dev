namespace MDD4All.DME.DataModels.PersonsExamples
{
    public class City
    {
        public City()
        {
            Name = string.Empty;
            Country = string.Empty;
        }

        public City(string name, string country)
        {
            Name = name;
            Country = country;
        }

        public string Name { get; set; }

        public string Country { get; set; }

        public override string ToString()
        {
            return Name + " (" + Country + ")";
        }
    }
}
