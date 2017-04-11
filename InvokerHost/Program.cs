using InvokerCore;
using InvokerHost.Extensions;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;

namespace InvokerHost
{
    class Program
    {
        static ManualResetEvent _quitEvent = new ManualResetEvent(false);

        static string PluginDirectory = "plugins";

        static void Main(string[] args)
        {
            Console.CancelKeyPress += (sender, eArgs) => {
                _quitEvent.Set();
                eArgs.Cancel = true;
            };

            Console.WriteLine("Loading Plugins...");

            var watchers = LoadLocalWatchers();
            LoadWatcherPlugins(watchers);

            Console.WriteLine("Starting Invoker...");

            var invoker = new ServiceInvoker(watchers);

            invoker.Start(new ServiceInvokerConsoleDisplay());

            _quitEvent.WaitOne();

            Console.WriteLine("Stopping Invoker...");

            invoker.Shutdown();

            Thread.Sleep(100);
        }

        static Dictionary<string, Type> LoadLocalWatchers()
        {
            var watchers = new Dictionary<string, Type>();

            var assys = DependencyContext.Default.GetDefaultAssemblyNames();

            foreach (var assy in assys)
            {
                if (assy.FullName.StartsWith("System."))
                    continue;

                watchers.AddRange(GetWatchersFromAssembly(Assembly.Load(assy)));
            }
            
            return watchers;
        }

        // Search a directory for plugins. Load each assembly found. 
        static void LoadWatcherPlugins(Dictionary<string, Type> watchers)
        {
            var dlls = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), PluginDirectory), "*.dll", SearchOption.TopDirectoryOnly);

            foreach(var dll in dlls)
            {
                var assy = AssemblyLoadContext.Default.LoadFromAssemblyPath(dll);
                watchers.AddRange(GetWatchersFromAssembly(assy));
            }
        }

        static IEnumerable<KeyValuePair<string, Type>> GetWatchersFromAssembly(Assembly assy)
        {
            var types = assy.GetTypes();

            foreach (var type in types)
            {
                var attr = type.GetTypeInfo().GetCustomAttribute<RegisterWatcherAttribute>();

                if (attr == null)
                    continue;

                yield return new KeyValuePair<string, Type>(attr.Name, type);
            }
        }
    }
}