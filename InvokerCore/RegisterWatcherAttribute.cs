using System;
using System.Collections.Generic;
using System.Text;

namespace InvokerCore
{
    public class RegisterWatcherAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
