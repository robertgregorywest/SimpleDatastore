using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleCore.Models
{
    public class WidgetModel
    {
        public string Name { get; set; }
        public string MainPart { get; set; }
        public List<string> Parts { get; set; }

        public WidgetModel()
        {
            Parts = new List<string>();
        }
    }
}
