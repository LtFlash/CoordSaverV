using System;
using System.Collections.Generic;

namespace CoordSaverV
{
    [Serializable]
    public class ExtendedSpawnPoint
    {
        public List<SpawnPoint> Spawn;

        public string Zone;
        public string Street;
        public List<string> Tags;

        public ExtendedSpawnPoint()
        {
            Spawn = new List<SpawnPoint>();
            Tags = new List<string>();
            Zone = string.Empty;
            Street = string.Empty;
        }
    }
}
