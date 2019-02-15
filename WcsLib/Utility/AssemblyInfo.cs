using System;
using System.IO;
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
        /// Gets the title property
        /// </summary>
        public string Title =>
                // UMU: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/lambda-operator
                GetAttributeValue<AssemblyTitleAttribute>(a => a.Title, Path.GetFileNameWithoutExtension(assembly.CodeBase));

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

        /// <summary>
        /// Gets the description about the application.
        /// </summary>
        public string Description => GetAttributeValue<AssemblyDescriptionAttribute>(a => a.Description);

        /// <summary>
        ///  Gets the product's full name.
        /// </summary>
        public string Product => GetAttributeValue<AssemblyProductAttribute>(a => a.Product);

        /// <summary>
        /// Gets the copyright information for the product.
        /// </summary>
        public string Copyright => GetAttributeValue<AssemblyCopyrightAttribute>(a => a.Copyright);

        /// <summary>
        /// Gets the company information for the product.
        /// </summary>
        public string Company => GetAttributeValue<AssemblyCompanyAttribute>(a => a.Company);

        protected string GetAttributeValue<T>(Func<T, string> resolveFunc, string defaultResult = null) where T : Attribute
        {
            object[] attributes = assembly.GetCustomAttributes(typeof(T), false);
            if (attributes.Length > 0)
            {
                return resolveFunc((T)attributes[0]);
            }
            else
            {
                return defaultResult;
            }
        }

        private readonly Assembly assembly;
    }
}
