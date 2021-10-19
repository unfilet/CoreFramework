using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;

public static class ObjectToDictionaryHelper
{
    public static Dictionary<string, object> ToDictionary(this object source)
    {
        return source.ObjectToDictionary<object>();
    }

    public static Dictionary<string, TValue> ObjectToDictionary<TValue>(this object source)
    {
        if (source == null) ThrowExceptionWhenSourceArgumentIsNull();

        var dictionary = new Dictionary<string, TValue>();

        foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
        {
            object value = property.GetValue(source);
            if (IsOfType<TValue>(value) && property.Attributes[typeof(JsonPropertyAttribute)] != null )
            {
                dictionary.Add(property.Name, (TValue)value);
            }
        }
        return dictionary;
    }

    private static bool IsOfType<T>(object value)
    {
        return value is T;
    }

    private static void ThrowExceptionWhenSourceArgumentIsNull()
    {
        throw new NullReferenceException("Unable to convert anonymous object to a dictionary. The source anonymous object is null.");
    }
}