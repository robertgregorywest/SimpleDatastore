using System.Collections.Generic;

namespace Example.Web.Models
{
    public class WidgetModel
    {
        public string Name { get; set; }
        public string MainPart { get; set; }
        public List<string> Parts { get; set; } = new List<string>();
    }
}