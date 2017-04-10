using System;
using System.Collections.Generic;
using System.Text;

namespace Host.Core
{
    public class Watch
    {
        public WatchType Type { get; set; }
        public string Path { get; set; }
        public string Args { get; set; }
        public Dictionary<string,string> Environment { get; set; }
    }
}
