﻿#if NET45 || NETSTANDARD2_0

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace FluentAssertions.Common
{
    internal class FullFrameworkReflector : IReflector
    {
        public IEnumerable<Type> GetAllTypesFromAppDomain(Func<Assembly, bool> predicate)
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => !IsDynamic(a) && IsRelevant(a) && predicate(a))
                .SelectMany(GetExportedTypes).ToArray();
        }

        private bool IsRelevant(Assembly ass)
        {
            string assemblyName = ass.GetName().Name.ToLower();

            return !assemblyName.StartsWith("microsoft.") &&
                   !assemblyName.StartsWith("xunit") &&
                   !assemblyName.StartsWith("jetbrains.") &&
                   !assemblyName.StartsWith("system") &&
                   !assemblyName.StartsWith("mscorlib") &&
                   !assemblyName.StartsWith("newtonsoft");
        }


        private static bool IsDynamic(Assembly assembly)
        {
            return (assembly is AssemblyBuilder) ||
                   (assembly.GetType().FullName == "System.Reflection.Emit.InternalAssemblyBuilder");
        }

        private static IEnumerable<Type> GetExportedTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetExportedTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types;
            }
            catch (FileLoadException)
            {
                return new Type[0];
            }
            catch (Exception)
            {
                return new Type[0];
            }
        }
    }
}

#endif