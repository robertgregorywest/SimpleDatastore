using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Example.Models
{
    public class WidgetModel
    {
        public string Name { get; set; }
        public List<string> Parts { get; set; }

        public WidgetModel()
        {
            Parts = new List<string>();
        }
    }
}