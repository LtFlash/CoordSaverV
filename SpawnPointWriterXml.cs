using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoordSaverV
{
    internal class SpawnPointWriterXml : ISpawnPointWriter
    {
        public string FileName { get; set; }

        public string FileExtension
        {
            get
            {
                return "xml";
            }
        }

        public void Save(ExtendedSpawnPoint esp)
        {
            Serialization.AppendToXML(esp, FileName);
        }
    }
}
