using System;
using System.Linq;

namespace NotesApp.Models
{
    public class Note
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime LastModifiedDate { get; set; } = DateTime.Now;

        public int Year => LastModifiedDate.Year;

        public string Title
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Content))
                    return "新建备忘录";
                var firstLine = Content.Split('\n')[0].TrimEnd('\r');
                return firstLine.Length > 40 ? firstLine.Substring(0, 40) + "..." : firstLine;
            }
        }

        public string Preview
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Content))
                    return string.Empty;
                var lines = Content.Split('\n');
                if (lines.Length > 1)
                {
                    var body = string.Join(" ", lines.Skip(1)).Trim();
                    return body.Length > 60 ? body.Substring(0, 60) + "..." : body;
                }
                return string.Empty;
            }
        }

        public string DateDisplay => LastModifiedDate.ToString("yyyy/MM/dd HH:mm");
    }

    public class YearGroup
    {
        public int Year { get; set; }
        public System.Collections.ObjectModel.ObservableCollection<Note> Notes { get; set; } = new();
    }
}
