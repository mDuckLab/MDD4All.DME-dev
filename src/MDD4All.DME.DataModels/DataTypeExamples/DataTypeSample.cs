using System;
using System.ComponentModel.DataAnnotations;

namespace MDD4All.DME.DataModels.DataTypeExamples
{
    // One property per [DataType] the editor turns into a different input control, plus the two
    // rules that come as their own attribute rather than as a hint.
    public class DataTypeSample
    {
        public DataTypeSample()
        {
            Secret = "hunter2";
            BirthDate = new DateTime(1990, 4, 12);
            Alarm = new DateTime(2000, 1, 1, 7, 30, 0);
            Notes = "Mehrere Zeilen\nsind hier erlaubt.";
            Mail = "erika@example.com";
            Homepage = "https://example.com";
            Plain = "no hint on this one";
        }

        // Dots instead of the characters. Only on screen - the file still holds it in the clear.
        [DataType(DataType.Password)]
        public string Secret { get; set; }

        // A calendar rather than date and time together.
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        // Only the clock; the date part is carried along but not shown.
        [DataType(DataType.Time)]
        public DateTime Alarm { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        // Not a hint but a rule: EmailAddress derives from DataTypeAttribute and checks as well.
        [EmailAddress]
        public string Mail { get; set; }

        [Url]
        public string Homepage { get; set; }

        // For comparison. A plain text field, as every string without a hint.
        public string Plain { get; set; }

        public override string ToString()
        {
            return Mail;
        }
    }
}
