using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Host.Core
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum WatchType
    {
        Logger,
        Executable,
        DotNetCore
    }
}
