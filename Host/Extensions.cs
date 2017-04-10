using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Host
{
    public static class Extensions
    {
        public static void CloneDirectory(string source, string destination)
        {
            Directory.CreateDirectory(destination);

            foreach (var dir in Directory.GetDirectories(source, "*", System.IO.SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(destination + dir.Substring(source.Length));
            }

            foreach (var file in Directory.GetFiles(source, "*", System.IO.SearchOption.AllDirectories))
            {
                File.Copy(file, destination + file.Substring(source.Length));
            }
        }
    }
}
