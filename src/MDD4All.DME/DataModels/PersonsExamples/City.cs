namespace MDD4All.DME.DataModels.PersonsExamples
{
    public class City
    {
        public City()
        {
            CityName = string.Empty;
        }

        public City(int PostCode, string CityName)
        {
            this.PostCode = PostCode;
            this.CityName = CityName;
        }

        public int PostCode { set; get; }

        public string CityName { set; get; }

        public override string ToString()
        {
            return $"{PostCode} {CityName}";
        }
    }
}