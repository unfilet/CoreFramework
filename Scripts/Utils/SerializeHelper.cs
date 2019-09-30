using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using System;
using System.IO;

namespace Core.Scripts.Utils
{
    public static class SerializeHelper
    {
        public static bool SerializeIntoJson<T>(string filePath, T content)
        {
            try
            {
                using (Stream s = File.Open(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                using (BufferedStream bs = new BufferedStream(s))
                using (StreamWriter sw = new StreamWriter(bs))
                using (JsonTextWriter jw = new JsonTextWriter(sw))
                {
                    MemoryTraceWriter traceWriter = new MemoryTraceWriter();
                    JsonSerializer serializer = new JsonSerializer();

                    //serializer.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    //serializer.TypeNameHandling = TypeNameHandling.All;
                    serializer.Converters.Add(new StringEnumConverter());
                    serializer.NullValueHandling = NullValueHandling.Ignore;
                    serializer.Formatting = Formatting.Indented;
                    serializer.TraceWriter = traceWriter;

                    serializer.Serialize(jw, content);
                    //Debug.Log(traceWriter);
                }
                return true;
            }
            catch (NullReferenceException nullException)
            {

                Debug.LogError("Exception happened while serializing input object, Error: " + nullException.Message);
                return false;
            }
            catch (Exception e)
            {

                Debug.LogError("Exception happened while serializing input object, Error: " + e.Message);
                return false;
            }
        }

        public static bool DeserializeFromJson<T>(string filePath, out T content)
        {
            try
            {
                using (Stream s = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (BufferedStream bs = new BufferedStream(s))
                using (StreamReader sr = new StreamReader(bs))
                using (JsonTextReader jr = new JsonTextReader(sr))
                {
                    MemoryTraceWriter traceWriter = new MemoryTraceWriter();
                    JsonSerializer serializer = new JsonSerializer();

                    //serializer.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    //serializer.TypeNameHandling = TypeNameHandling.All;
                    serializer.Converters.Add(new StringEnumConverter());
                    serializer.NullValueHandling = NullValueHandling.Ignore;
                    serializer.Formatting = Formatting.Indented;
                    serializer.TraceWriter = traceWriter;

                    content = serializer.Deserialize<T>(jr);
                    //Debug.Log(traceWriter);
                }
                return true;
            }
            catch (NullReferenceException nullException)
            {
                content = default;
                Debug.LogError("Exception happened while deserializing input object, Error: " + nullException.Message);
                return false;
            }
            catch (Exception e)
            {
                content = default;
                Debug.LogError("Exception happened while deserializing input object, Error: " + e.Message);
                return false;
            }
        }
    }
}