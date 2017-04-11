using InvokerCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace InvokerCore
{
    public class Watch
    {
        public string Type { get; set; }
        public string Path { get; set; }
        public string Args { get; set; }
        public IDictionary<string,string> Environment { get; set; }
    }
}
