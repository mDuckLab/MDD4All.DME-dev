using System;
using System.Collections.Generic;

namespace MDD4All.DME.DataModels.BookExamples
{
    // A deliberately plain model. Everything in it is either a simple data type or a list of them,
    // so its JSON and its XML can be held side by side without dictionaries, inheritance or shared
    // references getting in the way - unlike PersonRepository, which exists to exercise exactly
    // those.
    public class Book
    {
        public Book()
        {
            Title = "Der Process";
            Author = "Franz Kafka";
            NumberOfPages = 264;
            Published = new DateTime(1925, 4, 26);
            IsHardcover = true;

            Chapters = new List<string>
            {
                "Verhaftung",
                "Erste Untersuchung",
                "Im Dom"
            };
        }

        public string Title { get; set; }

        public string Author { get; set; }

        public int NumberOfPages { get; set; }

        public DateTime Published { get; set; }

        public bool IsHardcover { get; set; }

        // The one place where the two formats visibly disagree: JSON writes a list as an array,
        // XML wraps every element in a tag of its own.
        public List<string> Chapters { get; set; }

        public override string ToString()
        {
            return Title + " (" + Author + ")";
        }
    }
}
