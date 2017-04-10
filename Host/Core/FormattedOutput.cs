using System;
using System.Collections.Generic;
using System.Text;

namespace Host.Core
{
    public class FormattedOutput
    {
        public string Text { get; set; }
        public Action PreFormat { get; set; }
        public Action PostFormat { get; set; }
    }
}
