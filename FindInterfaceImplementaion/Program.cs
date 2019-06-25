using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace FindInterfaceImplementation
{
    class Program
    {
        static void Main(string[] args)
        {
            var name = typeof(Program).Assembly.GetName().Name;
            Console.WriteLine($"AssemblyName: {name}");

            var assemblies = GetReferencingAssemblies1(name);
            List<string> result = new List<string>();
            foreach (var assembly in assemblies)
            {
                
                foreach(var impl in assembly.GetExportedTypes().Where(p => p.GetInterfaces().Contains(typeof(ITalkable)))) 
                {
                    Console.WriteLine($"Impl: {impl}");
                    var method = impl.GetMethod("Talk");
                    var obj = Activator.CreateInstance(impl);
                    method.Invoke(obj, null);
                    var method2 = impl.GetMethod("Append");
                    method2.Invoke(obj, new object[] {result});
                }

            }
            Console.WriteLine(assemblies.Count());
            Console.WriteLine($"Result: {result[0]} {result[1]}");
        }

        public static IEnumerable<Assembly> GetReferencingAssemblies1(string assemblyName)
        {
            return DependencyContext.Default.RuntimeLibraries.Where(p => IsCandidateLibrary(p, assemblyName)).Select(
                p => Assembly.Load(new AssemblyName(p.Name)));
        }

        private static bool IsCandidateLibrary(RuntimeLibrary library, string assemblyName)
        {
            return library.Name == assemblyName || library.Dependencies.Any(d => d.Name.StartsWith(assemblyName));
        }

        // The implementation of this blog post http://www.michael-whelan.net/replacing-appdomain-in-dotnet-core/
        public static IEnumerable<Assembly> GetReferencingAssemblies2(string assemblyName)
        {
            var assemblies = new List<Assembly>();
            var dependencies = DependencyContext.Default.RuntimeLibraries;
            foreach (var library in dependencies)
            {
                if (IsCandidateLibrary(library, assemblyName))
                {
                    var assembly = Assembly.Load(new AssemblyName(library.Name));
                    assemblies.Add(assembly);
                }
            }
            return assemblies;
        }
    }


    public interface ITalkable
    {
        void Talk();
        void Append(List<string> str);
    }

    public class Hello : ITalkable
    {
        public void Talk()
        {
            Console.WriteLine("Hello!");
        }

        public void Append(List<string> str)
        {
            str.Add("Hello!");
        }
    }
}
