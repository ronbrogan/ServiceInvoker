using InvokerHost.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Net;
using InvokerCore;

namespace InvokerHost
{
    public class ServiceInvokerConsoleDisplay : IServiceInvokerOutput
    {
        Dictionary<int, int> ColWidths { get; set;}

        public ServiceInvokerConsoleDisplay()
        {
            ColWidths = new Dictionary<int, int>
            {
                {0, 4 },
                {1, 15 },
                {2, 30 },
                {3, 8 },
                {4, 7 },
                {5, 21 },
                {6, 14 }
            };
        }

        public void Update(InvokerConfig config, IDictionary<Watch, Process> running)
        {
            ScaffoldFrame();
            WriteRegisteredWatches(config, running);
        }

        private void ScaffoldFrame()
        {
            Console.Clear();
            WriteHeader("Service Invoker");
        }

        private void WriteRegisteredWatches(InvokerConfig config, IDictionary<Watch, Process> running)
        {
            foreach (var watch in config.Watches)
            {
                var pid = running.ContainsKey(watch) ? running[watch].Id : (int?)null;
                var binding = string.Empty;

                if (pid.HasValue)
                {
                    binding = watch.Environment.ContainsKey("ASPNETCORE_URLS") ? watch.Environment["ASPNETCORE_URLS"] : string.Empty;
                }

                WriteTabularOutput(
                    ColWidths,
                    config.Watches.IndexOf(watch),
                    watch.Type,
                    watch.Path,
                    new FormattedOutput {
                        PreFormat = () => Console.ForegroundColor = running.ContainsKey(watch) ? ConsoleColor.Green : ConsoleColor.Red,
                        Text = (running.ContainsKey(watch) ? "Running:" : "Stopped"),
                        PostFormat = () => Console.ForegroundColor = ConsoleColor.White
                    }, 
                    pid.HasValue ? running[watch].Id.ToString() : string.Empty,
                    binding,
                    pid.HasValue ? running[watch].StartTime.ToString("MM/dd hh:mm:ss") : string.Empty);
            }
        }

        private void WriteTabularOutput(Dictionary<int, int> colWidths, params object[] values)
        {
            for(var i = 0; i < values.Length; i++)
            {
                var colWidth = colWidths.ContainsKey(i) ? ColWidths[i] : 20;

                if (values[i].GetType() == typeof(FormattedOutput))
                {
                    var formatValue = ((FormattedOutput)values[i]);
                    formatValue.PreFormat();
                    Console.Write(formatValue.Text.ToString().PadRight(colWidth).Substring(0, colWidth));
                    formatValue.PostFormat();
                }
                else
                {
                    Console.Write(values[i].ToString().PadRight(colWidth).Substring(0, colWidth));
                }
            }

            Console.WriteLine();
        }
        
        private void WriteHeader(string text)
        {
            var margin = (Console.WindowWidth - text.Length - 2) / 2;

            Console.Write(string.Join("", Enumerable.Range(0, margin).Select(t => "=")));
            Console.Write($" {text} ");
            Console.Write(string.Join("", Enumerable.Range(0, Console.WindowWidth - (margin + 2 + text.Length)).Select(t => "=")));
        }

        
    }
}
