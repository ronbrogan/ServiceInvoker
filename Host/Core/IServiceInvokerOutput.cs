using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Host.Core
{
    public interface IServiceInvokerOutput
    {
        void Update(InvokerConfig config, IDictionary<Watch,Process> running);

    }
}
