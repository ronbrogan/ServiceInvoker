using System;
using System.IO;
using System.Threading;

namespace Host
{
    class Program
    {
        static ManualResetEvent _quitEvent = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            Console.CancelKeyPress += (sender, eArgs) => {
                _quitEvent.Set();
                eArgs.Cancel = true;
            };

            Console.WriteLine("Starting Invoker...");

            ServiceInvoker.Start(new ServiceInvokerConsoleDisplay());

            _quitEvent.WaitOne();

            Console.WriteLine("Stopping Invoker...");

            ServiceInvoker.Shutdown();

            Thread.Sleep(100);
        }
        private static void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine(File.ReadAllText(e.FullPath));
        }
    }
}