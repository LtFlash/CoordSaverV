using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoordSaverV
{
    interface ISpawnPointWriter
    {
        string FileExtension { get; }
        string FileName { get; set; } // to check if can be saved
        void Save(ExtendedSpawnPoint esp);
    }
}
