//using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
//using Rage;

namespace CoordSaverV
{
    internal static class Serialization
    {
        public static List<T> LoadFromXML<T>(string filePath)
        {
            List<T> list = new List<T>();

            XmlSerializer deserializer = new XmlSerializer(typeof(List<T>));
            using (TextReader reader = new StreamReader(filePath))
            {
                list = new List<T>();
                list = (List<T>)deserializer.Deserialize(reader);
            }

            return list;
        }

        public static void SaveToXML<T>(List<T> list, string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
            using (TextWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, list);
            }
        }

        public static void AppendToXML<T>(T ObjectToAdd, string filePath)
        {
            string directoryName = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }

            if(File.Exists(filePath))
            {
                List<T> listOfSpawns = LoadFromXML<T>(filePath);
                listOfSpawns.Add(ObjectToAdd);

                SaveToXML<T>(listOfSpawns, filePath);
            }
            else
            {
                List<T> list = new List<T>();
                list.Add(ObjectToAdd);

                SaveToXML<T>(list, filePath);
            }
        }
    }
}