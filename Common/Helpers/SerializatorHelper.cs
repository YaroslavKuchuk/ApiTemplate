using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Common.Helpers
{
    public static class SerializatorHelper
    {
        public static byte[] GetMessageBytes(object message)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, message);

                return stream.ToArray();
            }
        }

        public static T Deserialize<T>(byte[] data) where T: class
        {
            if (data == null || data.Length == 0)
                return null;

            using (var stream = new MemoryStream())
            {
                try
                {
                    stream.Write(data, 0, data.Length);
                    stream.Seek(0, SeekOrigin.Begin);
                    var formatter = new BinaryFormatter();
                    return (T)formatter.Deserialize(stream);
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
