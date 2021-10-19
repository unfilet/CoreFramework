using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using UnityEngine;

public static class JsonSafe
{
    private readonly static ITraceWriter traceWriter;
    private readonly static JsonSerializerSettings serializerSettings;
    private readonly static JsonSerializer serializer;

    static JsonSafe ()
    {
        traceWriter = new MemoryTraceWriter();

        serializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            TypeNameHandling = TypeNameHandling.Auto,
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
            TraceWriter = traceWriter,
            //PreserveReferencesHandling = PreserveReferencesHandling.Objects,
        };
        serializerSettings.Converters.Add(new StringEnumConverter());
        serializerSettings.Converters.Add(new EpochDateTimeConverter());

        serializer = JsonSerializer.CreateDefault(serializerSettings);
    }

    #region Serialization

    public static string SerializeObject<T>(T value,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0) where T : class
    {
        try
        {
            var sb = new StringBuilder(256);
            using (TextWriter sw = new StringWriter(sb, CultureInfo.InvariantCulture))
            {
                Serialize<T>(sw, value, out var json,
                    memberName,
                    sourceFilePath,
                    sourceLineNumber);
                return json;
            }    
        }
        catch (NullReferenceException nullException)
        {
            Debug.LogError("Exception happened while serializing input object, Error: " + nullException.Message);
            throw nullException;
        }
    }

    public static bool SerializeIntoFile<T>(string filePath, T content,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0) where T : class
    {
        try
        {
            using Stream s = File.Open(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            using BufferedStream bs = new BufferedStream(s);
            using TextWriter sw = new StreamWriter(bs);
            return Serialize<T>(sw, content, out _,
                memberName,
                sourceFilePath,
                sourceLineNumber);
        }
        catch (NullReferenceException nullException)
        {

            Debug.LogError("Exception happened while serializing input object, Error: " + nullException.Message);
            return false;
        }
    }

    private static bool Serialize<T>(TextWriter sw, T content, out string json,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0) where T : class
    {
        try
        {
            using (JsonWriter jw = new MSJsonWriter(sw))
            {
                var serializer = JsonSerializer.CreateDefault(serializerSettings);
                serializer.ContractResolver = new InterfaceContractResolver<T>();

                jw.Formatting = serializer.Formatting;
                serializer.Serialize(jw, content, typeof(T));
                json = sw.ToString();
            }
            return true;
        }
        catch (Exception e)
        {
            json = default;
            string d = $"value: {content}\n" +
                $"member name: {memberName}\n" +
                $"source file path: {sourceFilePath}\n" +
                $"source line number: {sourceLineNumber}";
            Debug.LogError(d);
            Debug.LogError(traceWriter);
            Debug.LogError("Exception happened while serializing input object, Error: " + e.Message);
            return false;
        }
    }

    #endregion

    #region Deserialization

    public static void PopulateFromFile(string filePath, object content,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        try
        {
            using Stream stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            using BufferedStream bs = new BufferedStream(stream);
            using TextReader sr = new StreamReader(bs);
                Populate(sr, content);
            
        }
        catch (NullReferenceException nullException)
        {
            Debug.LogError("Exception happened while deserializing input object, Error: " + nullException.Message);
        }
    }

    public static bool DeserializeObject<T>(string value, out T content,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        try
        {
            using TextReader sr = new StringReader(value);
            return Deserialize<T>(sr, out content,
                memberName,
                sourceFilePath,
                sourceLineNumber);
        }
        catch (ArgumentNullException)
        {
            content = default;
            return false;
        }
        catch (NullReferenceException nullException)
        {
            content = default;
            Debug.LogError("Exception happened while deserializing input object, Error: " + nullException.Message);
            return false;
        }
    }
    
    public static bool DeserializeFromFile<T>(string filePath, out T content,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        try
        {
            using Stream stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            using BufferedStream bs = new BufferedStream(stream);
            using TextReader sr = new StreamReader(bs);
            return Deserialize<T>(sr, out content,
                memberName,
                sourceFilePath,
                sourceLineNumber);
        }
        catch (NullReferenceException nullException)
        {
            content = default;
            Debug.LogError("Exception happened while deserializing input object, Error: " + nullException.Message);
            return false;
        }
        
    }

    #region Internal

    private static bool Deserialize<T>(TextReader textReader, out T content,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        try
        {
            using (JsonReader reader = new MSJsonReader(textReader))
            {
                /**
                reader.SupportMultipleContent = true;
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        var c = serializer.Deserialize(reader);
                    }
                }
                /**/

                content = serializer.Deserialize<T>(reader);
            }
            return true;
        }
        catch (Exception e)
        {
            content = default;

            string d = $"value: {textReader}\n" +
                $"member name: {memberName}\n" +
                $"source file path: {sourceFilePath}\n" +
                $"source line number: {sourceLineNumber}";
            Debug.LogError(d);
            Debug.LogError(traceWriter);
            Debug.LogError("Exception happened while deserializing input object, Error: " + e.Message);
            return false;
        }
    }

    private static bool Populate(TextReader textReader, object content,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        try
        {
            using (JsonReader reader = new MSJsonReader(textReader))
                serializer.Populate(reader, content);
            return true;
        }
        catch (Exception e)
        {
            string d = $"value: {textReader}\n" +
                $"member name: {memberName}\n" +
                $"source file path: {sourceFilePath}\n" +
                $"source line number: {sourceLineNumber}";
            Debug.LogError(d);
            Debug.LogError(traceWriter);
            Debug.LogError("Exception happened while deserializing input object, Error: " + e.Message);
            return false;
        }
    }

    #endregion

    #endregion


    private class MSJsonReader : JsonTextReader
    {
        public MSJsonReader(TextReader reader) : base(reader) { }

        public MSJsonReader(Stream readStream, Encoding effectiveEncoding)
            : base(new StreamReader(readStream, effectiveEncoding)) { }

        public override bool Read()
        {
            var hasToken = base.Read();

            if (hasToken
                    && TokenType == JsonToken.PropertyName
                    && Value != null
                    && Value.Equals("__type"))
                SetToken(JsonToken.PropertyName, "$type");

            return hasToken;
        }
    }

    private class MSJsonWriter : JsonTextWriter
    {
        public MSJsonWriter(TextWriter textWriter) : base(textWriter) { }

        public MSJsonWriter(Stream writeStream, Encoding effectiveEncoding)
            : base(new StreamWriter(writeStream, effectiveEncoding)) { }

        public override void WritePropertyName(string name, bool escape)
        {
            if (name.StartsWith("$"))
                name = name.Replace("$", "__");
            base.WritePropertyName(name, escape);
        }
    }

    private class InterfaceContractResolver<TInterface> : CamelCasePropertyNamesContractResolver where TInterface : class
    {
        private readonly Type[] _interfaceTypes;

        private readonly ConcurrentDictionary<Type, Type> _typeToSerializeMap;

        public InterfaceContractResolver() : this(typeof(TInterface))
        { }

        public InterfaceContractResolver(params Type[] interfaceTypes) : base()
        {
            _interfaceTypes = interfaceTypes;
            _typeToSerializeMap = new ConcurrentDictionary<Type, Type>();
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var typeToSerialize = _typeToSerializeMap.GetOrAdd(
                type,
                t => _interfaceTypes.FirstOrDefault(
                    it => it.IsAssignableFrom(t)) ?? t);

            return base.CreateProperties(typeToSerialize, memberSerialization);

            //IList<JsonProperty> properties = base.CreateProperties(typeof(TInterface), memberSerialization);
            //return properties;
        }
    }
}