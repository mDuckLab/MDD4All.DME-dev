using System.Collections.Generic;
using MDD4All.DME.DataModels.PersonsExamples;

namespace MDD4All.DME.DataModels.ReadOnlyExamples
{
    // Properties without a setter, one of each kind. The editor has to disable editing and
    // Create on them instead of throwing when it tries to write.
    public class ReadOnlySample
    {
        private Address _address;

        private List<string> _tags;

        public ReadOnlySample()
        {
            Title = "Nur lesbar";
            _address = new Address("Konstanterstrasse 5", "54321", new City("Immerstadt", "Deutschland"));
            _tags = new List<string> { "Beispiel", "Test" };
        }

        // Writable, so the difference to the three below is visible in the same object.
        public string Title { get; set; }

        // Computed from Title - there is nothing behind it that could be written.
        public string TitleInUpperCase
        {
            get
            {
                return Title.ToUpper();
            }
        }

        // A whole object behind a getter only, so Create has to stay disabled as well.
        public Address Address
        {
            get
            {
                return _address;
            }
        }

        // The property cannot be replaced, but the list itself can still take new elements.
        public List<string> Tags
        {
            get
            {
                return _tags;
            }
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
