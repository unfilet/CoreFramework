using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Globalization;
using Newtonsoft.Json.Utilities;
using UniRx;
using Zenject;

public class ReactivePropertyConverter<T> : JsonConverter<ReactiveProperty<T>>
{
    public override ReactiveProperty<T> ReadJson(JsonReader reader, Type objectType, ReactiveProperty<T> existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        T value = default(T);
        
        if (reader.Value is T)
            value = (T)reader.Value;
        else if (typeof(T).IsEnum)
            value = (T)Enum.Parse(typeof(T), (string)reader.Value);
        else
            value = (T)Convert.ChangeType(reader.Value, typeof(T));

        return new ReactiveProperty<T>(value);
    }

    public override void WriteJson(JsonWriter writer, ReactiveProperty<T> value, JsonSerializer serializer)
    {
        object obj = value.Value;
        if (typeof(T).IsEnum)
            obj = value.Value.ToString();
        writer.WriteValue(obj);
    }
}
