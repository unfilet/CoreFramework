using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CardKit.Utils
{
    public static class Utils
    {
        public static IEnumerable<Type> GetAssignableOfType<T>()
        {
            var baseType = typeof(T);
            return GetAssignableTypes<T>(baseType.Assembly);
        }

        public static IEnumerable<Type> GetSubclassOfType<T>() where T : class
        {
            var baseType = typeof(T);
            return GetDerivedTypes<T>(baseType.Assembly);
        }

        public static IEnumerable<Type> GetAllDerivedTypes<T>()
            => GetDerivedTypes<T>(AppDomain.CurrentDomain.GetAssemblies());

        public static IEnumerable<Type> GetAllAssignableTypes<T>()
            => GetAssignableTypes<T>(AppDomain.CurrentDomain.GetAssemblies());
        
        private static IEnumerable<Type> GetAssignableTypes<T>(params Assembly[] assemblies)
        {
            var baseType = typeof(T);
            return assemblies
                .SelectMany(domain => domain.GetTypes())
                .Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract);
        }
        
        private static IEnumerable<Type> GetDerivedTypes<T>(params Assembly[] assemblies)
        {
            var baseType = typeof(T);
            return assemblies
                .SelectMany(domain => domain.GetTypes())
                .Where(t => t.IsSubclassOf(baseType) && !t.IsAbstract);
        }
    }
}