using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlParser.EventArgs
{
    class NewProgressBarValueEventsArgs : System.EventArgs
    {
        public double NewValue { get; }

        public NewProgressBarValueEventsArgs(double newValue)
        {
            NewValue = newValue;
        }
    }
}
