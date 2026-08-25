using System.Collections.Generic;

namespace MDD4All.DME.DataModels.HouseExamples
{
    public class HouseRepository
    {
        public HouseRepository()
        {
            Houses = new List<House>
            {
                new House("Musterweg 7", new List<string> { "Anna", "Ben" }, new int[] { 24, 18, 12 }),
                new House("Lindenallee 21", new List<string> { "Clara" }, new int[] { 31, 15 })
            };
        }

        public List<House> Houses { get; set; }
    }
}
