using System;
using System.Collections.Generic;
using System.Text;

namespace Host.Core
{
    public interface IWatcher<T>
    {
        T OnInvoke(Action invoke);
        T DebounceWith(Func<bool> debounce);
        T DisplayChanges(Action update);
        void StartWatching();
    }
}
