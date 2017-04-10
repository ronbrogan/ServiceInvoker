using Host.Core;
using Host.Watchers;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace Host
{
    public static class ServiceInvoker 
    {
        private static int DebounceTimeout = 1500;
        private static Func<bool> DebounceCheck { get; set; }

        private static InvokerConfig Config { get; set; }
        private static ConcurrentDictionary<Watch, Process> RunningProcesses { get; set; }
        private static ConcurrentDictionary<Watch, DateTime> LastInvokeTimes { get; set; }
        private static IServiceInvokerOutput Display { get; set; }

        public static void Start(IServiceInvokerOutput display)
        {
            RunningProcesses = new ConcurrentDictionary<Watch, Process>();
            LastInvokeTimes = new ConcurrentDictionary<Watch, DateTime>();

            Display = display;

            JsonConfiguration.Configure();
            LoadConfiguration();
            SetupWatches();
        }

        private static void SetupWatches()
        {
            foreach(var watch in Config.Watches)
            {
                switch (watch.Type)
                {
                    case WatchType.DotNetCore:
                        SetupDotNetCoreWatch(watch);
                        break;

                    default:
                        break;
                }

            }
        }

        private static void SetupDotNetCoreWatch(Watch watch)
        {
            new DotNetCoreWatcher(watch, RunningProcesses)
                .OnInvoke(() => LastInvokeTimes.AddOrUpdate(watch, DateTime.Now, (w, oldDate) => DateTime.Now))
                .DebounceWith(() => LastInvokeTimes.ContainsKey(watch) && (DateTime.Now - LastInvokeTimes[watch]).TotalMilliseconds < DebounceTimeout)
                .DisplayChanges(() => Display.Update(Config, RunningProcesses))
                .StartWatching();
        }

        private static void LoadConfiguration()
        {
            using (var config = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "invokerconfig.json"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            using (var reader = new StreamReader(config))
            {
                Config = JsonConvert.DeserializeObject<InvokerConfig>(reader.ReadToEnd());
            }
        }

        private static void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine(e.ChangeType);
        }

        public static void Shutdown()
        {
            foreach(var proc in RunningProcesses.Values)
            {
                proc.Kill();
            }
        }

    }
}
