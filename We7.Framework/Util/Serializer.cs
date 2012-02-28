using System.Security;
using System.Security.Permissions;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Globalization;
using System.Xml;

namespace We7.Framework.Util
{
    public class Serializer
    {
        public static readonly bool CanBinarySerialize;

        static Serializer()
        {
            SecurityPermission permission = new SecurityPermission(SecurityPermissionFlag.SerializationFormatter);
            try
            {
                permission.Demand();
                CanBinarySerialize = true;
            }
            catch (SecurityException)
            {
                CanBinarySerialize = false;
            }
        }

        public static object ConvertFileToObject(string path, Type objectType)
        {
            object obj2 = null;
            if ((path != null) && (path.Length > 0))
            {
                using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    obj2 = new XmlSerializer(objectType).Deserialize(stream);
                    stream.Close();
                }
            }
            return obj2;
        }

        public static byte[] ConvertToBytes(object objectToConvert)
        {
            byte[] buffer = null;
            if (CanBinarySerialize)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (MemoryStream stream = new MemoryStream())
                {
                    formatter.Serialize(stream, objectToConvert);
                    stream.Position = 0;
                    buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    stream.Close();
                }
            }
            return buffer;
        }

        public static T ConvertToObject<T>(string xml) where T : class
        {
            object obj2 = null;
            if (!string.IsNullOrEmpty(xml))
            {
                using (StringReader reader = new StringReader(xml))
                {
                    obj2 = new XmlSerializer(typeof(T)).Deserialize(reader);
                    reader.Close();
                }
            }
            return (obj2 as T);
        }

        public static T ConvertToObject<T>(XmlNode node) where T : class
        {
            return ConvertToObject<T>(node.OuterXml);
        }

        public static object ConvertToObject(byte[] byteArray)
        {
            object obj2 = null;
            if ((CanBinarySerialize && (byteArray != null)) && (byteArray.Length > 0))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (MemoryStream stream = new MemoryStream())
                {
                    stream.Write(byteArray, 0, byteArray.Length);
                    stream.Position = 0;
                    if (byteArray.Length > 4)
                    {
                        obj2 = formatter.Deserialize(stream);
                    }
                    stream.Close();
                }
            }
            return obj2;
        }

        public static string ConvertToString(object objectToConvert)
        {
            string str = null;
            if (objectToConvert != null)
            {
                XmlSerializer serializer = new XmlSerializer(objectToConvert.GetType());
                using (StringWriter writer = new StringWriter(CultureInfo.InvariantCulture))
                {
                    serializer.Serialize((TextWriter)writer, objectToConvert);
                    str = writer.ToString();
                    writer.Close();
                }
            }
            return str;
        }

        public static object LoadBinaryFile(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                BinaryReader reader = new BinaryReader(stream);
                byte[] buffer = new byte[stream.Length];
                reader.Read(buffer, 0, (int)stream.Length);
                return ConvertToObject(buffer);
            }
        }

        public static bool SaveAsBinary(object objectToSave, string path)
        {
            if ((objectToSave != null) && CanBinarySerialize)
            {
                byte[] buffer = ConvertToBytes(objectToSave);
                if (buffer != null)
                {
                    using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        using (BinaryWriter writer = new BinaryWriter(stream))
                        {
                            writer.Write(buffer);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static void SaveAsXML(object objectToSave, string path)
        {
            if (objectToSave != null)
            {
                XmlSerializer serializer = new XmlSerializer(objectToSave.GetType());
                using (StreamWriter writer = new StreamWriter(path))
                {
                    serializer.Serialize((TextWriter)writer, objectToSave);
                    writer.Close();
                }
            }
        }
    }
}
