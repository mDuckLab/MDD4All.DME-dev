using System.Collections.Generic;

namespace MDD4All.DME.DataModels.HouseExamples
{
    // A list of simple values and an array next to it - both branch over the index, not over
    // property names.
    public class House
    {
        public House()
        {
            Street = string.Empty;
            Residents = new List<string>();
            RoomSizes = new int[0];
        }

        public House(string street, List<string> residents, int[] roomSizes)
        {
            Street = street;
            Residents = residents;
            RoomSizes = roomSizes;
        }

        public string Street { get; set; }

        public List<string> Residents { get; set; }

        public int[] RoomSizes { get; set; }

        public override string ToString()
        {
            return Street;
        }
    }
}
