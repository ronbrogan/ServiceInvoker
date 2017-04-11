using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace InvokerCore
{
    public abstract class Watcher
    {
        protected Action DisplayUpdate { get; set; }
        protected Action InvokeCallback { get; set; }
        protected Func<bool> ShouldSkip { get; set; }

        protected Watch Watch;
        protected ConcurrentDictionary<Watch, Process> RunningProcesses { get; set; }

        private Watcher() { }

        public Watcher(Watch watch, ConcurrentDictionary<Watch, Process> runningProcesses)
        {
            Watch = watch;
            RunningProcesses = runningProcesses;
        }

        public Watcher OnInvoke(Action invoke)
        {
            InvokeCallback = invoke;
            return this;
        }

        public Watcher DebounceWith(Func<bool> debounce)
        {
            ShouldSkip = debounce;
            return this;
        }

        public Watcher DisplayChanges(Action update)
        {
            DisplayUpdate = update;
            return this;
        }

        public virtual void StartWatching() { }
    }
}
