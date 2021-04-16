using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.Infrastructure
{
    public class Service
    {
        public Service(string name)
        {
            Name = name;
            Widgets = new List<Widget>();
        }

        public string Name;
        public List<Widget> Widgets;

        public Widget GetWidget(string widgetname)
        {
            foreach (var widget in Widgets)
            {
                if (widget.Name == widgetname)
                {
                    return widget;
                }
            }
            return null;
        }
    }
}
