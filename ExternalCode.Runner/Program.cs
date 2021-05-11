using ExternalCode.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ExternalCode.Runner
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Checking for external assemblies to load...");

            var externals = GetExternalCodeClasses("plugins");

            Console.WriteLine($"Found {externals.Length} external plugin types.");

            if (externals.Length == 0)
            {
                return;
            }

            Console.Write("Enter data: ");
            var data = Console.ReadLine();

            foreach (var item in externals)
            {
                var instance = (IExternalCode)Activator.CreateInstance(item);

                instance?.Run(data);
            }
        }

        private static Type[] GetExternalCodeClasses(string path)
        {
            var properExternalClasses = new List<Type>();

            foreach (var potentialAssembly in GetPotentialAssemblies(path))
            {
                try
                {
                    var assembly = Assembly.LoadFile(potentialAssembly);

                    var externalClasses = assembly.GetExportedTypes()
                        .Where(t => t.IsAssignableTo(typeof(IExternalCode)))
                        .ToArray();

                    if (externalClasses.Length > 0)
                    {
                        foreach (var item in externalClasses)
                        {
                            properExternalClasses.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unable to load {potentialAssembly} as external assembly: {ex.Message}");
                }
            }

            return properExternalClasses.ToArray();
        }

        private static IEnumerable<string> GetPotentialAssemblies(string path)
        {
            if (!Directory.Exists(path))
            {
                Console.WriteLine("No plugin directory exists!");
                yield break;
            }

            foreach (var item in Directory.EnumerateFiles(path, "*.dll"))
            {
                yield return Path.GetFullPath(item);
            }
        }
    }
}
