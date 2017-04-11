using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace DotNetCoreWatcher
{
    public static class CsprojHelper
    {
        public static string GetBinaryPath(string projectFolder, string buildType = "Debug")
        {
            var binaryPath = "";

            var csproj = new DirectoryInfo(projectFolder).GetFiles("*.csproj", SearchOption.TopDirectoryOnly)[0];

            if (csproj == null)
            {
                throw new FileNotFoundException("Unable to locate a .csproj project in " + projectFolder);
            }

            var binaryExtension = ".exe";

            var proj = XDocument.Load(csproj.FullName);
            foreach (var propGroup in proj.Root.Descendants("PropertyGroup"))
            {
                var target = propGroup.Element("TargetFramework");
                if (target != null)
                {
                    binaryPath = target.Value;
                }
                else
                {
                    var targets = propGroup.Element("TargetFrameworks");
                    binaryPath = targets.Value.Split(';')[0];
                }


                if (!string.IsNullOrWhiteSpace(binaryPath))
                {
                    var runtimeID = propGroup.Element("RuntimeIdentifier");
                    var runtimeIDs = propGroup.Element("RuntimeIdentifiers");

                    if (runtimeID != null)
                    {
                        binaryPath = Path.Combine(binaryPath, runtimeID.Value);
                    }
                    else if(runtimeIDs != null)
                    {
                        binaryPath = Path.Combine(binaryPath, runtimeID.Value.Split(';')[0]);
                    }
                    else
                    {
                        binaryExtension = ".dll";
                    }

                    break;
                }
            }

            return Path.Combine(projectFolder, "bin", buildType, binaryPath, Path.GetFileNameWithoutExtension(csproj.FullName) + binaryExtension);
        }

    }
}
