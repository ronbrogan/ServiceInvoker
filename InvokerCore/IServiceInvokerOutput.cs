using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace InvokerCore
{
    public interface IServiceInvokerOutput
    {
        void Update(InvokerConfig config, IDictionary<Watch,Process> running);
    }
}
