using System.ComponentModel.DataAnnotations;

namespace MDD4All.DME.DataModels.DataAnnotationExamples
{
    public class Note
    {
        public Note()
        {
            Title = string.Empty;
            Content = string.Empty;
        }

        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
    }
}
