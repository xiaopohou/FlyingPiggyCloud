using System;
using System.Reflection;

namespace Wangsu.WcsLib.Utility
{
    // UMU: https://www.codeproject.com/tips/353819/get-all-assembly-information
    internal class AssemblyInfo
    {
        public AssemblyInfo(Assembly assembly)
        {
            this.assembly = assembly ?? throw new ArgumentNullException("assembly");
        }

        /// <summary>
        /// Gets the application's version
        /// </summary>
        public string Version
        {
            get
            {
                Version version = assembly.GetName().Version;
                if (version != null)
                {
                    return version.ToString();
                }
                else
                {
                    return "1.0.0.0";
                }
            }
        }

        private readonly Assembly assembly;
    }
}
