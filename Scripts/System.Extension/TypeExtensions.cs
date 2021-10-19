using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class TypeExtensions
{
    public static FieldInfo GetPrivateField(this Type t, String name, BindingFlags bf = 
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
    {
        FieldInfo fi;
        while ((fi = t.GetField(name, bf)) == null && (t = t.BaseType) != null);
        return fi;
    }

    public static MethodInfo GetPrivateMethod(this Type t, String name, BindingFlags bf =
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
    {
        MethodInfo fi;
        while ((fi = t.GetMethod(name, bf)) == null && (t = t.BaseType) != null) ;
        return fi;
    }

    public static bool IsAssignableTo(this Type type, Type assignableType)
    {
        return assignableType.IsAssignableFrom(type);
    }

    public static bool IsAssignableTo<TAssignable>(this Type type)
    {
        return IsAssignableTo(type, typeof(TAssignable));
    }

    /// <summary>
    /// Return all interfaces implemented by the incoming type as well as the type itself if it is an interface.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static IEnumerable<Type> GetInterfacesAndSelf(this Type type)
    {
        if (type == null)
            throw new ArgumentNullException();
        if (type.IsInterface)
            return new[] { type }.Concat(type.GetInterfaces());
        else
            return type.GetInterfaces();
    }

    public static IEnumerable<KeyValuePair<Type, Type>> GetDictionaryKeyValueTypes(this Type type)
    {
        foreach (Type intType in type.GetInterfacesAndSelf())
        {
            if (intType.IsGenericType
                && intType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
            {
                var args = intType.GetGenericArguments();
                if (args.Length == 2)
                    yield return new KeyValuePair<Type, Type>(args[0], args[1]);
            }
        }
    }
}
