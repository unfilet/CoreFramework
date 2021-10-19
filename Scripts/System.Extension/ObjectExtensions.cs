using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class ObjectExtensions
{
    public static byte[] Serialize(this object obj)
    {
        BinaryFormatter binaryF = new BinaryFormatter();
        using (MemoryStream memoryStream = new MemoryStream())
        {
            binaryF.Serialize(memoryStream, obj);
            return memoryStream.ToArray();
        }
    }

    public static T Deserialize<T>(this byte[] dataStream)
    {
        return (T)Deserialize(dataStream);
    }

    public static object Deserialize(this byte[] dataStream)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            BinaryFormatter binaryF = new BinaryFormatter();
            memoryStream.Write(dataStream, 0, dataStream.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return binaryF.Deserialize(memoryStream);
        }
    }
}