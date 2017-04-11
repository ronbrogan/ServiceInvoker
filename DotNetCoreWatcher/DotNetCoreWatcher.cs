using InvokerCore;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

namespace DotNetCoreWatcher
{
    [RegisterWatcher(Name = "dotNetCore")]
    public class DotNetCoreWatcher : Watcher
    {
        private FileSystemWatcher Watcher { get; set; }

        public DotNetCoreWatcher(Watch watch, ConcurrentDictionary<Watch, Process> runningProcesses) : base(watch, runningProcesses)
        {
            
        }

        public override void StartWatching()
        {
            var actualPath = Path.GetDirectoryName(Path.GetFullPath(Watch.Path));
            var binPath = CsprojHelper.GetBinaryPath(actualPath);
            var outputDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()) + "\\";

            // Run Process Delegate
            WaitCallback runProcess = (o) =>
            {
                var process = new ProcessStartInfo()
                {
                    CreateNoWindow = false,
                    WorkingDirectory = actualPath,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    RedirectStandardError = true
                };

                if (Path.GetExtension(binPath) == ".exe")
                {
                    process.FileName = Path.Combine(outputDir, Path.GetFileName(binPath));
                }
                else
                {
                    process.FileName = "dotnet";
                    process.Arguments = Path.Combine(outputDir, Path.GetFileName(binPath));
                }

                foreach (var env in Watch.Environment)
                {
                    process.Environment[env.Key] = env.Value;
                }

                if (!string.IsNullOrWhiteSpace(Watch.Args))
                {
                    process.Arguments = string.Join(" ", process.Arguments, Watch.Args);
                }

                var proc = Process.Start(process);
                proc.EnableRaisingEvents = true;
                RunningProcesses.GetOrAdd(Watch, proc);
                DisplayUpdate?.Invoke();
                proc.WaitForExit();
                RunningProcesses.TryRemove(Watch, out proc);

                if (Directory.Exists(outputDir))
                    Directory.Delete(outputDir, true);

                DisplayUpdate?.Invoke();
            };

            Watcher = new FileSystemWatcher(Path.GetDirectoryName(binPath), Path.GetFileName(binPath))
            {
                NotifyFilter = NotifyFilters.LastWrite
            };

            Watcher.Changed += (sender, e) =>
            {
                if (ShouldSkip?.Invoke() ?? false)
                    return;

                InvokeCallback?.Invoke();

                if (RunningProcesses.ContainsKey(Watch))
                {
                    RunningProcesses[Watch].Kill();
                }

                Thread.Sleep(500);
                Extensions.CloneDirectory(Path.GetDirectoryName(binPath), outputDir);
                ThreadPool.QueueUserWorkItem(runProcess);
            };

            Watcher.EnableRaisingEvents = true;

            if (!RunningProcesses.ContainsKey(Watch))
            {
                // Initial Run - we want to run at startup
                Extensions.CloneDirectory(Path.GetDirectoryName(binPath), outputDir);
                ThreadPool.QueueUserWorkItem(runProcess);
            }
        }
    }
}
