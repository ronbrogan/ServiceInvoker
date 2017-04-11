using InvokerHost.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using InvokerCore;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyModel;

namespace InvokerHost
{
    public class ServiceInvoker 
    {
        private Dictionary<string, Type> RegisteredWatchers { get; set; }

        private int DebounceTimeout = 1500;
        private Func<bool> DebounceCheck { get; set; }

        private InvokerConfig Config { get; set; }
        private ConcurrentDictionary<Watch, Process> RunningProcesses { get; set; }
        private ConcurrentDictionary<Watch, DateTime> LastInvokeTimes { get; set; }
        private IServiceInvokerOutput Display { get; set; }

        public ServiceInvoker(Dictionary<string, Type> watchers)
        {
            RegisteredWatchers = watchers;
        }

        public void Start(IServiceInvokerOutput display)
        {
            RunningProcesses = new ConcurrentDictionary<Watch, Process>();
            LastInvokeTimes = new ConcurrentDictionary<Watch, DateTime>();

            Display = display;

            JsonConfiguration.Configure();
            LoadConfiguration();
            SetupWatches();
        }

        private void SetupWatches()
        {
            foreach(var watch in Config.Watches)
            {
                if (!RegisteredWatchers.ContainsKey(watch.Type))
                    throw new Exception($"Invalid type specified: {watch.Type}, no registered watcher matches.");

                var watcher = (Watcher)Activator.CreateInstance(RegisteredWatchers[watch.Type], watch, RunningProcesses);
                
                watcher.OnInvoke(() => LastInvokeTimes.AddOrUpdate(watch, DateTime.Now, (w, oldDate) => DateTime.Now))
                    .DebounceWith(() => LastInvokeTimes.ContainsKey(watch) && (DateTime.Now - LastInvokeTimes[watch]).TotalMilliseconds < DebounceTimeout)
                    .DisplayChanges(() => Display.Update(Config, RunningProcesses))
                    .StartWatching();
            }
        }

        private void LoadConfiguration()
        {
            using (var config = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "invokerconfig.json"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            using (var reader = new StreamReader(config))
            {
                Config = JsonConvert.DeserializeObject<InvokerConfig>(reader.ReadToEnd());
            }
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine(e.ChangeType);
        }

        public void Shutdown()
        {
            foreach(var proc in RunningProcesses.Values)
            {
                proc.Kill();
            }
        }

    }
}
